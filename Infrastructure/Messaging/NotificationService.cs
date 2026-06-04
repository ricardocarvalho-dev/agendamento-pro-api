using System.Text;
using System.Text.Json;
using Application.Interfaces;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging;

public class NotificationService : IMessagingService
{
    private IConnection? _connection;
    private readonly ILogger<NotificationService> _logger;
    //private readonly string _queueName = "agendamentos.eventos";
    private readonly string _queueName = "fila_agendamentos";
    private readonly string _connectionUrl;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
        
        // Lê a variável de ambiente
        _connectionUrl = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING") 
            ?? "amqp://guest:guest@localhost:5672/";
            
        _logger.LogInformation($"✅ NotificationService inicializado (conexão será feita quando necessário)");
    }

    private IConnection GetConnection()
    {
        if (_connection == null || _connection.IsOpen == false)
        {
            try
            {
                _logger.LogInformation($"Conectando ao RabbitMQ: {_connectionUrl.Split('@')[0]}@...");
                
                var factory = new ConnectionFactory() 
                { 
                    Uri = new Uri(_connectionUrl),
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true
                };

                _connection = factory.CreateConnection();
                _logger.LogInformation("✅ Conectado ao RabbitMQ com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao conectar ao RabbitMQ");
                throw;
            }
        }

        return _connection;
    }

    public void EnviarNotificacaoAgendamento(object notificacao)
    {
        try
        {
            var connection = GetConnection();
            
            using (var channel = connection.CreateModel())
            {
                // Declara a exchange
                channel.ExchangeDeclare(
                    exchange: "agendamentos.exchange",
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false
                );

                // Declara a fila
                channel.QueueDeclare(
                    //queue: _queueName,
                    queue: "fila_agendamentos",  // ← Mude aqui também
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                // Faz o binding
                channel.QueueBind(
                    queue: _queueName,
                    exchange: "agendamentos.exchange",
                    routingKey: "agendamento.criado"
                );

                // Serializa a notificação
                var json = JsonSerializer.Serialize(notificacao);
                var body = Encoding.UTF8.GetBytes(json);

                // Define propriedades
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentType = "application/json";

                // Publica
                channel.BasicPublish(
                    exchange: "agendamentos.exchange",
                    routingKey: "agendamento.criado",
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation($"✅ Mensagem publicada no RabbitMQ: {json}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao enviar notificação para RabbitMQ");
            throw;
        }
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }
}