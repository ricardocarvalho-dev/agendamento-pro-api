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
    private IChannel? _channel;

    public AgendamentoConsumerWorker(ILogger<AgendamentoConsumerWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Worker] Iniciando o consumidor de agendamentos...");

        var factory = new ConnectionFactory() { Uri = new Uri(_connectionUrl) };
        
        // Abre conexão e canal de forma assíncrona (Padrão v7)
        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // Garante que a fila existe antes de consumir
        await _channel.QueueDeclareAsync(queue: "fila_agendamentos",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null,
                                  cancellationToken: stoppingToken);

        // Cria o consumidor de eventos
        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            _logger.LogInformation($"\n==================================================\n" +
                                   $"[Worker] MENSAGEM RECEBIDA DO CLOUDAMQP!\n" +
                                   $"Conteúdo: {message}\n" +
                                   $"==================================================");

            // Simulação de envio de e-mail ou WhatsApp
            await Task.Delay(500, stoppingToken); 

            // Dá o "Ack" (Acknowledge) avisando o CloudAMQP que a mensagem foi processada com sucesso.
            // Isso faz a mensagem SUMIR da fila permanentemente.
            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        };

        // Inicia a leitura da fila de fato
        await _channel.BasicConsumeAsync(queue: "fila_agendamentos",
                                  autoAck: false, // Usamos false para só deletar após processar tudo no bloco acima
                                  consumer: consumer,
                                  cancellationToken: stoppingToken);

        // Mantém o Worker vivo escutando a fila
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}