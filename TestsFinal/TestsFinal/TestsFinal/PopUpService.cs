using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Interfaces;

namespace TestsFinal
{
    public class PopUpService : IPopUpService
    {
        private readonly INavigationService _navigationService;

        public PopUpService(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public async void ShowOkCancelPopUp(string title, string message, Action<bool> resultAction)
        {
            var result = await ((NavigationService) _navigationService).CurrentPage.DisplayAlert(title, message, "ok", "cancel");
            resultAction(result);
        }

        public async void ShowAlertPopUp(string title, string message, Action resultAction)
        {
            await((NavigationService)_navigationService).CurrentPage.DisplayAlert(title, message,"ok");
            resultAction();
        }
    }
}
