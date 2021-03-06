﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Interfaces;
using Backend.Model;
using Backend.Model.Operator;

namespace Backend.Business
{
    public class CalculationManager : ICalculationManager
    {
        private readonly IDataAccess _dataAccess;
        private readonly IRestService _restService;

        public CalculationManager(
            IDataAccess dataAccess,
            IRestService restService)
        {
            _dataAccess = dataAccess;
            _restService = restService;
        }

        public async Task FetchGlobalCalculationsFromServiceAsync()
        {
            var globalCalculations = await _restService.FetchGlobalCalculations();
            _dataAccess.Insert(globalCalculations);
            foreach (var globalCalculation in globalCalculations) {
                foreach (var localCalculation in globalCalculation.LocalCalculations) {
                    localCalculation.ParentGlobalCalculation = globalCalculation;
                    localCalculation.ParentGlobalCalculationId = globalCalculation.Id;
                    _dataAccess.Insert(localCalculation);
                    foreach (var operation in localCalculation.Operations) {
                        operation.ParentLocalCalculation = localCalculation;
                        operation.ParentLocalCalculationId = localCalculation.Id;
                        _dataAccess.Insert(operation);
                    }
                    SetResult(localCalculation);
                }
                RefreshGlobalResult(globalCalculation);
            }
        }

        public IList<GlobalCalculation> GetAllGlobalCalculations()
        {
            return _dataAccess.GetAllGlobalCalculations();
        }

        public void LoadGlobalCalculation(GlobalCalculation globalCalculation)
        {
            if (globalCalculation.LocalCalculations.Any()) {
                return;
            }

            //load local calculations and remove them with their operations
            var localCalculations = _dataAccess.GetLocalCalculations(globalCalculation.Id);
            foreach (var localCalculation in localCalculations) {
                globalCalculation.LocalCalculations.Add(localCalculation);
                var operations = _dataAccess.GetOperations(localCalculation.Id);
                foreach (var operation in operations) {
                    localCalculation.Operations.Add(operation);
                }
            }
        }

        public void RemoveLocalCalculation(GlobalCalculation globalCalculation, LocalCalculation localCalculation)
        {
            _dataAccess.Remove(localCalculation.Operations);
            _dataAccess.Remove(localCalculation);
            globalCalculation.LocalCalculations.Remove(localCalculation);
        }

        public void RemoveLocalCalculationWithRefresh(GlobalCalculation globalCalculation, LocalCalculation localCalculation)
        {
            var orderToRemove = localCalculation.Order;
            RemoveLocalCalculation(globalCalculation, localCalculation);
            //calculate new from this position and refresh the order
            var toRefresh = globalCalculation.LocalCalculations.Where(lc => lc.Order > orderToRemove).ToList();
            foreach (var calculation in toRefresh) {
                calculation.Order = orderToRemove;
                var startOprand = globalCalculation.LocalCalculations.First(lc => lc.Order == orderToRemove - 1).Result;
                calculation.StartOperand = startOprand;
                SetResult(calculation);
                SetOperationString(calculation);
                orderToRemove++;
            }
            RefreshGlobalResult(globalCalculation);
        }

        public void RefreshGlobalResult(GlobalCalculation globalCalculation)
        {
            globalCalculation.Result = globalCalculation.LocalCalculations.Any()
                ? globalCalculation.LocalCalculations.OrderBy(lc => lc.Order).Last().Result
                : 0;
            _dataAccess.Update(globalCalculation);
        }

        public void RemoveGlobalCalculation(GlobalCalculation globalCalculation)
        {
            var toRemove = globalCalculation.LocalCalculations.ToList();
            foreach (var localCalculation in toRemove) {
                RemoveLocalCalculation(globalCalculation, localCalculation);
            }
            _dataAccess.Remove(globalCalculation);
        }

        public void AddNewGlobalCalculation(GlobalCalculation globalCalculation, decimal startOperand)
        {
            var allGlobalCalculations = GetAllGlobalCalculations().ToList();
            var newOrder = allGlobalCalculations.Any()
                ? allGlobalCalculations.Max(g => g.Order) + 1
                : 1;

            globalCalculation.Order = newOrder;
            _dataAccess.Insert(globalCalculation);

            //create first local calculation
            var localCalculation = new LocalCalculation();
            localCalculation.Order = 1;
            localCalculation.StartOperand = startOperand;
            localCalculation.ParentGlobalCalculationId = globalCalculation.Id;
            localCalculation.ParentGlobalCalculation = globalCalculation;
            globalCalculation.LocalCalculations.Add(localCalculation);
            _dataAccess.Insert(localCalculation);
        }

        public void AddOperation(LocalCalculation localCalculation,Operation operation)
        {
            int order = localCalculation.Operations.Any()
                ? localCalculation.Operations.Max(o => o.Order) + 1
                : 1;

            operation.ParentLocalCalculationId = localCalculation.Id;
            operation.ParentLocalCalculation = localCalculation;
            operation.Order = order;
            localCalculation.Operations.Add(operation);
            _dataAccess.Insert(operation);
        }

