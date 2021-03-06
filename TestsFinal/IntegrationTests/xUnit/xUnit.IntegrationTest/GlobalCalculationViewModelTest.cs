﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Business;
using Backend.DataAccess;
using Backend.Interfaces;
using Backend.Model;
using Backend.Model.Operator;
using Backend.RestService;
using ViewModels;
using Xamarin.Forms;
using Xunit;

namespace xUnit.IntegrationTest
{
    public class GlobalCalculationViewModelTest : IDisposable
    {
        private readonly GlobalCalculationViewModel _globalCalculationViewModel;
        private CalculationManager _calculationManager;
        private ISqliteConnectionService _connectionService;

        public GlobalCalculationViewModelTest()
        {
            _connectionService = DependencyService.Get<ISqliteConnectionService>();
            DataAccess.Init(_connectionService);
            var dataAccess = DataAccess.GetInstance();
            var restService = new RestService();
            _calculationManager = new CalculationManager(dataAccess, restService);
            _globalCalculationViewModel = new GlobalCalculationViewModel(_calculationManager);
            _globalCalculationViewModel.GlobalCalculation =
                CalculationHelper.CreateNewGlobalCalculationAndOneOperation(_calculationManager);
        }

        public void Dispose()
        {
            DatabaseHelper.CleanupDatabase(_connectionService.GetConnection());
        }

        [Fact(DisplayName = "CanAddBracketTest1")]
        public void CanAddBracketTest1()
        {
            //8+2
            Assert.True(_globalCalculationViewModel.CanUseOpenBracket);
            Assert.False(_globalCalculationViewModel.CanUseCloseBracket);
        }

        [Fact(DisplayName = "CanAddBracketTest2")]
        public void CanAddBracketTest2()
        {
            //8+2+(4...
            var localCalculation =
                _globalCalculationViewModel.GlobalCalculation.LocalCalculations.First();
            _globalCalculationViewModel.NewOperation.BracketType = BracketType.Open;
            _globalCalculationViewModel.NewOperation.Operand = 4;
            _globalCalculationViewModel.AddOperationCommand.Execute(null);

            Assert.False(_globalCalculationViewModel.CanUseOpenBracket);
            Assert.True(_globalCalculationViewModel.CanUseCloseBracket);
        }

        [Fact(DisplayName = "CanAddBracketTest3")]
        public void CanAddBracketTest3()
        {
            var localCalculation =
                _globalCalculationViewModel.GlobalCalculation.LocalCalculations.First();

            //8+2+(4+3)
            _globalCalculationViewModel.NewOperation.BracketType = BracketType.Open;
            _globalCalculationViewModel.NewOperation.Operand = 4;
            _globalCalculationViewModel.AddOperationCommand.Execute(null);
            _globalCalculationViewModel.NewOperation.BracketType = BracketType.Close;
            _globalCalculationViewModel.NewOperation.Operand = 3;
            _globalCalculationViewModel.AddOperationCommand.Execute(null);

            Assert.True(_globalCalculationViewModel.CanUseOpenBracket);
            Assert.False(_globalCalculationViewModel.CanUseCloseBracket);
        }
    }
}
