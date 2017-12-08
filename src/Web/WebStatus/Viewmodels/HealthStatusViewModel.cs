using Microsoft.Extensions.HealthChecks;
using System.Collections.Generic;
using System.Linq;

namespace WebStatus.Viewmodels
{
    public class HealthStatusViewModel
    {
        private readonly CheckStatus _overall;

        private readonly Dictionary<string, IHealthCheckResult> _results;

        public CheckStatus OverallStatus => _overall;

        public IEnumerable<NamedCheckResult> Results => _results.Select(kvp => new NamedCheckResult(kvp.Key, kvp.Value));

        private HealthStatusViewModel() => _results = new Dictionary<string, IHealthCheckResult>();

        public HealthStatusViewModel(CheckStatus overall) : this() => _overall = overall;

        public void AddResult(string name, IHealthCheckResult result) => _results.Add(name, result);
    }
}
