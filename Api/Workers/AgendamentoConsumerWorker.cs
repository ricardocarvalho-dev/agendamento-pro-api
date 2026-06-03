using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace API.Workers;

public class AgendamentoConsumerWorker : BackgroundService
{
    private readonly ILogger<AgendamentoConsumerWorker> _logger;
    private readonly string _connectionUrl = "amqps://wgseihrb:KFWP5XSX1-6GYn1CevzJcu_kvgBmipc9@shark.rmq.cloudamqp.com/wgseihrb";
    private IConnection? _connection;
    private IModel? _channel;

    public AgendamentoConsumerWorker(ILogger<AgendamentoConsumerWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Worker] Iniciando o consumidor de agendamentos...");

        try
        {
            var factory = new ConnectionFactory() { Uri = new Uri(_connectionUrl) };
            
            // Abre conexão e canal
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Garante que a fila existe antes de consumir
            _channel.QueueDeclare(queue: "agendamentos.eventos",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _logger.LogInformation("✅ Conectado ao RabbitMQ. Aguardando mensagens...");

            // Cria o consumidor de eventos
            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = System.Text.Encoding.UTF8.GetString(body);
                
                _logger.LogInformation($"\n==================================================\n" +
                                       $"[Worker] MENSAGEM RECEBIDA DO CLOUDAMQP!\n" +
                                       $"Conteúdo: {message}\n" +
                                       $"==================================================");

                // Simulação de envio de e-mail ou WhatsApp
                Thread.Sleep(500); 

                // Dá o "Ack" (Acknowledge) avisando o CloudAMQP que a mensagem foi processada
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            // Inicia a leitura da fila
            _channel.BasicConsume(queue: "agendamentos.eventos",
                                  autoAck: false,
                                  consumer: consumer);

            // Mantém o Worker vivo escutando a fila
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro no AgendamentoConsumerWorker");
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}