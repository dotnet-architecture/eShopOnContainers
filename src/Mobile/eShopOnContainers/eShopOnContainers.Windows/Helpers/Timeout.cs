using System;
using System.Threading;
using System.Threading.Tasks;

namespace eShopOnContainers.Windows.Helpers
{
    internal class Timeout
    {
        public const int Infinite = -1;
        readonly CancellationTokenSource _canceller = new CancellationTokenSource();

        public Timeout(int timeout, Action timesUp)
        {
            if (timeout == Infinite)
                return;
            if (timeout < 0)
                throw new ArgumentOutOfRangeException("timeoutMilliseconds");
            if (timesUp == null)
                throw new ArgumentNullException("timesUp");

            Task.Delay(TimeSpan.FromMilliseconds(timeout), _canceller.Token).ContinueWith(t =>
            {
                if (!t.IsCanceled)
                    timesUp();
            });
        }

        public void Cancel()
        {
            _canceller.Cancel();
        }
    }
}
