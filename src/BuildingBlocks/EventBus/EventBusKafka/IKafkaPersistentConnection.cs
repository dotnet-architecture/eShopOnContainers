namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusKafka;


public interface IKafkaPersistentConnection : IDisposable
{
    Handle Handle { get; }
}