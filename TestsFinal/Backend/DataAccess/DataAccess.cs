using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Interfaces;
using Backend.Model;
using SQLite.Net;

namespace Backend.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private static SQLiteConnection _connection;

        private static DataAccess _instance;
        public static DataAccess GetInstance()
        {
            if (_instance == null) {
                _instance = new DataAccess();
            }

            return _instance;
        }

        private DataAccess()
        {

        }

        public static void Init(ISqliteConnectionService sqliteConnectionService)
        {
            _connection = sqliteConnectionService.GetConnection();

            _connection.CreateTable<GlobalCalculation>();
            _connection.CreateTable<LocalCalculation>();
            _connection.CreateTable<Operation>();
        }

        public IList<GlobalCalculation> GetAllGlobalCalculations()
        {
            return _connection.Table<GlobalCalculation>().ToList();
        }

        public IList<LocalCalculation> GetLocalCalculations(int parentGlobalCalculationId)
        {
            return _connection.Table<LocalCalculation>()
                .Where(lc => lc.ParentGlobalCalculationId == parentGlobalCalculationId)
                .ToList();
        }

        public IList<Operation> GetOperations(int localCalculationParentId)
        {
            return _connection.Table<Operation>()
                .Where(o => o.ParentLocalCalculationId == localCalculationParentId)
                .ToList();
        }

        public void Insert(IEnumerable<Object> objs)
        {
            _connection.InsertAll(objs);
        }

        public void Insert(Object obj)
        {
            _connection.Insert(obj);
        }

        public void Update(Object obj)
        {
            _connection.Update(obj);
        }

        public void Remove(IEnumerable<Object> objs)
        {
            foreach (var o in objs) {
                Remove(o);
            }
        }

        public void Remove(Object obj)
        {
            _connection.Delete(obj);
        }
    }
}
