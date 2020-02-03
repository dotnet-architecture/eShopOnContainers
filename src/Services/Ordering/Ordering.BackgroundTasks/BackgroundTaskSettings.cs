namespace Ordering.BackgroundTasks
{
    public class BackgroundTaskSettings
    {
        public string ConnectionString { get; set; }

        public string EventBusConnection { get; set; }

        public int GracePeriodTime { get; set; }

        public int CheckUpdateTime { get; set; }

        public string SubscriptionClientName { get; set; }
    }
}
