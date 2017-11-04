﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;

namespace Backend.Interfaces
{
    public interface ISqliteConnectionService
    {
        SQLiteConnection GetConnection(string filename = "ShoppingList.db3");

        void TeardownAndDelete(string filename);
    }
}
