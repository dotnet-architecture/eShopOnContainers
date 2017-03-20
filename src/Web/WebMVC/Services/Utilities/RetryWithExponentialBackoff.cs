using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    /// <summary>
    /// When working with cloud services and Docker containers, it's very important to always catch
    /// TimeoutException, and retry the operation.  
    /// RetryWithExponentialBackoff makes it easy to implement such pattern. 
    /// Usage:
    ///     var retry = new RetryWithExponentialBackoff();
    ///     await retry.RunAsync(async ()=>
    ///     {
    ///        // work with HttpClient
    ///     });
    /// </summary>
    public sealed class RetryWithExponentialBackoff
    {
        private readonly int maxRetries, delayMilliseconds, maxDelayMilliseconds;

        public RetryWithExponentialBackoff(int maxRetries = 50, int delayMilliseconds = 200, int maxDelayMilliseconds = 2000)
        {
            this.maxRetries = maxRetries;
            this.delayMilliseconds = delayMilliseconds;
            this.maxDelayMilliseconds = maxDelayMilliseconds;
        }

        public async Task RunAsync(Func<Task> func)
        {
            ExponentialBackoff backoff = new ExponentialBackoff(this.maxRetries, this.delayMilliseconds, this.maxDelayMilliseconds);
        retry:
            try
            {
                await func();
            }
            catch (Exception ex) when (ex is TimeoutException || ex is System.Net.Http.HttpRequestException)
            {
                Debug.WriteLine("Exception raised is: " + ex.GetType().ToString() + " -- Message: " + ex.Message + " -- Inner Message: " + ex.InnerException.Message);
                await backoff.Delay();
                goto retry;
            }
        }
    }


    /// <summary>
    /// Usage:
    /// ExponentialBackoff backoff = new ExponentialBackoff(3, 10, 100);
    /// retry:
    /// try {
    ///        // ...
    /// }
    /// catch (Exception ex) {
    ///    await backoff.Delay(cancellationToken);
    ///    goto retry;
    /// }
    /// </summary>
    public struct ExponentialBackoff
    {
        private readonly int m_maxRetries, m_delayMilliseconds, m_maxDelayMilliseconds;
        private int m_retries, m_pow;

        public ExponentialBackoff(int maxRetries, int delayMilliseconds, int maxDelayMilliseconds)
        {
            m_maxRetries = maxRetries;
            m_delayMilliseconds = delayMilliseconds;
            m_maxDelayMilliseconds = maxDelayMilliseconds;
            m_retries = 0;
            m_pow = 1;
        }

        public Task Delay()
        {
            if (m_retries == m_maxRetries)
            {
                throw new TimeoutException("Max retry attempts exceeded.");
            }
            ++m_retries;
            if (m_retries < 31)
            {
                m_pow = m_pow << 1; // m_pow = Pow(2, m_retries - 1)
            }
            int delay = Math.Min(m_delayMilliseconds * (m_pow - 1) / 2, m_maxDelayMilliseconds);
            return Task.Delay(delay);
        }
    }
}
