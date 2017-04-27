using Android.App;
using Android.OS;
using Xunit.Runners.UI;
using Xunit.Sdk;

namespace eShopOnContainers.TestRunner.Droid
{
    [Activity(Label = "eShopOnContainers.TestRunner.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : RunnerActivity
    {
        protected override void OnCreate(Bundle bundle)
        {         
            // We need this to ensure the execution assembly is part of the app bundle
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);

            // or in any reference assemblies getting the Assembly from any type/class	
			AddTestAssembly(typeof(UnitTests.CatalogViewModelTests).Assembly);

            base.OnCreate(bundle);
        }
    }
}

