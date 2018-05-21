using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.BackgroundTasksHost.Configuration
{
    public class BackgroundTaskSettings
    {
        public string ConnectionString { get; set; }

        public string EventBusConnection { get; set; }

        public int GracePeriodTime { get; set; }

        public int CheckUpdateTime { get; set; }
    }
}
