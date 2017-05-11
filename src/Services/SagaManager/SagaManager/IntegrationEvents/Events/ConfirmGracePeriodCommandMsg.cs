namespace SagaManager.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    // Integration Events notes: 
    // An Event is “something that has happened in the past”, therefore its name has to be   
    // An Integration Event is an event that can cause side effects to other microsrvices, Bounded-Contexts or external systems.
    public class ConfirmGracePeriodCommandMsg : IntegrationEvent
    {
        public int OrderId { get;}

        public ConfirmGracePeriodCommandMsg(int orderId) => OrderId = orderId;
    }
}
