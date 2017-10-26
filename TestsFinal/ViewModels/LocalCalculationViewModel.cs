using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Backend.Interfaces;
using Backend.Model;
using Backend.Model.Operator;
using Backend.Utils;
using GalaSoft.MvvmLight.Command;

namespace ViewModels
{
    public class LocalCalculationViewModel : NotifyingObject
    {
        private readonly ICalculationManager _calculationManager;

        public LocalCalculationViewModel(ICalculationManager calculationManager)
        {
            _calculationManager = calculationManager;
        }

        public List<Operator> Operators
        {
            get { return Backend.Model.Operator.Operators.GetAll(); }
        }

        #region LocalCalculation

        private LocalCalculation _localCalculation;

        public LocalCalculation LocalCalculation
        {
            get { return _localCalculation; }
            set
            {
                if (_localCalculation != value) {
                    _localCalculation = value;
                    OnPropertyChanged();
                    Refresh();
                }
            }
        }

        #endregion

        #region Brackets

        #region CanUseOpenBracket

        private bool _canUseOpenBracket;

        public bool CanUseOpenBracket
        {
            get { return _canUseOpenBracket; }
            set
            {
                if (_canUseOpenBracket != value) {
                    _canUseOpenBracket = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region CanUseCloseBracket

        private bool _canUseCloseBracket;

        public bool CanUseCloseBracket
        {
            get { return _canUseCloseBracket; }
            set
            {
                if (_canUseCloseBracket != value) {
                    _canUseCloseBracket = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region AddBracketCommand

        private ICommand _addBracketCommand;

        public ICommand AddBracketCommand
        {
            get
            {
                if (_addBracketCommand == null) {
                    _addBracketCommand = new RelayCommand<BracketType>(HandleAddBracket);
                }
                return _addBracketCommand;
            }
        }

        private void HandleAddBracket(BracketType bracketType)
        {
            if (bracketType == BracketType.Open) {
                SelectedBracketType = SelectedBracketType == BracketType.Open
                    ? BracketType.None
                    : BracketType.Open;
            } else {
                SelectedBracketType = SelectedBracketType == BracketType.Close
                    ? BracketType.None
                    : BracketType.Close;
            }
        }
 
        #endregion

        #endregion

        #region SelectedBracketType

        private BracketType _selectedBracketType;

        public BracketType SelectedBracketType
        {
            get { return _selectedBracketType; }
            set
            {
                if (_selectedBracketType != value) {
                    _selectedBracketType = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region SelectedOperator

        private Operator _selectedOperator;

        public Operator SelectedOperator
        {
            get { return _selectedOperator; }
            set
            {
                if (_selectedOperator != value) {
                    _selectedOperator = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region NewOperand

        private decimal _newOperand;

        public decimal NewOperand
        {
            get { return _newOperand; }
            set
            {
                if (_newOperand != value) {
                    _newOperand = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region AddOperationCommand

        private ICommand _addOperationCommand;

        public ICommand AddOperationCommand
        {
            get
            {
                if (_addOperationCommand == null) {
                    _addOperationCommand = new RelayCommand(HandleAddOperation);
                }
                return _addOperationCommand;
            }
        }

        private void HandleAddOperation()
        {
            int order = LocalCalculation.Operations.Any()
                ? LocalCalculation.Operations.Max(o => o.Order)
                : 1;

            var newOperation = new Operation
            {
                Operand = NewOperand,
                Operator = SelectedOperator,
                BracketType = SelectedBracketType,
                Order = order
            };

            LocalCalculation.Operations.Add(newOperation);
        }

        #endregion

        #region RemoveLastOperationCommand

        private ICommand _removeLastOperationCommand;

        public ICommand RemoveLastOperationCommand
        {
            get
            {
                if (_removeLastOperationCommand == null) {
                    _removeLastOperationCommand = new RelayCommand(HandleRemoveLastOperation);
                }
                return _removeLastOperationCommand;
            }
        }

        private void HandleRemoveLastOperation()
        {
            var operation = LocalCalculation.Operations.OrderBy(o => o.Order).LastOrDefault();
            if (operation != null) {
                LocalCalculation.Operations.Remove(operation);
            }
        }

        #endregion

        #region OperationString

        private string _operationString;

        public string OperationString
        {
            get { return _operationString; }
            set
            {
                if (_operationString != value) {
                    _operationString = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        private void Refresh()
        {
            if (LocalCalculation == null) {
                return;
            }

            var operationString = string.Empty;
            foreach (var operation in LocalCalculation.Operations) {
                operationString += CreateOperationString(operation);
            }

            OperationString = operationString;

            RefreshCanAddBrackets();
        }

        private string CreateOperationString(Operation operation)
        {
            var operationString = operation.Operator.Label;
            if (operation.BracketType == BracketType.Open) operationString += " (";

            operationString += " " + operation.Operand.ToString("N2");

            if (operation.BracketType == BracketType.Close) operationString += " )";

            return operationString;
        }

        private void RefreshCanAddBrackets()
        {
            var lastOperationWithOpenBracket = LocalCalculation.Operations
                .OrderBy(o => o.Order)
                .LastOrDefault(o => o.BracketType == BracketType.Open);
            var lastOperationWithCloseBracket = LocalCalculation.Operations
                .OrderBy(o => o.Order)
                .LastOrDefault(o => o.BracketType == BracketType.Close);

            CanUseOpenBracket = lastOperationWithOpenBracket == null
                                || (lastOperationWithCloseBracket != null
                                    && lastOperationWithOpenBracket.Order < lastOperationWithCloseBracket.Order);
            CanUseCloseBracket = lastOperationWithOpenBracket != null
                                 && (lastOperationWithCloseBracket == null || lastOperationWithCloseBracket.Order > lastOperationWithOpenBracket.Order);
        }

    }
}
