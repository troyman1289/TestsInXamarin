using System;
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

            //load local calculations
            var localCalculations = _dataAccess.GetLocalCalculations(globalCalculation.Id);
            foreach (var localCalculation in localCalculations) {
                globalCalculation.LocalCalculations.Add(localCalculation);
                var operations = _dataAccess.GetOperations(localCalculation.Id);
                foreach (var operation in operations) {
                    localCalculation.Operations.Add(operation);
                }
            }
        }

        public void RemoveLocalCalculation(GlobalCalculation globalCalculation, LocalCalculation localCalculation, bool withRefresh)
        {
            _dataAccess.Remove(localCalculation.Operations);
            _dataAccess.Remove(localCalculation);
            var orderToRemove = localCalculation.Order;
            globalCalculation.LocalCalculations.Remove(localCalculation);

            if(!withRefresh) return;

            //calculate new from this position
            var toRefresh = globalCalculation.LocalCalculations.Where(lc => lc.Order > orderToRemove).ToList();
            foreach (var calculation in toRefresh) {
                calculation.Order = orderToRemove;
                var startOprand = globalCalculation.LocalCalculations.First(lc => lc.Order == orderToRemove - 1).Result;
                calculation.StartOperand = startOprand;
                SetResult(calculation);
                SetOperationString(localCalculation);
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
                RemoveLocalCalculation(globalCalculation, localCalculation, false);
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
            
            //create first operation
            //var operation = new Operation();
            //operation.BracketType = BracketType.None;
            //operation.Operand = startOperand;
            //operation.OperatorType = OperatorType.Addition;
            //operation.ParentLocalCalculationId = localCalculation.Id;
            //operation.ParentLocalCalculation = localCalculation;
            //operation.Order = 1;
            //localCalculation.Operations.Add(operation);
            //_dataAccess.Insert(operation);
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

            foreach (var operation in localCalculation.Operations.ToList()) {
                AddOperation(localCalculation, operation);
            }
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
            //- multiplication and division first, then addition and subtraction
            //...

            //search for bracketes and calculate them
            var orderedoperations = localCalculation.Operations.OrderBy(o => o.Order).ToList();
            while (orderedoperations.Any(o => o.BracketType == BracketType.Open))
            {
                var openOperation = orderedoperations.First(o => o.BracketType == BracketType.Open);
                var closeOperation = orderedoperations.FirstOrDefault(o => o.Order >= openOperation.Order && o.BracketType == BracketType.Close);
                if (closeOperation == null) {
                    //take the end
                    closeOperation = orderedoperations.Last();
                }

                var toSummarize = orderedoperations
                    .Where(o => o.Order >= openOperation.Order && o.Order <= closeOperation.Order)
                    .ToList();

                var summarizedOperation = Summarize(toSummarize);
                summarizedOperation.BracketType = BracketType.Close;
                orderedoperations = orderedoperations.Except(toSummarize).ToList();
                orderedoperations.Add(summarizedOperation);
                orderedoperations = orderedoperations.OrderBy(o => o.Order).ToList();
            }

            //Operation with start operand
            var startOperation = new Operation {Order = 0, Operand = localCalculation.StartOperand};
            orderedoperations.Add(startOperation);

            var endOperation = Summarize(orderedoperations);
            localCalculation.Result = Math.Round(endOperation.Operand, 2);
            _dataAccess.Update(localCalculation);
        }

        private Operation Summarize(IList<Operation> operations)
        {
            //if(operations.Any())
            var orderedOperations = operations.OrderBy(o => o.Order).ToList();
            var startOrder = orderedOperations.First().Order;
            
            //search for operations
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
