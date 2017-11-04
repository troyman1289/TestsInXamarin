using Android.App;
using Android.Widget;
using Android.OS;
using Xunit.Runners.UI;
using Xunit.Sdk;

namespace xUnit.IntegrationTest.Droid
{
    [Activity(Label = "xUnit.IntegrationTest.Droid", MainLauncher = true)]
    public class MainActivity : RunnerActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);
            AddTestAssembly(typeof(CalculationManagerTest).Assembly);

            base.OnCreate(savedInstanceState);
        }
    }
}

