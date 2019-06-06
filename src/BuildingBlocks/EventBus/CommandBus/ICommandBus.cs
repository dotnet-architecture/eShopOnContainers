using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.BuildingBlocks.CommandBus
{
    public interface ICommandBus
    {
        void Send<T>(string name, T data);
        void Handle<TC>(string name, IIntegrationCommandHandler<TC> handler);
        void Handle(string name, IIntegrationCommandHandler handler);
        void Handle<TI, TC>(TI handler) 
            where TI : IIntegrationCommandHandler<TC>;
    }
}
