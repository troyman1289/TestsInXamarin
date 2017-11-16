using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backend.Interfaces;
using Backend.Model;
using Backend.Model.Operator;

namespace TestsFinal.DroidNative
{
    [Activity(Label = "GlobalCalculationActivity")]
    public class GlobalCalculationActivity : Activity
    {
        private ListView _listView;
        private EditText _operandEditText;
        private Spinner _operationsSpinner;
        private Button _openBracketButton;
        private Button _closeBracketButton;
        private ICalculationManager _calculationManager;

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
                }
            }
        }

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
                }
            }
        }

        #endregion

        #region CurrentLocalCalculation

        private LocalCalculation _currentLocalCalculation;

        public LocalCalculation CurrentLocalCalculation
        {
            get { return _currentLocalCalculation; }
            set
            {
                if (_currentLocalCalculation != value) {
                    _currentLocalCalculation = value;
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
                    RefreshAll();
                }
            }
        }

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.GlobalCalculation);

            _calculationManager = CustomLocator.CalculationManager;

            _listView = FindViewById<ListView>(Resource.Id.LocalCalculationsListView);
            _listView.Adapter = ViewModel.ShoppingListElementCollection.GetAdapter(GetShoppingListElementAdapter);

            _operandEditText = FindViewById<EditText>(Resource.Id.);

            _operationsSpinner = FindViewById<Spinner>(Resource.Id.);

            _openBracketButton = FindViewById<Button>(Resource.Id.);
            _openBracketButton.Click += HandleOpenBracketButtonClick;

            _closeBracketButton = FindViewById<Button>(Resource.Id.);
            _closeBracketButton.Click += HandleCloseBracketButtonClick;

             var addOperationButton = FindViewById<Button>(Resource.Id.);
            addOperationButton.Click += HandleAddOperationButtonClick;

            var addLocalCalculationButton = FindViewById<Button>(Resource.Id.);
            addLocalCalculationButton.Click += HandleAddLocalCalculationClick;
        }

        private void HandleAddLocalCalculationClick(object sender, EventArgs e)
        {
            var newLocalCalculation = new LocalCalculation();
            _calculationManager.AddNewLocalCalculation(GlobalCalculation, newLocalCalculation);
            RefreshAll();
        }

        private void HandleOpenBracketButtonClick(object sender, EventArgs e)
        {
            NewOperation.BracketType = NewOperation.BracketType == BracketType.Open
                ? BracketType.None
                : BracketType.Open;
        }

        private void HandleCloseBracketButtonClick(object sender, EventArgs e)
        {
            NewOperation.BracketType = NewOperation.BracketType == BracketType.Close
                ? BracketType.None
                : BracketType.Close;
        }

        private void HandleAddOperationButtonClick(object sender, EventArgs e)
        {
            NewOperation.OperatorType = SelectedOperator.OperatorType;
            _calculationManager.AddOperation(CurrentLocalCalculation, NewOperation);
            NewOperation = new Operation();

            RefreshLocalCalculation(CurrentLocalCalculation);
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

            _openBracketButton.Enabled = lastOperationWithOpenBracket == null
                                || (lastOperationWithCloseBracket != null
                                    && lastOperationWithOpenBracket.Order < lastOperationWithCloseBracket.Order);
            _closeBracketButton.Enabled = lastOperationWithOpenBracket != null
                                 && (lastOperationWithCloseBracket == null || lastOperationWithCloseBracket.Order > lastOperationWithOpenBracket.Order);
        }

        private void RefreshAll()
        {
            foreach (var localCalculation in GlobalCalculation.LocalCalculations) {
                _calculationManager.SetOperationString(localCalculation);
            }

            CurrentLocalCalculation = GlobalCalculation.LocalCalculations
                .OrderBy(lc => lc.Order)
                .LastOrDefault();

            RefreshCanAddBrackets();
        }
    }
}