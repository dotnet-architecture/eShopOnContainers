using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebStatus.Viewmodels
{
    public class NamedCheckResult
    {
        public string Name { get; }
        public IHealthCheckResult Result { get; }

        public NamedCheckResult(string name, IHealthCheckResult result)
        {
            Name = name;
            Result = result;
        }
    }
}
