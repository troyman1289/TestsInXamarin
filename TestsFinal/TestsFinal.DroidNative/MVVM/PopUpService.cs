using System;
using Android.App;
using Android.Content;
using Backend.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace TestsFinal.DroidNative.MVVM
{
    public class PopUpService : IPopUpService
    {
        public void ShowOkCancelPopUp(string title, string message, Action<bool> resultAction)
        {
            var context = ((NavigationService) ServiceLocator.Current.GetInstance<INavigationService>())
                .CurrentActivity;
            new AlertDialog.Builder(context)
                .SetTitle(title)
                .SetMessage(message)
                .SetPositiveButton(
                    "Ok",
                    (sender, args) => {
                        resultAction(true);
                    })
                .SetNegativeButton(
                    "Cancel",
                    (sender, args) => {
                        resultAction(true);
                    })
                .Show();
        }

        public void ShowAlertPopUp(string title, string message, Action resultAction)
        {
            
        }
    }
}