using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Backend.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using INavigationService = Backend.Interfaces.INavigationService;

namespace TestsFinal.DroidNative.MVVM
{
    public class BaseActivity<T> : AppCompatActivity where T : NotifyingObject
    {
        #region ViewModel

        private T _viewModel;

        protected T ViewModel
        {
            get { return _viewModel ?? (_viewModel = ServiceLocator.Current.GetInstance<T>()); }
        }

        #endregion

        private List<Binding> _bindings = new List<Binding>();

        protected List<Binding> Bindings
        {
            get { return _bindings; }
        }

        protected override void OnResume()
        {
            base.OnResume();
            ((NavigationService)ServiceLocator.Current.GetInstance<INavigationService>()).CurrentActivity = this;
        }

        protected override void OnDestroy()
        {
            foreach (var binding in Bindings) {
                binding.Detach();
            }
            Bindings.Clear();
            base.OnDestroy();
        }
    }
}