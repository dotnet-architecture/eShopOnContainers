namespace EventBusKafka;

public interface IKafkaPersistentConnection : IDisposable
{
    Handle Handle { get; }
}