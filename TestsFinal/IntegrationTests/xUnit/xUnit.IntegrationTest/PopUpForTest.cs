using System;
using System.Collections.Generic;
using Backend.Interfaces;

namespace xUnit.IntegrationTest
{
    public class PopUpForTest : IPopUpService
    {
        public bool ActionResultValue { get; set; }

        public void ShowAlertPopUp(string title, string message, Action resultAction)
        {
            throw new NotImplementedException();
        }

        public void ShowOkCancelPopUp(string title, string message, Action<bool> resultAction)
        {
            resultAction.Invoke(ActionResultValue);
        }
    }
}