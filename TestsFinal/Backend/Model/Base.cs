using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils;
using SQLite.Net.Attributes;

namespace Backend.Model
{
    public class Base : NotifyingObject
    {
        #region Id

        private int _id;

        [PrimaryKey, AutoIncrement]
        public int Id
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
}
