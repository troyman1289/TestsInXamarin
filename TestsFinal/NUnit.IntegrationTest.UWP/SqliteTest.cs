using System.IO;
using Windows.Storage;
using Backend.Model;
using NUnit.IntegrationTest.UWP;
using SQLite.Net;

[assembly: Xamarin.Forms.Dependency(typeof(SqliteTest))]
namespace NUnit.IntegrationTest.UWP
{
    public class SqliteTest : ISqliteConnectionForTest
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "NUnitTest.db3";
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);
            var platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }

        public void TeardownAndDelete()
        {
            var connection = GetConnection();
            if (connection == null)
                return;

            connection.DeleteAll<Operation>();
            connection.DeleteAll<LocalCalculation>();
            connection.DeleteAll<GlobalCalculation>();
            //var path = connection.DatabasePath;
            //connection.Close();
            //connection.Dispose();
            //connection = null;

            //if (File.Exists(path)) {
            //    File.Delete(path);
            //}
        }
    }
}
