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

namespace TestsFinal.DroidNative
{
    public class PopUpService : IPopUpService
    {
        public Context Context { get; set; }

        public void ShowOkCancelPopUp(string title, string message, Action<bool> resultAction)
        {
            new AlertDialog.Builder(Context)
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