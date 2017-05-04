using System.Reflection;
using Xunit.Runners.UI;

namespace eShopOnContainers.TestRunner.Windows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : RunnerApplication
    {
        protected override void OnInitializeRunner()
        {    
            // Otherwise you need to ensure that the test assemblies will 
            // become part of the app bundle
            AddTestAssembly(typeof(UnitTests.CatalogViewModelTests).GetTypeInfo().Assembly);
        }
    }
}