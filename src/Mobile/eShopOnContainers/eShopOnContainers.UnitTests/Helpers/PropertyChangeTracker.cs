using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace eShopOnContainers.UnitTests.Helpers
{
    public class PropertyChangeTracker
    {
        List<string> _notifications = new List<string>();

        public PropertyChangeTracker(INotifyPropertyChanged changer)
        {
            changer.PropertyChanged += (sender, e) => _notifications.Add(e.PropertyName + ".Value");
        }

        //public string[] ChangedProperties
        //{
        //    get { return _notifications.ToArray(); }
        //}

        public bool WaitForChange(string propertyName, int maxWaitMilliSeconds)
        {
            var startTime = DateTime.UtcNow;
            while (!_notifications.Contains(propertyName))
            {
                if (startTime.AddMilliseconds(maxWaitMilliSeconds) < DateTime.UtcNow)
                    return false;

            }
            return true;
        }

        public bool WaitForChange(string propertyName, TimeSpan maxWait)
        {
            var startTime = DateTime.UtcNow;
            while (!_notifications.Contains(propertyName))
            {
                if (startTime + maxWait < DateTime.UtcNow)
                    return false;

            }
            return true;
        }

        public void Reset()
        {
            _notifications.Clear();
        }
    }
}
