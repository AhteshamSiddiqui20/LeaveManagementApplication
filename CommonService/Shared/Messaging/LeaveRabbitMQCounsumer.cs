using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
namespace CommonService.Shared.Messaging
{
    public class LeaveRabbitMQCounsumer
    {
        private readonly string _queueName;
        private readonly string _exchange;
        private readonly string _routingKey;

        public LeaveRabbitMQCounsumer(string exchange, string queue, string routingKey)
        {
            _exchange = exchange;
            _queueName = queue;
            _routingKey = routingKey;
        }

        public void Start<T>(Action<T> onMessage)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(_exchange, ExchangeType.Direct);
            channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(_queueName, _exchange, _routingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deserialized = System.Text.Json.JsonSerializer.Deserialize<T>(message);
                onMessage(deserialized!);
            };

            channel.BasicConsume(_queueName, autoAck: true, consumer: consumer);
        }
    }
}

