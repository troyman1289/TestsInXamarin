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
using Moq;
using ViewModels;
using Xamarin.Forms;
using Xunit;

namespace xUnit.IntegrationTest
{

    public class MainViewModelTest : IDisposable
    {
        private readonly MainViewModel _mainViewModel;
        private readonly ISqliteConnectionForTest _connectionService;
        private readonly ICalculationManager _calculationManager;
        private Mock<IPopUpService> _popUpServiceMock;

        public MainViewModelTest()
        {
            _connectionService = DependencyService.Get<ISqliteConnectionForTest>();
            var dataAccess = new DataAccess(_connectionService);
            var restService = new RestService();
            _calculationManager = new CalculationManager(dataAccess, restService);

            _popUpServiceMock = new Mock<IPopUpService>();
            var obj = _popUpServiceMock.Object;
            _mainViewModel = new MainViewModel(_calculationManager, null, obj, null);
        }

        public void Dispose()
        {
            _connectionService.TeardownAndDelete();
        }


        //[Fact(DisplayName = "DeleteGlobalCalculation")]
        //public void DeleteGlobalCalculation()
        //{
        //    _popUpService.ActionResults = new List<bool>{false,true};
        //    var globalCalculation = new GlobalCalculation();
        //    _calculationManager.AddNewGlobalCalculation(globalCalculation,8);
        //    _mainViewModel.RefreshCalculations();
        //    _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);
            
        //    //Assert.Collection(_mainViewModel.GlobalCalculations,gc => gc.Order = 1);
        //    //TODO ID weg - und fragen, warum nicht true wird --> gc werden neu retrieved
        //    Assert.NotEmpty(_mainViewModel.GlobalCalculations);
        //    globalCalculation = _mainViewModel.GlobalCalculations.First();
        //    _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);
        //    Assert.DoesNotContain(_mainViewModel.GlobalCalculations, calculation => calculation == globalCalculation);
        //    Assert.Empty(_connectionService.GetConnection().Table<GlobalCalculation>());
        //}

        [Fact(DisplayName = "DeleteGlobalCalculationWithMock")]
        public void DeleteGlobalCalculationWithMock()
        {

            var globalCalculation = new GlobalCalculation();
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 8);
            _mainViewModel.RefreshCalculations();
            globalCalculation = _mainViewModel.GlobalCalculations.First();

            _popUpServiceMock.Setup(service => service.ShowOkCancelPopUp(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Action<bool>>()))
            .Callback<string, string, Action<bool>>((s, s1, arg3) => arg3.Invoke(false));

            _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);

            _popUpServiceMock.Setup(service => service.ShowOkCancelPopUp(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Action<bool>>()))
            .Callback<string, string, Action<bool>>((s, s1, arg3) => arg3.Invoke(true));

            //setup.Do<string, string, Action<bool>>((s, s1, arg3) => arg3.Invoke(false));
            //Assert.Collection(_mainViewModel.GlobalCalculations,gc => gc.Order = 1);
            //TODO ID weg - und fragen, warum nicht true wird --> gc werden neu retrieved
            Assert.NotEmpty(_mainViewModel.GlobalCalculations);

            _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);
            Assert.DoesNotContain(_mainViewModel.GlobalCalculations, calculation => calculation == globalCalculation);
            Assert.Empty(_connectionService.GetConnection().Table<GlobalCalculation>());
        }

        [Fact(DisplayName = "DeleteGlobalCalculationWithoutCommand")]
        public void DeleteGlobalCalculationWithoutCommand()
        {
            var globalCalculation = new GlobalCalculation();
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 8);
            _mainViewModel.RefreshCalculations();

            Assert.NotEmpty(_mainViewModel.GlobalCalculations);
            globalCalculation = _mainViewModel.GlobalCalculations.First();

            //Reflection
            _mainViewModel.GetType().GetRuntimeMethods()
                .First(m => m.Name == "RemoveGlobalCalculation")
                .Invoke(_mainViewModel, new[] {globalCalculation});

            Assert.Empty(_mainViewModel.GlobalCalculations);
        }
    }
}
