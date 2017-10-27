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
        private LocalCalculation _currentLocalCalculation = new LocalCalculation();

        public LocalCalculationViewModel(ICalculationManager calculationManager)
        {
            _calculationManager = calculationManager;
        }

        public List<Operator> Operators
        {
            get { return Backend.Model.Operator.Operators.GetAll(); }
        }

        #region LocalCalculation

        private GlobalCalculation _globalCalculation;

        public GlobalCalculation GlobalCalculation
        {
            get { return _globalCalculation; }
            set
            {
                if (_globalCalculation != value) {
                    _globalCalculation = value;
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

        private decimal _newOperand = 0;

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
            int order = _currentLocalCalculation.Operations.Any()
                ? _currentLocalCalculation.Operations.Max(o => o.Order)
                : 1;

            var newOperation = new Operation
            {
                Operand = NewOperand,
                Operator = SelectedOperator,
                BracketType = SelectedBracketType,
                Order = order
            };

            _currentLocalCalculation.Operations.Add(newOperation);
            NewOperand = 0;
        }

        #endregion

        #region AddLocalCalculationCommand

        private ICommand _addLocalCalculationCommand;

        public ICommand AddLocalCalculationCommand
        {
            get
            {
                if (_addLocalCalculationCommand == null) {
                    _addLocalCalculationCommand = new RelayCommand(HandleAddLocalCalculation);
                }
                return _addLocalCalculationCommand;
            }
        }

        private void HandleAddLocalCalculation()
        {
            
        }

        #endregion

        #region RemoveLocalCalculationCommand

        private ICommand _removeLocalCalculationCommand;

        public ICommand RemoveLocalCalculationCommand
        {
            get
            {
                if (_removeLocalCalculationCommand == null) {
                    _removeLocalCalculationCommand = new RelayCommand<LocalCalculation>(HandleRemoveLocalCalculation);
                }
                return _removeLocalCalculationCommand;
            }
        }

        private void HandleRemoveLocalCalculation(LocalCalculation localCalculation)
        {

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
            if (_currentLocalCalculation == null) {
                return;
            }

            var operationString = string.Empty;
            foreach (var operation in _currentLocalCalculation.Operations) {
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
            var lastOperationWithOpenBracket = _currentLocalCalculation.Operations
                .OrderBy(o => o.Order)
                .LastOrDefault(o => o.BracketType == BracketType.Open);
            var lastOperationWithCloseBracket = _currentLocalCalculation.Operations
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
