using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Backend.Interfaces;
using Foundation;
using SQLite.Net;
using TestsFinal.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Sqlite))]
namespace TestsFinal.iOS
{
    public class Sqlite : ISqliteConnectionService
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
    }
}