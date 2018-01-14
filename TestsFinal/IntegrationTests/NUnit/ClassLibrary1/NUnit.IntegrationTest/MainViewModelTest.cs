using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.Interfaces.Mocks;
using Backend.Model;
using Backend.RestService;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using PCLMock;
using ViewModels;
using Xamarin.Forms;

namespace NUnit.IntegrationTest
{

    public class MainViewModelTest
    {
        private PopUpForTest _popUpService;
        private MainViewModel _mainViewModel;
        private CalculationManager _calculationManager;
        private ISqliteConnectionService _connectionService;
        private PopUpServiceMock _popUpServiceMock;

        [SetUp]
        public void Setup()
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

        [TearDown]
        public void Teardown()
        {
            DatabaseHelper.CleanupDatabase(_connectionService.GetConnection());
        }

        [TestCase(false,Ignore = "skip")]
        [TestCase(true, Ignore = "skip")]
        public void RemoveGlobalCalculation(bool popUpResultValue)
        {
            //Arange
            _popUpService.ActionResultValue = popUpResultValue;
            var globalCalculation = _mainViewModel.GlobalCalculations.First();

            //Act
            _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);

            if (popUpResultValue) {
                Assert.False(_mainViewModel.GlobalCalculations.Contains(globalCalculation));
                Assert.IsEmpty(_connectionService.GetConnection().Table<GlobalCalculation>());
                Assert.IsEmpty(_connectionService.GetConnection().Table<LocalCalculation>());
                Assert.IsEmpty(_connectionService.GetConnection().Table<Operation>());
            } else {
                Assert.IsNotEmpty(_mainViewModel.GlobalCalculations);
                Assert.IsEmpty(_connectionService.GetConnection().Table<GlobalCalculation>());
                Assert.Contains(globalCalculation, _mainViewModel.GlobalCalculations);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
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


            if (popUpResultValue) {
                Assert.IsEmpty(_connectionService.GetConnection().Table<GlobalCalculation>());
                Assert.IsEmpty(_connectionService.GetConnection().Table<LocalCalculation>());
                Assert.IsEmpty(_connectionService.GetConnection().Table<Operation>());
            } else {
                Assert.IsNotEmpty(_mainViewModel.GlobalCalculations);
                Assert.IsNotEmpty(_connectionService.GetConnection().Table<GlobalCalculation>());
                Assert.Contains(globalCalculation, _mainViewModel.GlobalCalculations);
            }

            var v = _popUpServiceMock.Verify(
                e => e.ShowOkCancelPopUp("Remove", It.IsLike("Do you want"), It.IsAny<Action<bool>>()));
            v.WasCalledExactly(1);
        }

        [Test]
        public void DeleteGlobalCalculationWithoutCommand()
        {
            var globalCalculation = _mainViewModel.GlobalCalculations.First();

            //Act with Reflection
            _mainViewModel.GetType().GetRuntimeMethods()
                .First(m => m.Name == "RemoveGlobalCalculation")
                .Invoke(_mainViewModel, new[] { globalCalculation });

            Assert.IsEmpty(_mainViewModel.GlobalCalculations);
        }

        [Test]
        public void DeleteGlobalCalculationWithLocator()
        {
            var globalCalculation = _mainViewModel.GlobalCalculations.First();

            //Act with Reflection
            _mainViewModel.GetType().GetRuntimeMethods()
                .First(m => m.Name == "RemoveGlobalCalculationWithLocator")
                .Invoke(_mainViewModel, new[] { globalCalculation });

            Assert.IsEmpty(_mainViewModel.GlobalCalculations);
        }
    }
}
