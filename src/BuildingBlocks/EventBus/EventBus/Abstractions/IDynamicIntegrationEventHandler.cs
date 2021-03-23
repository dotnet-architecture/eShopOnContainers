using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
