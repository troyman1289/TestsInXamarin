using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface IPopUpService
    {
        void ShowOkCancelPopUp(string title, string message, Action<bool> resultAction);
        void ShowAlertPopUp(string title, string message, Action resultAction);
    }
}