        public void AddNewLocalCalculation(GlobalCalculation globalCalculation, LocalCalculation localCalculation)
        {
            int order = globalCalculation.LocalCalculations.Any()
                ? globalCalculation.LocalCalculations.Max(o => o.Order) + 1
                : 1;

            localCalculation.ParentGlobalCalculationId = globalCalculation.Id;
            localCalculation.ParentGlobalCalculation = globalCalculation;
            localCalculation.Order = order;

            if (globalCalculation.LocalCalculations.Any()) {
                localCalculation.StartOperand = globalCalculation.LocalCalculations.Last().Result;
            }
            globalCalculation.LocalCalculations.Add(localCalculation);
            _dataAccess.Insert(localCalculation);
        }

        public void SetOperationString(LocalCalculation localCalculation)
        {
            var operationString = localCalculation.StartOperand.ToString("N2") + " ";
            var hasOpenBracket = false;
            foreach (var operation in localCalculation.Operations)
            {
                operationString += operation.Operator.Label;

                if (operation.BracketType == BracketType.Open) { 
                    operationString += " (";
                    hasOpenBracket = true;
                }

                operationString += " " + operation.Operand.ToString("N2");

                if (operation.BracketType == BracketType.Close) { 
                    operationString += " )";
                    hasOpenBracket = false;
                }

                operationString += " ";
            }

            if (hasOpenBracket) {
                operationString += ")";
            }

            localCalculation.OperationString = operationString;
        }

        public void SetResult(LocalCalculation localCalculation)
        {
            //Rules:
            //- from left to right
            //- brackets first
            //- multiplication and division first, then addition and subtraction = Weight of operators
            //...

            try {
                //search for bracketes and calculate them at first
                var orderedOperations = localCalculation.Operations.OrderBy(o => o.Order).ToList();
                while (orderedOperations.Any(o => o.BracketType == BracketType.Open)) {
                    var openOperation = orderedOperations.First(o => o.BracketType == BracketType.Open);
                    var closeOperation = orderedOperations.FirstOrDefault(o =>
                       o.Order >= openOperation.Order && o.BracketType == BracketType.Close);

                    if (closeOperation == null) {
                        //take the end
                        closeOperation = orderedOperations.Last();
                    }

                    var toSummarize = orderedOperations
                        .Where(o => o.Order >= openOperation.Order && o.Order <= closeOperation.Order)
                        .ToList();

                    var summarizedOperation = Summarize(toSummarize);
                    summarizedOperation.BracketType = BracketType.None;
                    orderedOperations = orderedOperations.Except(toSummarize).ToList();
                    orderedOperations.Add(summarizedOperation);
                    orderedOperations = orderedOperations.OrderBy(o => o.Order).ToList();
                }

                //Operation with start operand
                var startOperation = new Operation { Order = 0, Operand = localCalculation.StartOperand };
                orderedOperations.Add(startOperation);

                //Calculate all operations and summarize them
                var endOperation = Summarize(orderedOperations);
                localCalculation.Result = Math.Round(endOperation.Operand, 2);
                _dataAccess.Update(localCalculation);
            } catch (DivideByZeroException) {
                var lastOperation = localCalculation.Operations.OrderBy(o => o.Order).Last();
                _dataAccess.Remove(lastOperation);
                localCalculation.Operations.Remove(lastOperation);
                SetOperationString(localCalculation);
                SetResult(localCalculation);
            }
        }

        private Operation Summarize(IList<Operation> operations)
        {
            var orderedOperations = operations.OrderBy(o => o.Order).ToList();
            var startOrder = orderedOperations.First().Order;
            
            //search for remaining operations
            while (orderedOperations.Count >= 2)
            {
                var operationsToCheck = orderedOperations
                    .Where(o => o.Order > startOrder)
                    .ToList();
                var maxWeight = operationsToCheck
                    .Max(o => o.Operator.Weight);
                var operationsWithMaxWeight = operationsToCheck
                    .Where(o => o.Operator.Weight == maxWeight)
                    .ToList();

                foreach (var operation in operationsWithMaxWeight) {
                    var operation1 = orderedOperations.Last(o => o.Order < operation.Order);
                    var summarizedOperation = Summarize(operation1, operation);
                    orderedOperations.Remove(operation1);
                    orderedOperations.Remove(operation);
                    orderedOperations.Add(summarizedOperation);
                    orderedOperations = orderedOperations.OrderBy(o => o.Order).ToList();
                }

                orderedOperations = orderedOperations.OrderBy(o => o.Order).ToList();
            }

            var toCopy = orderedOperations.First();
            return new Operation
            {
                BracketType = toCopy.BracketType,
                Operand = toCopy.Operand,
                OperatorType = toCopy.OperatorType,
                Order = toCopy.Order
            };
        }

        /// <summary>
        /// Here we perform the real calculation
        /// </summary>
        /// <param name="operation1"></param>
        /// <param name="operation2"></param>
        /// <returns></returns>
        private Operation Summarize(Operation operation1, Operation operation2)
        {
            var summarizedOperation = new Operation();
            summarizedOperation.OperatorType = operation1.OperatorType;
            summarizedOperation.Operand = operation2.Operator.Calculate(operation1.Operand, operation2.Operand);
            summarizedOperation.Order = operation1.Order;
            return summarizedOperation;
        }

    }
}
