using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Backend.Interfaces;
using Backend.Model;
using GalaSoft.MvvmLight.Helpers;
using Microsoft.Practices.ServiceLocation;
using ViewModels;
using AlertDialog = Android.App.AlertDialog;
using Toolbar = Android.Widget.Toolbar;

namespace TestsFinal.DroidNative.MVVM
{
    [Activity(Label = "TestsFinal.DroidNative", MainLauncher = true, Theme = "@style/MainTheme")]
    public class MainActivityWithMVVM : BaseActivity<MainViewModel>
    {
        private RelativeLayout _progressBarRelativeLayout;
        private ListView _listView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Locator.Init();
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.Toolbar);
            toolbar.InflateMenu(Resource.Menu.MainMenu);
            var getButton = toolbar.FindViewById<AppCompatButton>(Resource.Id.GetButton);
            getButton.SetBackgroundColor(Color.Transparent);
            getButton.Text = "Get";
            getButton.SetCommand("Click", ViewModel.FetchCalculationsCommand);

            _progressBarRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.ProgressBarLayout);
            _progressBarRelativeLayout.Visibility = ViewStates.Invisible;
            Bindings.Add(this.SetBinding(() => ViewModel.IsBusy, () => _progressBarRelativeLayout.Visibility, BindingMode.OneWay)
                  .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Invisible));

            _listView = FindViewById<ListView>(Resource.Id.ShoppingListListView);
            _listView.Adapter = ViewModel.GlobalCalculations.GetAdapter(GetListElementAdapter);
            _listView.ItemClick += HandleItemClick;
            RegisterForContextMenu(_listView);

            var floatingButton = FindViewById<FloatingActionButton>(Resource.Id.AddButton);
            floatingButton.SetCommand("Click", ViewModel.AddNewCalculationCommand);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var globalCalculation = ServiceLocator.Current.GetInstance<GlobalCalculationViewModel>().GlobalCalculation;
            if (globalCalculation != null) {
                var pos = ViewModel.GlobalCalculations.IndexOf(globalCalculation);
                var view = _listView.GetChildAt(pos);
                if(view == null) return;
                view.FindViewById<TextView>(Resource.Id.LabelTextView).Text = globalCalculation.Label;
                view.FindViewById<TextView>(Resource.Id.ResultTextView).Text = globalCalculation.Result.ToString();
                view.RefreshDrawableState();
            }
        }

        private void HandleItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ViewModel.GlobalCalculations[e.Position];
            if (item == null)
                return;

           ViewModel.OpenCalculationCommand.Execute(item);
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
                    var globalCalculation = ViewModel.GlobalCalculations[menuInfo.Position];
                    ViewModel.RemoveGlobalCalculationCommand.Execute(globalCalculation);
                    return true;
            }

            return base.OnContextItemSelected(item);
        }

        private View GetListElementAdapter(int position, GlobalCalculation globalCalculation, View convertView)
        {
            convertView = LayoutInflater.Inflate(Resource.Layout.GlobalCalculationListViewElement, null);

            convertView.FindViewById<TextView>(Resource.Id.LabelTextView).Text = globalCalculation.Label; 
            convertView.FindViewById<TextView>(Resource.Id.ResultTextView).Text = globalCalculation.Result.ToString();

            return convertView;
        }

    }
}

