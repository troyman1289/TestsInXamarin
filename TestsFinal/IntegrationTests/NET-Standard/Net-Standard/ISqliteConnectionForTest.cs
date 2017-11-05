using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Interfaces;

namespace xUnit.IntegrationTest
{
    public interface ISqliteConnectionForTest : ISqliteConnectionService
    {
        void TeardownAndDelete();
    }
}
