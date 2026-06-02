using System.Text;
using System.Text.Json;
using Application.Interfaces;
using RabbitMQ.Client;

namespace Infrastructure.Messaging
{
    public class NotificationService : IMessagingService
    {
        private readonly string _connectionUrl = "amqps://wgseihrb:KFWP5XSX1-6GYn1CevzJcu_kvgBmipc9@shark.rmq.cloudamqp.com/wgseihrb";

        public void EnviarNotificacaoAgendamento(object dadosAgendamento)
        {
            // COMENTADO TUDO temporariamente para remover o Deadlock de 60 segundos na Azure F1
            /*
            var factory = new ConnectionFactory() { Uri = new Uri(_connectionUrl) };
            
            using var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            using var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

            channel.QueueDeclareAsync(queue: "fila_agendamentos",
                                      durable: true,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null).GetAwaiter().GetResult();

            var json = JsonSerializer.Serialize(dadosAgendamento);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublishAsync(exchange: "",
                                      routingKey: "fila_agendamentos",
                                      mandatory: false,
                                      basicProperties: new BasicProperties { DeliveryMode = DeliveryModes.Persistent },
                                      body: body).GetAwaiter().GetResult();
            */
        }
    }
}