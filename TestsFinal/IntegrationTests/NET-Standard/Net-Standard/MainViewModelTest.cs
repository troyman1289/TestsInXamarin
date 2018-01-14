using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.Model;
using Backend.RestService;
using ViewModels;
using Xamarin.Forms;
using Xunit;

namespace xUnit.IntegrationTest
{

    public class MainViewModelTest : IDisposable
    {
        private readonly MainViewModel _mainViewModel;
        private readonly ISqliteConnectionService _connectionService;
        private readonly CalculationManager _calculationManager;

        public MainViewModelTest()
        {
            _connectionService = DependencyService.Get<ISqliteConnectionService>();
            DataAccess.Init(_connectionService);
            var restService = new RestService();
            _calculationManager = new CalculationManager(DataAccess.GetInstance(), restService);
            CalculationHelper.CreateNewGlobalCalculationAndOneOperation(_calculationManager);
            _mainViewModel = new MainViewModel(_calculationManager, null, null, null);
        }

        public void Dispose()
        {
            DatabaseHelper.CleanupDatabase(_connectionService.GetConnection());
        }


        [Fact(DisplayName = "DeleteGlobalCalculationWithoutCommand")]
        public void DeleteGlobalCalculationWithoutCommand()
        {
            var globalCalculation = _mainViewModel.GlobalCalculations.First();

            //Act with Reflection
            _mainViewModel.GetType().GetRuntimeMethods()
                .First(m => m.Name == "RemoveGlobalCalculation")
                .Invoke(_mainViewModel, new[] { globalCalculation });

            Assert.Empty(_mainViewModel.GlobalCalculations);
        }
    }
}
