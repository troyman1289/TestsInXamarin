using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Model;

namespace Backend.Interfaces
{
    public interface ICalculationManager
    {
        IList<GlobalCalculation> GetAllGlobalCalculations();
        void RemoveLocalCalculation(GlobalCalculation globalCalculation, LocalCalculation localCalculation);
        void AddNewGlobalCalculation(GlobalCalculation globalCalculation);
    }
}
