using System;
using System.IO;
using Backend.Interfaces;
using Backend.Model;
using NUnit.IntegrationTest.iOS;
using SQLite.Net;

[assembly: Xamarin.Forms.Dependency(typeof(Sqlite))]
namespace NUnit.IntegrationTest.iOS
{
    public class Sqlite : ISqliteConnectionService
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "NUnitTest.db3";
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