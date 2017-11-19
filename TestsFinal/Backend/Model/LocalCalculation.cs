using System.Collections.ObjectModel;
using SQLite.Net.Attributes;

namespace Backend.Model
{
    public class LocalCalculation : Base
    {

        #region StartOperand

        private decimal _startOperand;

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

        #region OperationString

        private string _operationString;

        [Ignore]
        public string OperationString
        {
            get { return _operationString; }
            set
            {
                if (value != _operationString) {
                    _operationString = value;
                    OnPropertyChanged();
                }
            }
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

        #region ParentGlobalCalculation

        public int ParentGlobalCalculationId { get; set; }

        [Ignore]
        public GlobalCalculation ParentGlobalCalculation { get; set; }

        #endregion

        #region Operations

        private readonly ObservableCollection<Operation> _operations = new ObservableCollection<Operation>();

        /// <summary>
        /// Collection of Operations
        /// </summary>
        public ObservableCollection<Operation> Operations
        {
            get { return _operations; }
        }

        #endregion
    }
}