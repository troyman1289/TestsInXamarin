using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class MainViewModel : NotifyingObject
    {
        private readonly ICalculationManager _calculationManager;
        private readonly INavigationService _navigationService;

        public MainViewModel(
            ICalculationManager calculationManager,
            INavigationService navigationService)
        {
            _calculationManager = calculationManager;
            _navigationService = navigationService;
            SetGlobalCalculations();
        }

        #region IsBusy

        private bool _isBusy = false;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value) {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region GlobalCalculations

        private readonly ObservableCollection<GlobalCalculation> _globalCalculations = new ObservableCollection<GlobalCalculation>();

        /// <summary>
        /// Collection of Operations
        /// </summary>
        public ObservableCollection<GlobalCalculation> GlobalCalculations
        {
            get { return _globalCalculations; }
        }

        #endregion

        #region GlobalResult

        private string _globalResult;

        public string GlobalResult
        {
            get { return _globalResult; }
            set
            {
                if (_globalResult != value) {
                    _globalResult = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        private void SetGlobalCalculations()
        {
            var calculations = _calculationManager.GetAllGlobalCalculations();
            GlobalCalculations.Clear();
            foreach (var globalCalculation in calculations) {
                GlobalCalculations.Add(globalCalculation);
            }

            //add placeholder for new global calculations
            var palceHolder = new GlobalCalculation {Order = int.MaxValue, Label = "New..."};
            GlobalCalculations.Add(palceHolder);

            SetGlobalResult();
        }

        private void SetGlobalResult()
        {
    
        }

        public void RefreshCalculations()
        {
            SetGlobalCalculations();
        }

        #region OpenCalculationCommand

        private ICommand _openCalculationCommand;

        public ICommand OpenCalculationCommand
        {
            get
            {
                if (_openCalculationCommand == null) {
                    _openCalculationCommand = new RelayCommand<LocalCalculation>(HandleOpenLocalCalculation, (localCalculation) => !IsBusy);
                }
                return _openCalculationCommand;
            }
        }

        private void HandleOpenLocalCalculation(LocalCalculation localCalculation)
        {
            _navigationService.PushView(typeof(GlobalCalculationViewModel).ToString());
        }

        #endregion

        #region AddNewCalculationCommand

        private ICommand _addNewCalculationCommand;

        public ICommand AddNewCalculationCommand
        {
            get
            {
                if (_addNewCalculationCommand == null) {
                    _addNewCalculationCommand = new RelayCommand(HandleAddNewCalculation, () => !IsBusy);
                }
                return _addNewCalculationCommand;
            }
        }

        private void HandleAddNewCalculation()
        {
            _navigationService.PushView(typeof(GlobalCalculationViewModel).ToString());
        }

        #endregion

        #region FetchCalculationsCommand

        private ICommand _fetchCalculationsCommand;

        public ICommand FetchCalculationsCommand
        {
            get
            {
                if (_fetchCalculationsCommand == null) {
                    _fetchCalculationsCommand = new RelayCommand(HandleFetchCalculation, () => !IsBusy);
                }
                return _fetchCalculationsCommand;
            }
        }

        private void HandleFetchCalculation()
        {

        }

        #endregion

        #region SaveCalculationsCommand

        private ICommand _saveCalculationsCommand;

        public ICommand SaveCalculationsCommand
        {
            get
            {
                if (_saveCalculationsCommand == null) {
                    _saveCalculationsCommand = new RelayCommand(HandleSaveCalculation, () => !IsBusy);
                }
                return _saveCalculationsCommand;
            }
        }

        private void HandleSaveCalculation()
        {

        }

        #endregion

        #region RemoveGlobalCalculationCommand

        private ICommand _removeGlobalCalculationCommand;

        public ICommand RemoveGlobalCalculationCommand
        {
            get
            {
                if (_removeGlobalCalculationCommand == null) {
                    _removeGlobalCalculationCommand = new RelayCommand<GlobalCalculation>(HandleRemoveGlobalCalculation);
                }
                return _removeGlobalCalculationCommand;
            }
        }

        private void HandleRemoveGlobalCalculation(GlobalCalculation globalCalculation)
        {

        }

        #endregion

    }
}
