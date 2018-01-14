using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Model;

namespace NUnit.IntegrationTest
{
    public class DatabaseHelper
    {
        public static void CleanupDatabase(SQLite.Net.SQLiteConnection connection)
        {
            connection.DeleteAll<Operation>();
            connection.DeleteAll<LocalCalculation>();
            connection.DeleteAll<GlobalCalculation>();
        }
    }
}
