using Microsoft.Extensions.Configuration;
using OcelotApiGw.Enums;

namespace OcelotApiGw.Orchestrators
{
    public class Kubernetes : IOrchestratorStrategy
    {
        public OrchestratorType OrchestratorType => OrchestratorType.K8S;

        public IConfigurationBuilder ConfigureOrchestrator(IConfigurationBuilder builder) => builder;
    }
}
