namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;

public interface IDynamicIntegrationEventHandler
{
    Task Handle(dynamic eventData);
}
