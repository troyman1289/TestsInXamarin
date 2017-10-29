﻿using System.Collections.ObjectModel;
using System.Linq;
using SQLite.Net.Attributes;

namespace Backend.Model
{
    public class GlobalCalculation : Base
    {
        #region LocalCalculations

        private readonly ObservableCollection<LocalCalculation> _localCalculations = new ObservableCollection<LocalCalculation>();

        /// <summary>
        /// Collection of Operations
        /// </summary>
        public ObservableCollection<LocalCalculation> LocalCalculations
        {
            get { return _localCalculations; }
        }

        #endregion

        #region Result
        private decimal _result;

        public decimal Result
        {
            get { return _result; }
            set
            {
                if (value != _result) {
                    _result = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}