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

        public MainViewModel(ICalculationManager calculationManager)
        {
            _calculationManager = calculationManager;
            SetGlobalCalculations();
        }

        #region IsBusy

        private bool _isBusy;

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


        #region SelectedGlobalCalculation

        private GlobalCalculation _selectedGlobalCalculation;

        public GlobalCalculation SelectedGlobalCalculation
        {
            get { return _selectedGlobalCalculation; }
            set
            {
                if (_selectedGlobalCalculation != value) {
                    _selectedGlobalCalculation = value;
                    OnPropertyChanged();
                }
            }
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
            SetGlobalResult();
        }

        private void SetGlobalResult()
        {
            
        }

        public void RefreshCalculation()
        {
            
        }



        #region RemoveLocalCalculationCommand

        private ICommand _removeLocalCalculationCommand;

        public ICommand RemoveLocalCalculationCommand
        {
            get
            {
                if (_removeLocalCalculationCommand == null) {
                    _removeLocalCalculationCommand = new RelayCommand<LocalCalculation>(HandleRemoveLocalCalculation);
                }
                return _removeLocalCalculationCommand;
            }
        }

        private void HandleRemoveLocalCalculation(LocalCalculation localCalculation)
        {

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
            
        }

        #endregion

    }
}
