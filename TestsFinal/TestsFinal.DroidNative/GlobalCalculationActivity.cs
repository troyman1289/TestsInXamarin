using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [Activity(Label = "GlobalCalculationActivity", Theme = "@style/MainTheme")]
    public class GlobalCalculationActivity : Activity
    {
        private ListView _listView;
        private EditText _operandEditText;
        private Spinner _operatorSpinner;
        private Button _openBracketButton;
        private Button _closeBracketButton;
        private ICalculationManager _calculationManager;
        private LocalCalculation _currentLocalCalculation;
        private GlobalCalculation _globalCalculation;
        private Operator _selectedOperator = Backend.Model.Operator.Operators.Addition;

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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.GlobalCalculation);
            _calculationManager = CustomLocator.CalculationManager;


            //Set Global Calculation
            var globalCalculationId = Intent.GetIntExtra("GlobalCalculationId", -1);
            if (globalCalculationId != -1) {
                var globalCalculation = _calculationManager.GetAllGlobalCalculations().First(gc => gc.Id == globalCalculationId);
                _calculationManager.LoadGlobalCalculation(globalCalculation);
                _globalCalculation = globalCalculation;
                _currentLocalCalculation = _globalCalculation.LocalCalculations.OrderBy(lc => lc.Order).Last();
                _calculationManager.SetOperationString(_currentLocalCalculation);
            }

            _listView = FindViewById<ListView>(Resource.Id.LocalCalculationsListView);
            _listView.Adapter = new LocalCalculationListViewAdapter(_globalCalculation.LocalCalculations, this);
            RegisterForContextMenu(_listView);

            _operandEditText = FindViewById<EditText>(Resource.Id.OperandEditText);

            _operatorSpinner = FindViewById<Spinner>(Resource.Id.OperatorSpinner);
            ArrayAdapter<String> operationsAdapter = new ArrayAdapter<String>(this,
                Android.Resource.Layout.SimpleSpinnerItem, Operators.GetAll().Select(o => o.Label).ToList());
            _operatorSpinner.Adapter = operationsAdapter;
            _operatorSpinner.ItemSelected += HandleOperationSpinnerItemSelected;
     

            _openBracketButton = FindViewById<Button>(Resource.Id.AddOpenBracketButton);
            _openBracketButton.Click += HandleOpenBracketButtonClick;

            _closeBracketButton = FindViewById<Button>(Resource.Id.AddCloseBracketButton);
            _closeBracketButton.Click += HandleCloseBracketButtonClick;

             var addOperationButton = FindViewById<Button>(Resource.Id.AddoperationButton);
            addOperationButton.Click += HandleAddOperationButtonClick;

            var addLocalCalculationButton = FindViewById<Button>(Resource.Id.AddLocalCalculationButton);
            addLocalCalculationButton.Click += HandleAddLocalCalculationClick;
        }

        private void HandleOperationSpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _selectedOperator = Operators.GetAll()[e.Position];
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            MenuInflater.Inflate(Resource.Menu.DeleteContextMenu, menu);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var menuInfo = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;

            switch (item.ItemId) {
                case Resource.Id.DeleteItem:
                    var localCalculation = _globalCalculation.LocalCalculations[menuInfo.Position];
                    _calculationManager.RemoveLocalCalculationWithRefresh(_globalCalculation, localCalculation);
                    RefreshAll();
                    return true;
            }

            return base.OnContextItemSelected(item);
        }

        private void HandleAddLocalCalculationClick(object sender, EventArgs e)
        {
            var newLocalCalculation = new LocalCalculation();
            _calculationManager.AddNewLocalCalculation(_globalCalculation, newLocalCalculation);
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
            NewOperation.OperatorType = _selectedOperator.OperatorType;
            NewOperation.Operand = decimal.Parse(_operandEditText.Text);
            _calculationManager.AddOperation(_currentLocalCalculation, NewOperation);
            NewOperation = new Operation();

            RefreshLocalCalculation(_currentLocalCalculation);
            _calculationManager.RefreshGlobalResult(_globalCalculation);
        }

        private void RefreshLocalCalculation(LocalCalculation localCalculation)
        {
            if (_currentLocalCalculation == null) {
                return;
            }

            _calculationManager.SetResult(localCalculation);
            _calculationManager.SetOperationString(localCalculation);
            _calculationManager.RefreshGlobalResult(_globalCalculation);

            ((LocalCalculationListViewAdapter)_listView.Adapter).NotifyDataSetChanged();

            RefreshCanAddBrackets();
        }

        private void RefreshCanAddBrackets()
        {
            var lastOperationWithOpenBracket = _currentLocalCalculation.Operations
                .OrderBy(o => o.Order)
                .LastOrDefault(o => o.BracketType == BracketType.Open);
            var lastOperationWithCloseBracket = _currentLocalCalculation.Operations
                .OrderBy(o => o.Order)
                .LastOrDefault(o => o.BracketType == BracketType.Close);

            _openBracketButton.Enabled = lastOperationWithOpenBracket == null
                                || (lastOperationWithCloseBracket != null
                                    && lastOperationWithOpenBracket.Order < lastOperationWithCloseBracket.Order);
            _closeBracketButton.Enabled = lastOperationWithOpenBracket != null
                                 && (lastOperationWithCloseBracket == null || lastOperationWithCloseBracket.Order < lastOperationWithOpenBracket.Order);
        }

        private void RefreshAll()
        {
            foreach (var localCalculation in _globalCalculation.LocalCalculations) {
                _calculationManager.SetOperationString(localCalculation);
            }

            _currentLocalCalculation = _globalCalculation.LocalCalculations
                .OrderBy(lc => lc.Order)
                .LastOrDefault();

            ((LocalCalculationListViewAdapter)_listView.Adapter).NotifyDataSetChanged();
            RefreshCanAddBrackets();
        }
    }

    public class LocalCalculationListViewAdapter : BaseAdapter<LocalCalculation>
    {
        private ObservableCollection<LocalCalculation> _localCalculations;
        private Activity _contextActivity;

        public LocalCalculationListViewAdapter(ObservableCollection<LocalCalculation> globalCalculations, Activity contextActivity)
        {
            _localCalculations = globalCalculations;
            _contextActivity = contextActivity;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override LocalCalculation this[int position]
        {
            get { return _localCalculations[position]; }
        }

        public override int Count
        {
            get { return _localCalculations.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = _contextActivity.LayoutInflater.Inflate(Resource.Layout.LocalCalculationListViewElement, null);
            view.FindViewById<TextView>(Resource.Id.OperationsTextView).Text = _localCalculations[position].OperationString;
            view.FindViewById<TextView>(Resource.Id.ResultTextView).Text = _localCalculations[position].Result.ToString("N2");
            return view;
        }
    }
}