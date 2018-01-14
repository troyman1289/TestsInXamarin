using Android.App;
using Android.Widget;
using Android.OS;
using Xunit.Runners.UI;
using xUnit.IntegrationTest;
using Xunit.Sdk;

namespace Net_Standard.Droid
{
    [Activity(Label = "Net_Standard.Droid", MainLauncher = true)]
    public class MainActivity : RunnerActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);
            AddTestAssembly(typeof(MainViewModelTest).Assembly);
            base.OnCreate(savedInstanceState);
        }
    }
}

