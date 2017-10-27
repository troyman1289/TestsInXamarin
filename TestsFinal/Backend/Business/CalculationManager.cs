﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Interfaces;
using Backend.Model;

namespace Backend.Business
{
    public class CalculationManager : ICalculationManager
    {
        private readonly IDataAccess _dataAccess;

        public CalculationManager(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IList<GlobalCalculation> GetAllGlobalCalculations()
        {
            return _dataAccess.GetAllGlobalCalculations();
        }

        public void RemoveLocalCalculation(GlobalCalculation globalCalculation, LocalCalculation localCalculation)
        {
            _dataAccess.Remove(localCalculation.Operations);
            _dataAccess.Remove(localCalculation);
            globalCalculation.LocalCalculations.Remove(localCalculation);
        }

        public void AddNewGlobalCalculation(GlobalCalculation globalCalculation)
        {
            var allGlobalCalculations = GetAllGlobalCalculations().ToList();
            var newOrder = allGlobalCalculations.Any()
                ? allGlobalCalculations.Max(g => g.Order) + 1
                : 1;

            globalCalculation.Order = newOrder;
            _dataAccess.Insert(globalCalculation);
        }
    }
}
