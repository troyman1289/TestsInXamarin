using System;
using System.Collections.Generic;
using Backend.Interfaces;

namespace NUnit.IntegrationTest
{
    public class PopUpForTest : IPopUpService
    {
        public bool ActionResultValue { get;set;}

        public void ShowOkCancelPopUp(string title, string message, Action<bool> resultAction)
        {
            resultAction.Invoke(ActionResultValue);
        }

        public void ShowAlertPopUp(string title, string message, Action resultAction)
        {
            throw new NotImplementedException();
        }
    }
}