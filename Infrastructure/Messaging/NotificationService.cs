using System.Text;
using System.Text.Json;
using Application.Interfaces;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging;

public class NotificationService : IMessagingService
{
    private readonly IConnection _connection;
    private readonly ILogger<NotificationService> _logger;
    private readonly string _queueName = "agendamentos.eventos";

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;

        try
        {
            // Obtém a string de conexão do RabbitMQ
            var rabbitMqUri = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING") 
                ?? "amqp://guest:guest@localhost:5672/";

            _logger.LogInformation($"Conectando ao RabbitMQ: {rabbitMqUri.Split('@')[0]}@...");

            var factory = new ConnectionFactory() 
            { 
                Uri = new Uri(rabbitMqUri),
                DispatchConsumersAsync = true
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

    public void EnviarNotificacaoAgendamento(object notificacao)
    {
        try
        {
            using (var channel = _connection.CreateModel())
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
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                // Faz o binding entre exchange e fila
                channel.QueueBind(
                    queue: _queueName,
                    exchange: "agendamentos.exchange",
                    routingKey: "agendamento.criado"
                );

                // Serializa a notificação
                var json = JsonSerializer.Serialize(notificacao);
                var body = Encoding.UTF8.GetBytes(json);

                // Define propriedades da mensagem
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true; // Persiste a mensagem em disco
                properties.ContentType = "application/json";

                // Publica a mensagem
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