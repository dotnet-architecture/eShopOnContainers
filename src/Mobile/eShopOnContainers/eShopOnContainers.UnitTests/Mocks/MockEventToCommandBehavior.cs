using eShopOnContainers.Core.Behaviors;

namespace eShopOnContainers.UnitTests
{
	public class MockEventToCommandBehavior : EventToCommandBehavior
	{
		public void RaiseEvent(params object[] args)
		{
			_handler.DynamicInvoke(args);
		}
	}
}
