using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure
{
    public enum EventStateEnum
    {
        NotSend = 0,
        Sent = 1,
        SendingFailed = 2
    }
}
