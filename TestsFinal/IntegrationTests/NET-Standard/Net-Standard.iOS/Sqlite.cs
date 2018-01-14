using System;
using System.IO;
using Backend.Interfaces;
using Net_Standard.iOS;
using SQLite.Net;
using xUnit.IntegrationTest;

[assembly: Xamarin.Forms.Dependency(typeof(Sqlite))]
namespace Net_Standard.iOS
{
    public class Sqlite : ISqliteConnectionService
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "StandradLibTests.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, sqliteFilename);
            // Create the connection
            var platform = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
            var conn = new SQLiteConnection(platform, path);
            // Return the database connection
            return conn;

        }
    }
}