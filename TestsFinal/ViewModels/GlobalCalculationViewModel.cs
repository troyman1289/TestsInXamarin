using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Backend.Interfaces;
using Backend.Model;
using Backend.Utils;
using GalaSoft.MvvmLight.Command;

namespace ViewModels
{
    public class GlobalCalculationViewModel : NotifyingObject
    {
        private readonly ICalculationManager _calculationManager;

        public GlobalCalculationViewModel(ICalculationManager calculationManager)
        {
            _calculationManager = calculationManager;
        }       
        
        #region GlobalCalculation

        private GlobalCalculation _globalCalculation;

        public GlobalCalculation GlobalCalculation
        {
            get { return _globalCalculation; }
            set
            {
                if (_globalCalculation != value) {
                    _globalCalculation = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region SaveCalculationCommand

        private ICommand _saveCalculationCommand;

        public ICommand SaveCalculationCommand
        {
            get
            {
                if (_saveCalculationCommand == null) {
                    _saveCalculationCommand = new RelayCommand(HandleSaveCalculation);
                }
                return _saveCalculationCommand;
            }
        }

        private void HandleSaveCalculation()
        {

        }

        #endregion
    }
}
