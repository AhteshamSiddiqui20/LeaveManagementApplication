

using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CommonService.Shared.Messaging
{
    public class UserRabbitMQProducer: IUserRabbitMQProducer
    {
        private readonly IModel _channel;

        public UserRabbitMQProducer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
        }

        public void Publish<T>(T message, string exchange, string routingKey)
        {
            _channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(exchange, routingKey, null, body);
        }
    }
}
