using System;
using System.IO;
using SQLite.Net;
using xUnit.IntegrationTest.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(SqliteTest))]
namespace xUnit.IntegrationTest.iOS
{
    public class SqliteTest : ISqliteConnectionForTest
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "Tests.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, sqliteFilename);
            // Create the connection
            var platform = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
            var conn = new SQLiteConnection(platform, path);
            // Return the database connection
            return conn;

        }

        public void TeardownAndDelete()
        {
            var connection = GetConnection();
            if (connection == null)
                return;

            var path = connection.DatabasePath;
            connection.Close();
            connection = null;

            if (File.Exists(path)) {
                File.Delete(path);
            }
        }
    }
}