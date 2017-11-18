using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Backend.Interfaces;
using Backend.Model;
using Backend.Model.Operator;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using ViewModels;

namespace TestsFinal.DroidNative.MVVM
{
    [Activity(Label = "GlobalCalculationActivityMVVM", Theme = "@style/MainTheme")]
    public class GlobalCalculationActivityMVVM : BaseActivity<GlobalCalculationViewModel>
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
        private Button _addOperationButton;
        private Button _addLocalCalculationButton;

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
            _listView.Adapter = ViewModel.GlobalCalculation.LocalCalculations.GetAdapter(GetListElementAdapter);
            RegisterForContextMenu(_listView);

            _operandEditText = FindViewById<EditText>(Resource.Id.OperandEditText);
            Bindings.Add(this.SetBinding(() => ViewModel.NewOperation.Operand, () => _operandEditText.Text, BindingMode.TwoWay));

            _operatorSpinner = FindViewById<Spinner>(Resource.Id.OperatorSpinner);
            ArrayAdapter<String> operationsAdapter = new ArrayAdapter<String>(this,
                Android.Resource.Layout.SimpleSpinnerItem, Operators.GetAll().Select(o => o.Label).ToList());
            _operatorSpinner.Adapter = operationsAdapter;
            _operatorSpinner.ItemSelected += HandleOperationSpinnerItemSelected;
     
            _openBracketButton = FindViewById<Button>(Resource.Id.AddOpenBracketButton);
            _openBracketButton.SetCommand("Click", (RelayCommand<BracketType>) ViewModel.AddBracketCommand,BracketType.Open);
            Bindings.Add(this.SetBinding(() => ViewModel.CanUseOpenBracket, () => _openBracketButton.Enabled, BindingMode.OneWay));

            _closeBracketButton = FindViewById<Button>(Resource.Id.AddCloseBracketButton);
            _closeBracketButton.SetCommand("Click", (RelayCommand<BracketType>)ViewModel.AddBracketCommand, BracketType.Close);
            Bindings.Add(this.SetBinding(() => ViewModel.CanUseCloseBracket, () => _closeBracketButton.Enabled, BindingMode.OneWay));

            _addOperationButton = FindViewById<Button>(Resource.Id.AddoperationButton);
            _addOperationButton.SetCommand("Click", ViewModel.AddOperationCommand);

            _addLocalCalculationButton = FindViewById<Button>(Resource.Id.AddLocalCalculationButton);           
            _addLocalCalculationButton.SetCommand("Click", ViewModel.AddLocalCalculationCommand);

            ViewModel.CurrentLocalCalculation.PropertyChanged += HandleLocalCalculationPropertyChanged;
            ViewModel.PropertyChanged += HandleViewModelPropertyChanged;
        }

        private void HandleViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CurrentLocalCalculation") && ViewModel.CurrentLocalCalculation != null)
            {
                if (_currentLocalCalculation != null) {
                    _currentLocalCalculation.PropertyChanged -= HandleLocalCalculationPropertyChanged;
                }
                ViewModel.CurrentLocalCalculation.PropertyChanged += HandleLocalCalculationPropertyChanged;
                _currentLocalCalculation = ViewModel.CurrentLocalCalculation;
            }
        }

        private void HandleLocalCalculationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshListViewView(ViewModel.CurrentLocalCalculation);
        }

        private void RefreshListViewView(LocalCalculation localCalculation)
        {
            var pos = ViewModel.GlobalCalculation.LocalCalculations.IndexOf(localCalculation);
            if(pos == -1) return;
            var view = _listView.GetChildAt(pos);
            view.FindViewById<TextView>(Resource.Id.OperationsTextView).Text = localCalculation.OperationString;
            view.FindViewById<TextView>(Resource.Id.ResultTextView).Text = localCalculation.Result.ToString("N2");
            view.RefreshDrawableState();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_currentLocalCalculation != null) {
                _currentLocalCalculation.PropertyChanged -= HandleLocalCalculationPropertyChanged;
            }
            ViewModel.PropertyChanged -= HandleViewModelPropertyChanged;
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
                    var localCalculation = ViewModel.GlobalCalculation.LocalCalculations[menuInfo.Position];
                    ViewModel.RemoveLocalCalculationCommand.Execute(localCalculation);
                    RefreshAll();
                    return true;
            }

            return base.OnContextItemSelected(item);
        }

        private void HandleOperationSpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            _selectedOperator = Operators.GetAll()[e.Position];
        }

        private View GetListElementAdapter(int position, LocalCalculation localCalculation, View convertView)
        {
            convertView = LayoutInflater.Inflate(Resource.Layout.LocalCalculationListViewElement, null);

            convertView.FindViewById<TextView>(Resource.Id.OperationsTextView).Text = localCalculation.OperationString;
            convertView.FindViewById<TextView>(Resource.Id.ResultTextView).Text = localCalculation.Result.ToString("N2");

            return convertView;
        }

        private void RefreshAll()
        {
            foreach (var localCalculation in ViewModel.GlobalCalculation.LocalCalculations) {
                _calculationManager.SetOperationString(localCalculation);
                RefreshListViewView(localCalculation);
            }
        }
    }
}