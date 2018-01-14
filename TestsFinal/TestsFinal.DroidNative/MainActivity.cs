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
    [Activity(Label = "TestsFinal.DroidNative", MainLauncher = true, Theme = "@style/MainTheme")]
    public class MainActivity : AppCompatActivity
    {
        private RelativeLayout _progressBarRelativeLayout;
        private ListView _listView;
        private List<GlobalCalculation> _globalCalculations;
        private ICalculationManager _calculationManager;
        private GlobalCalculation _currentGlobalCalculation;

        private const int FromCreateGlobalActivityResult = 1;
        private const int FromGlobalCalculationActivityResult = 2;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _calculationManager = CustomLocator.CalculationManager;
            _globalCalculations = _calculationManager.GetAllGlobalCalculations().ToList();

            SetContentView(Resource.Layout.Main);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.Toolbar);
            toolbar.InflateMenu(Resource.Menu.MainMenu);
            var getButton = toolbar.FindViewById<AppCompatButton>(Resource.Id.GetButton);
            getButton.SetBackgroundColor(Color.Transparent);
            getButton.Text = "Get";
            getButton.Click += HandleGetButtonClick;

            _progressBarRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.ProgressBarLayout);
            _progressBarRelativeLayout.Visibility = ViewStates.Invisible;

            _listView = FindViewById<ListView>(Resource.Id.ShoppingListListView);
            _listView.ItemClick += OnItemClick;
            _listView.Adapter = new ListViewAdapter(_globalCalculations,this);
            RegisterForContextMenu(_listView);

            var floatingButton = FindViewById<FloatingActionButton>(Resource.Id.AddButton);
            floatingButton.Click += HandleAddButtonClick;
        }

        private void HandleAddButtonClick(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CreateGlobalCalculationActivity));
            StartActivityForResult(intent, FromCreateGlobalActivityResult);
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
            ((ListViewAdapter)_listView.Adapter).NotifyDataSetChanged();
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = _globalCalculations[e.Position];
            if (item == null)
                return;

            _currentGlobalCalculation = item;

            _calculationManager.LoadGlobalCalculation(_currentGlobalCalculation);
            var intent = new Intent(this, typeof(GlobalCalculationActivity));
            intent.PutExtra("GlobalCalculationId", item.Id);
            StartActivityForResult(intent, FromGlobalCalculationActivityResult);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == FromGlobalCalculationActivityResult) {
                if (_currentGlobalCalculation == null)
                    return;
                _currentGlobalCalculation = null;
                SetGlobalCalculations();
                return;
            }

            if (requestCode == FromCreateGlobalActivityResult) {
                SetGlobalCalculations();
            }
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

                    new AlertDialog.Builder(this)
                        .SetTitle("Remove")
                        .SetMessage("Do you want to remove it?")
                        .SetPositiveButton("Ok", (o, args) => RemoveGlobalCalculation(globalCalculation))
                        .SetNegativeButton("Cancel", (o, args) => { })
                        .Show();

                    return true;
            }

            return base.OnContextItemSelected(item);
        }

        private void RemoveGlobalCalculation(GlobalCalculation globalCalculation)
        {
            _calculationManager.RemoveGlobalCalculation(globalCalculation);
            _globalCalculations.Remove(globalCalculation);
            ((ListViewAdapter)_listView.Adapter).NotifyDataSetChanged();
        }

    }

    public class ListViewAdapter : BaseAdapter<GlobalCalculation>
    {
        private List<GlobalCalculation> _globalCalculations;
        private Activity _contextActivity;

        public ListViewAdapter(List<GlobalCalculation> globalCalculations, Activity contextActivity)
        {
            _globalCalculations = globalCalculations;
            _contextActivity = contextActivity;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override GlobalCalculation this[int position]
        {
            get { return _globalCalculations[position]; }
        }

        public override int Count
        {
            get { return _globalCalculations.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = _contextActivity.LayoutInflater.Inflate(Resource.Layout.GlobalCalculationListViewElement, null);
            view.FindViewById<TextView>(Resource.Id.LabelTextView).Text = _globalCalculations[position].Label;
            view.FindViewById<TextView>(Resource.Id.ResultTextView).Text = _globalCalculations[position].Result.ToString("N2");
            return view;
        }
    }
}

