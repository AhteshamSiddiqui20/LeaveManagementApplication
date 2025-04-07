namespace CommonService.Shared.Messaging
{
    public interface IUserRabbitMQProducer
    {
        void Publish<T>(T message, string exchange, string routingKey);
    }
}
