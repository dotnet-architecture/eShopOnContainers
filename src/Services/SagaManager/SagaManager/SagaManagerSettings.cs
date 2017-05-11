namespace SagaManager
{
    public class SagaManagerSettings
    {
        public string ConnectionString { get; set; }

        public string EventBusConnection { get; set; }

        public int GracePeriod { get; set; }
    }
}