using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Interfaces;
using Backend.Model;

namespace Backend.RestService
{
    public class RestService : IRestService
    {
        public IList<GlobalCalculation> FetchGlobalCalculations()
        {
            throw new NotImplementedException();
        }

        public void SaveGlobalCalculations(IList<GlobalCalculation> globalCalculations)
        {
            
        }
    }
}
