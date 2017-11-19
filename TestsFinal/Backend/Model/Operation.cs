using Backend.Model.Operator;
using SQLite.Net.Attributes;

namespace Backend.Model
{
    public class Operation : Base
    {
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

        #region ParentLocalCalculation

        public int ParentLocalCalculationId { get; set; }

        [Ignore]
        public LocalCalculation ParentLocalCalculation { get; set; }

        #endregion

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
                AssignOperator();
            }
        }

        private Operator.Operator _operator;

        [Ignore]
        public Operator.Operator Operator
        {
            get { return _operator; }
            private set
            {
                if (_operator != value) {
                    _operator = value;
                    OnPropertyChanged();
                }
            }
        }

        public void AssignOperator()
        {
            switch (OperatorType) {
                case OperatorType.Addition:
                    Operator = Operators.Addition;
                    break;
                case OperatorType.Subtraction:
                    Operator = Operators.Substraction;
                    break;
                case OperatorType.Multiplication:
                    Operator = Operators.Multiplication;
                    break;
                case OperatorType.Division:
                    Operator = Operators.Division;
                    break;
            }
        }

        #endregion

    }
}