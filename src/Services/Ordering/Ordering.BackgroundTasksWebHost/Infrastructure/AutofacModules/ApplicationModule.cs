using Autofac;

namespace Ordering.BackgroundTasksWebHost.Infrastructure.AutofacModules
{

    public class ApplicationModule
        :Autofac.Module
    {        
        public ApplicationModule(string qconstr)
        {            
        }

        protected override void Load(ContainerBuilder builder)
        {           

        }
    }
}
