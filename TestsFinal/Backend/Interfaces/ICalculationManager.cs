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
        void RemoveLocalCalculationWithRefresh(GlobalCalculation globalCalculation, LocalCalculation localCalculation);
        void AddNewGlobalCalculation(GlobalCalculation globalCalculation, decimal startOperand);
        void SetOperationString(LocalCalculation localCalculation);
        void LoadGlobalCalculation(GlobalCalculation globalCalculation);
        void AddOperation(LocalCalculation localCalculation,Operation operation);
        void AddNewLocalCalculation(GlobalCalculation globalCalculation, LocalCalculation localCalculation);
        void SetResult(LocalCalculation localCalculation);
        void RefreshGlobalResult(GlobalCalculation globalCalculation);
        Task FetchGlobalCalculationsFromServiceAsync();
    }
}
