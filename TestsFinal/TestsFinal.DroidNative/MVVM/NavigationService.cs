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

namespace TestsFinal.DroidNative.MVVM
{
    public class NavigationService : INavigationService
    {
        public Activity CurrentActivity { get; set; }

        private readonly IDictionary<string, Type> _pagesViewModelDictionary = new Dictionary<string, Type>();

        public void PushView(string key)
        {
            lock (_pagesViewModelDictionary) {
                var type = _pagesViewModelDictionary[key];
                var intent = new Intent(CurrentActivity, type);
                CurrentActivity.StartActivity(intent);
            }
        }

        public void GoBack()
        {
            CurrentActivity.Finish();
        }

        public void Configure(string key, Type pageType)
        {
            lock (_pagesViewModelDictionary) {
                if (_pagesViewModelDictionary.ContainsKey(key)) {
                    _pagesViewModelDictionary[key] = pageType;
                } else {
                    _pagesViewModelDictionary.Add(key, pageType);
                }
            }
        }
    }
}