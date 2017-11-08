using System.IO;
using Backend.Model;
using NUnit.IntegrationTest.Droid;
using SQLite.Net;
using Environment = System.Environment;

[assembly: Xamarin.Forms.Dependency(typeof(SqliteTest))]
namespace NUnit.IntegrationTest.Droid
{
    public class SqliteTest : ISqliteConnectionForTest
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "Tests.db3";
            var folderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(folderPath, sqliteFilename);
            var platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
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
        }
    }
}