using System;

namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBus
{
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        public class SubscriptionInfo
        {
            public bool IsDynamic { get; }
            public Delegate Factory { get; }

            private SubscriptionInfo(bool isDynamic, Delegate factory)
            {
                IsDynamic = isDynamic;
                Factory = factory;
            }

            public static SubscriptionInfo Dynamic(Delegate factory)
            {
                return new SubscriptionInfo(true, factory);
            }
            public static SubscriptionInfo Typed(Delegate factory)
            {
                return new SubscriptionInfo(false, factory);
            }
        }
    }
}
