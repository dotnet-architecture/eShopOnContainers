using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.BuildingBlocks.CommandBus
{
    public abstract class IntegrationCommand
    {
        public Guid Id { get; }
        public DateTime Sent { get; }

        public abstract object GetDataAsObject();

        protected IntegrationCommand()
        {
            Id = Guid.NewGuid();
            Sent = DateTime.UtcNow;
        }

    }

    public class IntegrationCommand<T> : IntegrationCommand
    {
        public T Data { get; }
        public string Name { get; }
        public override object GetDataAsObject() => Data;

        public IntegrationCommand(string name, T data) : base()
        {
            Data = data;
            Name = name;
        }
    }

}
