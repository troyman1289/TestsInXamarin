using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Backend.Interfaces;
using Backend.Model;
using AlertDialog = Android.App.AlertDialog;
using Toolbar = Android.Widget.Toolbar;

namespace TestsFinal.DroidNative
{
    //[Activity(Label = "TestsFinal.DroidNative", MainLauncher = true, Theme = "@style/MainTheme")]
    public class MainActivityWithMVVM : AppCompatActivity
    {
        private RelativeLayout _progressBarRelativeLayout;
        private ListView _listView;
        private IList<GlobalCalculation> _globalCalculations;
        private ICalculationManager _calculationManager;
        private GlobalCalculation _currentGlobalCalculation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _calculationManager = CustomLocator.CalculationManager;
            _globalCalculations = _calculationManager.GetAllGlobalCalculations();

            SetContentView(Resource.Layout.Main);
            var toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);
            toolbar.InflateMenu(Resource.Menu.MainMenu);
            var getButton = toolbar.FindViewById<AppCompatButton>(Resource.Id.GetButton);
            getButton.SetBackgroundColor(Color.Transparent);
            getButton.Text = "Get";
            //getButton.SetCommand("Click", ViewModel.RetrieveShoppingListElementsCommand);
            getButton.Click += HandleGetButtonClick;

            _progressBarRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.ProgressBarLayout);
            _progressBarRelativeLayout.Visibility = ViewStates.Invisible;
            //Bindings.Add(this.SetBinding(() => ViewModel.IsBusy, () => _progressBarRelativeLayout.Visibility, BindingMode.OneWay)
            //    .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Invisible));

            _listView = FindViewById<ListView>(Resource.Id.ShoppingListListView);
            _listView.ItemClick += OnItemClick;
            //TODO _listView.Adapter = ViewModel.ShoppingListElementCollection.GetAdapter(GetShoppingListElementAdapter);
            RegisterForContextMenu(_listView);

            var floatingButton = FindViewById<FloatingActionButton>(Resource.Id.AddButton);
            floatingButton.Click += HandleAddButtonClick;
            //floatingButton.SetCommand("Click", ViewModel.AddShoppingListElementCommand);
        }

        private void HandleAddButtonClick(object sender, EventArgs e)
        {
            //TODO data through intent
            var intent = new Intent(this, typeof(CreateGlobalCalculationActivity));
            StartActivity(intent);
        }

        private async void HandleGetButtonClick(object sender, EventArgs e)
        {
            try
            {
                _progressBarRelativeLayout.Visibility = ViewStates.Visible;
                await _calculationManager.FetchGlobalCalculationsFromServiceAsync();
            } catch (Exception ex) {
                new AlertDialog.Builder(this)
                    .SetTitle("Error")
                    .SetMessage(ex.Message)
                    .SetPositiveButton("Ok",(o, args) => {})
                    .Show();
            } finally {
                _progressBarRelativeLayout.Visibility = ViewStates.Invisible;
            }

            SetGlobalCalculations();
        }

        private void SetGlobalCalculations()
        {
            var calculations = _calculationManager.GetAllGlobalCalculations();
            _globalCalculations.Clear();
            foreach (var globalCalculation in calculations) {
                _globalCalculations.Add(globalCalculation);
            }
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = _globalCalculations[e.Position];
            if (item == null)
                return;

            //TODO refresh on come back
            _currentGlobalCalculation = item;

            _calculationManager.LoadGlobalCalculation(_currentGlobalCalculation);
            //TODO data through intent
            var intent = new Intent(this, typeof(GlobalCalculationActivity));
            StartActivity(intent);
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
                    var globalCalculation = _globalCalculations[menuInfo.Position];
                    _calculationManager.RemoveGlobalCalculation(globalCalculation);
                    _globalCalculations.Remove(globalCalculation);
                    return true;
            }

            return base.OnContextItemSelected(item);
        }

    }
}

