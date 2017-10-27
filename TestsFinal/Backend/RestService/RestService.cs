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
        public Task<IList<GlobalCalculation>> FetchGlobalCalculations()
        {
            throw new NotImplementedException();
        }

        public Task SaveGlobalCalculations(IList<GlobalCalculation> globalCalculations)
        {
            throw new NotImplementedException();
        }
    }
}
