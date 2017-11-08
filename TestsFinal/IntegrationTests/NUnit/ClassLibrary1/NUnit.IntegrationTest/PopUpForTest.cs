using System;
using System.Collections.Generic;
using Backend.Interfaces;

namespace NUnit.IntegrationTest
{
    public class PopUpForTest : IPopUpService
    {
        private List<bool> _actionResults;
        private int _index = 0;

        public List<bool> ActionResults
        {
            get { return _actionResults; }
            set
            {
                if (value != _actionResults)
                {
                    _actionResults = value;
                    _index = 0;
                }
            }
        }

        public void ResetIndex()
        {
            _index = 0;
        }

        public void ShowOkCancelPopUp(string title, string message, Action<bool> resultAction)
        {
            if (_index >= ActionResults.Count) {
                _index = 0;
            }
            resultAction.Invoke(ActionResults[_index]);
            _index++;
        }

        public void ShowAlertPopUp(string title, string message, Action resultAction)
        {
            throw new NotImplementedException();
        }
    }
}