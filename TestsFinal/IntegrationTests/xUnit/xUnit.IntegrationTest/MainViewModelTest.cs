﻿using System;
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
using PCLMock;
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
        private readonly PopUpForTest _popUpService;
        private readonly PopUpServiceMock _popUpServiceMock;

        public MainViewModelTest()
        {
            Locator.Init();
            _connectionService = DependencyService.Get<ISqliteConnectionForTest>();
            var dataAccess = new DataAccess(_connectionService);
            var restService = new RestService();
            _calculationManager = new CalculationManager(dataAccess, restService);
            _popUpService = new PopUpForTest();
            _popUpServiceMock = new PopUpServiceMock();
            //_mainViewModel = new MainViewModel(_calculationManager, null, _popUpServiceMock.MockedObject, null);
            _mainViewModel = new MainViewModel(_calculationManager, null, _popUpService, null);
        }

        public void Dispose()
        {
            _connectionService.TeardownAndDelete();
        }


        [Theory(DisplayName = "DeleteGlobalCalculationWithFake")]
        [InlineData(false)]
        [InlineData(true)]
        public void RemoveGlobalCalculationWithFakeImplementation(bool popUpResultValue)
        {
            _popUpService.ActionResultValue = popUpResultValue;
            var globalCalculation = new GlobalCalculation();
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 8);
            _mainViewModel.RefreshCalculations();
            globalCalculation = _mainViewModel.GlobalCalculations.First();

            _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);

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

        [Theory(DisplayName = "DeleteGlobalCalculationWithMock")]
        [InlineData(false,false)]
        [InlineData(true, true)]
        public void DeleteGlobalCalculationWithMock(bool popSuccess, bool shouldbeDeleted)
        {

            var globalCalculation = new GlobalCalculation();
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 8);
            _mainViewModel.RefreshCalculations();
            globalCalculation = _mainViewModel.GlobalCalculations.First();

            _popUpServiceMock.When(service => service.ShowOkCancelPopUp(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Action<bool>>()))
            .Do<string, string, Action<bool>>((s, s1, arg3) => arg3.Invoke(popSuccess));

            _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);

            if (shouldbeDeleted) {
                Assert.DoesNotContain(_mainViewModel.GlobalCalculations, calculation => calculation == globalCalculation);
                Assert.Empty(_connectionService.GetConnection().Table<GlobalCalculation>());
            } else {
                Assert.NotEmpty(_mainViewModel.GlobalCalculations);
            }
        }

        [Fact(DisplayName = "DeleteGlobalCalculationWithoutCommand")]
        public void DeleteGlobalCalculationWithoutCommand()
        {
            var globalCalculation = new GlobalCalculation();
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 8);
            _mainViewModel.RefreshCalculations();
            globalCalculation = _mainViewModel.GlobalCalculations.First();

            //Reflection
            _mainViewModel.GetType().GetRuntimeMethods()
                .First(m => m.Name == "RemoveGlobalCalculation")
                .Invoke(_mainViewModel, new[] {globalCalculation});

            Assert.Empty(_mainViewModel.GlobalCalculations);
        }

        [Fact(DisplayName = "DeleteGlobalCalculationWithLocator")]
        public void DeleteGlobalCalculationWithLocator()
        {
            var globalCalculation = new GlobalCalculation();
            _calculationManager.AddNewGlobalCalculation(globalCalculation, 8);
            _mainViewModel.RefreshCalculations();
            globalCalculation = _mainViewModel.GlobalCalculations.First();

            //Reflection
            _mainViewModel.GetType().GetRuntimeMethods()
                .First(m => m.Name == "RemoveGlobalCalculationWithLocator")
                .Invoke(_mainViewModel, new[] { globalCalculation });

            Assert.Empty(_mainViewModel.GlobalCalculations);
        }
    }
}
