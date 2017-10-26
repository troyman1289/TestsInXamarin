using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Model;

namespace Backend.Interfaces
{
    public interface IRestService
    {
        IList<GlobalCalculation> FetchGlobalCalculations();
        void SaveGlobalCalculations(IList<GlobalCalculation> globalCalculations);
    }
}
