using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.Interfaces.Mocks;
using Backend.Model;
using Backend.RestService;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using PCLMock;
using ViewModels;
using Xamarin.Forms;
using Xunit;

namespace xUnit.IntegrationTest
{

    public class MainViewModelTest : IDisposable
    {
        private PopUpForTest _popUpService;
        private readonly MainViewModel _mainViewModel;
        private CalculationManager _calculationManager;
        private ISqliteConnectionService _connectionService;
        private PopUpServiceMock _popUpServiceMock;

        public MainViewModelTest()
        {
            if (!ServiceLocator.IsLocationProviderSet) {
                ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
                var connectionService = DependencyService.Get<ISqliteConnectionService>();
                DataAccess.Init(connectionService);
                SimpleIoc.Default.Register<IDataAccess>(() => DataAccess.GetInstance());
                SimpleIoc.Default.Register<ISqliteConnectionService>(() => connectionService);
                SimpleIoc.Default.Register<IRestService, RestService>();
                SimpleIoc.Default.Register<ICalculationManager, CalculationManager>();
            }

            _connectionService = DependencyService.Get<ISqliteConnectionService>();
            DataAccess.Init(_connectionService);
            var dataAccess = DataAccess.GetInstance();
            var restService = new RestService();
            _calculationManager = new CalculationManager(dataAccess, restService);
            CalculationHelper.CreateNewGlobalCalculationAndOneOperation(_calculationManager);
            //_popUpService = new PopUpForTest();
            //_mainViewModel = new MainViewModel(_calculationManager, null, _popUpService, null);
            _popUpServiceMock = new PopUpServiceMock();
            _mainViewModel = new MainViewModel(_calculationManager, null, _popUpServiceMock.MockedObject, null);
            _mainViewModel.RefreshCalculations();
        }

        public void Dispose()
        {
            DatabaseHelper.CleanupDatabase(_connectionService.GetConnection());
        }


        [Theory(DisplayName = "RemoveGlobalCalculation", Skip = "no reason")]
        [InlineData(false)]
        [InlineData(true)]
        public void RemoveGlobalCalculation(bool popUpResultValue)
        {
            //Arange
            _popUpService.ActionResultValue = popUpResultValue;
            var globalCalculation = _mainViewModel.GlobalCalculations.First();

            //Act
            _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);

            //Assert
            if (popUpResultValue) {
                Assert.DoesNotContain(globalCalculation, _mainViewModel.GlobalCalculations);
                Assert.Empty(_connectionService.GetConnection().Table<GlobalCalculation>());
                Assert.Empty(_connectionService.GetConnection().Table<LocalCalculation>());
                Assert.Empty(_connectionService.GetConnection().Table<Operation>());
            } else {
                Assert.NotEmpty(_mainViewModel.GlobalCalculations);
                Assert.NotEmpty(_connectionService.GetConnection().Table<GlobalCalculation>());
                Assert.Contains(globalCalculation, _mainViewModel.GlobalCalculations);
            }

        }

        [Theory(DisplayName = "RemoveGlobalCalculationWithMock")]
        [InlineData(false)]
        [InlineData(true)]
        public void RemoveGlobalCalculationWithMock(bool popUpResultValue)
        {
            //Arange
            var globalCalculation = _mainViewModel.GlobalCalculations.First();
            _popUpServiceMock.When(service => service.ShowOkCancelPopUp(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Action<bool>>()))
            .Do<string, string, Action<bool>>((s, s1, arg3) => arg3.Invoke(popUpResultValue));

            //Act
            _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);

            //Assert
            if (popUpResultValue) {
                Assert.DoesNotContain(globalCalculation, _mainViewModel.GlobalCalculations);
                Assert.Empty(_connectionService.GetConnection().Table<GlobalCalculation>());
                Assert.Empty(_connectionService.GetConnection().Table<LocalCalculation>());
                Assert.Empty(_connectionService.GetConnection().Table<Operation>());
            } else {
                Assert.NotEmpty(_mainViewModel.GlobalCalculations);
                Assert.NotEmpty(_connectionService.GetConnection().Table<GlobalCalculation>());
                Assert.Contains(globalCalculation, _mainViewModel.GlobalCalculations);
            }

            var v = _popUpServiceMock.Verify(
                e => e.ShowOkCancelPopUp("Remove", It.IsLike("Do you want"), It.IsAny<Action<bool>>()));
            v.WasCalledExactly(1);
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

        [Fact(DisplayName = "RemoveGlobalCalculationWithLocatorTest")]
        public void RemoveGlobalCalculationWithLocatorTest()
        {
            var globalCalculation = _mainViewModel.GlobalCalculations.First();

            //Act with Reflection
            _mainViewModel.GetType().GetRuntimeMethods()
                .First(m => m.Name == "RemoveGlobalCalculationWithLocator")
                .Invoke(_mainViewModel, new[] { globalCalculation });

            Assert.Empty(_mainViewModel.GlobalCalculations);
        }
    }
}
