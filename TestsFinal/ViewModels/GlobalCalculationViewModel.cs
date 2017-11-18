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
    public class GlobalCalculationViewModel : NotifyingObject
    {
        private readonly ICalculationManager _calculationManager;

        public GlobalCalculationViewModel(ICalculationManager calculationManager)
        {
            _calculationManager = calculationManager;
        }

        public List<Operator> Operators
        {
            get { return Backend.Model.Operator.Operators.GetAll(); }
        }

        #region CurrentLocalCalculation

        private LocalCalculation _currentLocalCalculation;

        public LocalCalculation CurrentLocalCalculation
        {
            get { return _currentLocalCalculation; }
            set
            {
                if (_currentLocalCalculation != value) {
                    _currentLocalCalculation = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region GlobalCalculation

        private GlobalCalculation _globalCalculation;

        public GlobalCalculation GlobalCalculation
        {
            get { return _globalCalculation; }
            set
            {
                if (_globalCalculation != value) {
                    _globalCalculation = value;
                    OnPropertyChanged();
                    RefreshAll();
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
                NewOperation.BracketType = NewOperation.BracketType == BracketType.Open
                    ? BracketType.None
                    : BracketType.Open;
            } else {
                NewOperation.BracketType = NewOperation.BracketType == BracketType.Close
                    ? BracketType.None
                    : BracketType.Close;
            }
        }

        #endregion

        #endregion

        #region SelectedOperator

        private Operator _selectedOperator = Backend.Model.Operator.Operators.Addition;

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

        #region NewOperation

        private Operation _newOperation = new Operation();

        public Operation NewOperation
        {
            get { return _newOperation; }
            set
            {
                if (_newOperation != value) {
                    _newOperation = value;
                    _newOperation.OperatorType = OperatorType.Addition;
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
            NewOperation.OperatorType = SelectedOperator.OperatorType;
            _calculationManager.AddOperation(CurrentLocalCalculation, NewOperation);
            NewOperation = new Operation();
            NewOperation.Operand = 1;

            RefreshLocalCalculation(CurrentLocalCalculation);     
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
            var newLocalCalculation = new LocalCalculation();
            _calculationManager.AddNewLocalCalculation(GlobalCalculation, newLocalCalculation);
            RefreshAll();
        }

        #endregion

        #region RemoveLocalCalculationCommand

        private ICommand _removeLocalCalculationCommand;

        public ICommand RemoveLocalCalculationCommand
        {
            get
            {
                if (_removeLocalCalculationCommand == null) {
                    _removeLocalCalculationCommand = new RelayCommand<LocalCalculation>(HandleRemoveLocalCalculation,lc => lc != null && lc.Order != 1);
                }
                return _removeLocalCalculationCommand;
            }
        }

        private void HandleRemoveLocalCalculation(LocalCalculation localCalculation)
        {
            _calculationManager.RemoveLocalCalculation(GlobalCalculation,localCalculation, true);
            CurrentLocalCalculation = GlobalCalculation.LocalCalculations.OrderBy(lc => lc.Order).Last();
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

        private void RefreshAll()
        {
            foreach (var localCalculation in GlobalCalculation.LocalCalculations)
            {
                _calculationManager.SetOperationString(localCalculation);
            }

            CurrentLocalCalculation = GlobalCalculation.LocalCalculations
                .OrderBy(lc => lc.Order)
                .LastOrDefault();

            RefreshCanAddBrackets();
        }

        private void RefreshLocalCalculation(LocalCalculation localCalculation)
        {
            if (CurrentLocalCalculation == null) {
                return;
            }
            
            _calculationManager.SetResult(localCalculation);
            _calculationManager.SetOperationString(localCalculation);
            _calculationManager.RefreshGlobalResult(GlobalCalculation);

            RefreshCanAddBrackets();
        }

        private void RefreshCanAddBrackets()
        {
            var lastOperationWithOpenBracket = CurrentLocalCalculation.Operations
                .OrderBy(o => o.Order)
                .LastOrDefault(o => o.BracketType == BracketType.Open);
            var lastOperationWithCloseBracket = CurrentLocalCalculation.Operations
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
