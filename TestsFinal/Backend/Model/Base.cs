using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Model.Operator;
using Backend.Utils;
using SQLite.Net.Attributes;

namespace Backend.Model
{
    public class Base : NotifyingObject
    {
        #region Id

        private string _id;

        [PrimaryKey, AutoIncrement]
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Label

        private string _label;

        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value) {
                    _label = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Order

        private int _order;

        public int Order
        {
            get { return _order; }
            set
            {
                if (_order != value) {
                    _order = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }

    //Statisch

    public enum OperatorType
    {
        Addition = 0,
        Subtraction = 1
    }

    public class Operation : Base
    {
        #region Operator

        private OperatorType _operatorType;

        public OperatorType OperatorType
        {
            get { return _operatorType; }
            set
            {
                if (_operatorType != value) {
                    _operatorType = value;
                    OnPropertyChanged();
                }
            }
        }

        private Operator.Operator _operator;

        [Ignore]
        public Operator.Operator Operator
        {
            get { return _operator; }
            set
            {
                if (_operator != value) {
                    _operator = value;
                    OnPropertyChanged();
                    OperatorType = Operator.OperatorType;
                }
            }
        }

        #endregion

        #region BracketType

        /// <summary>
        /// initial NONE
        /// </summary>
        private BracketType _bracketType = BracketType.None;

        public BracketType BracketType
        {
            get { return _bracketType; }
            set
            {
                if (_bracketType != value) {
                    _bracketType = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Operand

        private decimal _operand;

        public decimal Operand
        {
            get { return _operand; }
            set
            {
                if (_operand != value) {
                    _operand = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public void Calculate()
        {
            
        }
    }

    public class LocalCalculation : Base
    {

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

        public void Calculate()
        {

        }
    }

    public class GlobalCalculation : Base
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

        public void Calculate()
        {

        }
    }

    public enum BracketType
    {
        None = 0,
        Open = 1,
        Close = 2
    }
}
