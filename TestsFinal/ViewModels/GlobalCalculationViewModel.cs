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
        private readonly INavigationService _navigationService;
        private readonly MainViewModel _mainViewModel;

        public GlobalCalculationViewModel(
            ICalculationManager calculationManager,
            INavigationService navigationService,
            MainViewModel mainViewModel)
        {
            _calculationManager = calculationManager;
            _navigationService = navigationService;
            _mainViewModel = mainViewModel;
        }       
        
        #region GlobalCalculation

        private GlobalCalculation _globalCalculation = new GlobalCalculation();

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

        #region StartOperand

        private decimal _startOperand = 0;

        public decimal StartOperand
        {
            get { return _startOperand; }
            set
            {
                if (_startOperand != value) {
                    _startOperand = value;
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
            _calculationManager.AddNewGlobalCalculation(GlobalCalculation,StartOperand);
             GlobalCalculation = new GlobalCalculation();
            _mainViewModel.RefreshCalculations();
            _navigationService.GoBack();
        }

        #endregion
    }
}
