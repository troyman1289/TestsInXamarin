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
using NUnit.Framework;
using PCLMock;
using ViewModels;
using Xamarin.Forms;

namespace NUnit.IntegrationTest
{

    public class MainViewModelTest
    {
        private MainViewModel _mainViewModel;
        private ISqliteConnectionForTest _connectionService;
        private ICalculationManager _calculationManager;
        private PopUpForTest _popUpService;
        private PopUpServiceMock _popUpServiceMock;

        [SetUp]
        public void Setup()
        {
            Locator.Init();
            _connectionService = DependencyService.Get<ISqliteConnectionForTest>();
            var dataAccess = new DataAccess(_connectionService);
            var restService = new RestService();
            _calculationManager = new CalculationManager(dataAccess, restService);
            _popUpService = new PopUpForTest();
            _popUpServiceMock = new PopUpServiceMock();
            _mainViewModel = new MainViewModel(_calculationManager, null, _popUpServiceMock.MockedObject, null);
        }

        [TearDown]
        public void Teardown()
        {
            _connectionService.TeardownAndDelete();
        }


        [Test]
        [Ignore("Skip")]
        public void DeleteGlobalCalculationWithFakeImplementation()
        {
            _popUpService.ActionResults = new List<bool>{false,true};
            var globalCalculation = new GlobalCalculation();
            _calculationManager.AddNewGlobalCalculation(globalCalculation,8);
            _mainViewModel.RefreshCalculations();
            _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);
            
            //Assert.Collection(_mainViewModel.GlobalCalculations,gc => gc.Order = 1);
            //TODO ID weg - und fragen, warum nicht true wird --> gc werden neu retrieved
            Assert.IsNotEmpty(_mainViewModel.GlobalCalculations);
            globalCalculation = _mainViewModel.GlobalCalculations.First();
            _mainViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);
            Assert.False(_mainViewModel.GlobalCalculations.Contains(globalCalculation));
            Assert.IsEmpty(_connectionService.GetConnection().Table<GlobalCalculation>());
        }

        [TestCase(false,false)]
        [TestCase(true, true)]
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
                Assert.False(_mainViewModel.GlobalCalculations.Contains(globalCalculation));
                Assert.IsEmpty(_connectionService.GetConnection().Table<GlobalCalculation>());
            } else {
                Assert.IsNotEmpty(_mainViewModel.GlobalCalculations);
            }
        }

        [Test]
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

            Assert.IsEmpty(_mainViewModel.GlobalCalculations);
        }

        [Test]
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

            Assert.IsEmpty(_mainViewModel.GlobalCalculations);
        }
    }
}
