using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Model;

namespace Backend.Interfaces
{
    public interface IDataAccess
    {
        IList<GlobalCalculation> GetAllGlobalCalculations();
        void Insert(IEnumerable<Object> objs);
        void Insert(Object obj);
        void Remove(IEnumerable<Object> objs);
        void Remove(Object obj);
        IList<LocalCalculation> GetLocalCalculations(int parentGlobalCalculationId);
        IList<Operation> GetOperations(int localCalculationParentId);
        void Update(Object obj);
    }
}
