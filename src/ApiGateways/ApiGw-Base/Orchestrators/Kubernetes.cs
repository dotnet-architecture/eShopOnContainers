using Microsoft.Extensions.Configuration;
using OcelotApiGw.Enums;
using System.IO;

namespace OcelotApiGw.Orchestrators
{
    public class Kubernetes : IOrchestratorStrategy
    {
        public OrchestratorType OrchestratorType => OrchestratorType.K8S;

        public IConfigurationBuilder ConfigureOrchestrator(IConfigurationBuilder builder)
        {
            builder.AddJsonFile(Path.Combine("configuration", "configuration.json"));

            return builder;
        }
    }
}
