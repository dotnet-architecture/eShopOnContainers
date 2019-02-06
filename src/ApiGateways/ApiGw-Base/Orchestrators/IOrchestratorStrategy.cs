using Microsoft.Extensions.Configuration;
using OcelotApiGw.Enums;

namespace OcelotApiGw.Orchestrators
{
    public interface IOrchestratorStrategy
    {
        OrchestratorType OrchestratorType { get; }

        IConfigurationBuilder ConfigureOrchestrator(IConfigurationBuilder builder);
    }
}
