using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Backend.Interfaces;
using Xamarin.Forms;

namespace TestsFinal
{
    public class NavigationService : INavigationService
    {
        private readonly IDictionary<string, Type> _pageViewModelDictionary = new Dictionary<string, Type>();

        public INavigation Navigation { get; set; }

        public Page CurrentPage { get; set; }

        public void PushView(string key)
        {
            lock (_pageViewModelDictionary)
            {
                var type = _pageViewModelDictionary[key];
                var constructor = type.GetTypeInfo().DeclaredConstructors.First();
                var page = constructor.Invoke(null) as Page;
                CurrentPage = page;
                Navigation.PushAsync(page);
            }
        }

        public async void GoBack()
        {
            CurrentPage = await Navigation.PopAsync();
        }

        public void Configure(string key, Type pageType)
        {
            lock (_pageViewModelDictionary)
            {
                if (_pageViewModelDictionary.ContainsKey(key))
                {
                    _pageViewModelDictionary[key] = pageType;
                }
                else
                {
                    _pageViewModelDictionary.Add(key, pageType);
                }
            }
        }
    }
}
