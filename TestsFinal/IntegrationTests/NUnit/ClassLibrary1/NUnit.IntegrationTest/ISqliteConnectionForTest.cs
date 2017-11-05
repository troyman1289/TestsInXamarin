using Backend.Interfaces;

namespace NUnit.IntegrationTest
{
    public interface ISqliteConnectionForTest : ISqliteConnectionService
    {
        void TeardownAndDelete();
    }
}
