using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebStatus.Viewmodels
{
    public class HealthStatusViewModel
    {
        private readonly CheckStatus _overall;
        private readonly List<IHealthCheckResult> _results;

        public CheckStatus OverallStatus => _overall;
        public IEnumerable<IHealthCheckResult> Results => _results;
        private HealthStatusViewModel() => _results = new List<IHealthCheckResult>();
        public HealthStatusViewModel(CheckStatus overall) : this() => _overall = overall;
        public void AddResult(IHealthCheckResult result) => _results.Add(result);
   
       
    }
}
