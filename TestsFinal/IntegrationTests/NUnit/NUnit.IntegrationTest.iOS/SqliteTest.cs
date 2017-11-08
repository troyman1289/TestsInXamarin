using System;
using System.IO;
using Backend.Model;
using NUnit.IntegrationTest.iOS;
using SQLite.Net;

[assembly: Xamarin.Forms.Dependency(typeof(SqliteTest))]
namespace NUnit.IntegrationTest.iOS
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

            connection.DeleteAll<Operation>();
            connection.DeleteAll<LocalCalculation>();
            connection.DeleteAll<GlobalCalculation>();
        }
    }
}