using Android.App;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Toolbar = Android.Widget.Toolbar;

namespace TestsFinal.DroidNative
{
    [Activity(Label = "TestsFinal.DroidNative", MainLauncher = true, Theme = "@style/MainTheme")]
    public class MainActivity : AppCompatActivity
    {
        private RelativeLayout _progressBarRelativeLayout;
        private ListView _listView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);
            toolbar.InflateMenu(Resource.Menu.ShoppingListElementListMenu);
            var getButton = toolbar.FindViewById<AppCompatButton>(Resource.Id.GetButton);
            getButton.SetBackgroundColor(Color.Transparent);
            getButton.Text = "Get";
            getButton.SetCommand("Click", ViewModel.RetrieveShoppingListElementsCommand);

            _progressBarRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.ProgressBarLayout);
            Bindings.Add(this.SetBinding(() => ViewModel.IsBusy, () => _progressBarRelativeLayout.Visibility, BindingMode.OneWay)
                .ConvertSourceToTarget(b => b ? ViewStates.Visible : ViewStates.Invisible));

            _listView = FindViewById<ListView>(Resource.Id.ShoppingListListView);
            _listView.ItemClick += OnItemClick;
            _listView.Adapter = ViewModel.ShoppingListElementCollection.GetAdapter(GetShoppingListElementAdapter);
            RegisterForContextMenu(_listView);

            var floatingButton = FindViewById<FloatingActionButton>(Resource.Id.AddButton);
            floatingButton.SetCommand("Click", ViewModel.AddShoppingListElementCommand);
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = ViewModel.ShoppingListElementCollection[e.Position];
            if (item == null)
                return;

            if (_currentOpenShoppingListElement != null) {
                _currentOpenShoppingListElement.DetailElements.CollectionChanged -= OnCurrentShoppingListElementDetailElementsChanged;
            }

            _currentOpenShoppingListElement = item;
            _currentOpenShoppingListElement.DetailElements.CollectionChanged += OnCurrentShoppingListElementDetailElementsChanged;

            ViewModel.OpenShoppingListElementCommand.Execute(item);
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
                    var element = ViewModel.ShoppingListElementCollection[menuInfo.Position];
                    ViewModel.RemoveShoppingListElementCommand.Execute(element);
                    return true;
            }

            return base.OnContextItemSelected(item);
        }

    }
}

