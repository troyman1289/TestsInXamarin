using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backend.Interfaces;
using SQLite.Net;
using TestsFinal.Droid;
using Environment = System.Environment;

[assembly: Xamarin.Forms.Dependency(typeof(Sqlite))]
namespace TestsFinal.Droid
{
    public class Sqlite : ISqliteConnectionService
    {
        public SQLiteConnection GetConnection(string filename = "ShoppingList.db3")
        {
            var sqliteFilename = "Tests.db3";
            var folderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(folderPath, sqliteFilename);
            var platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }

        public void TeardownAndDelete(string filename)
        {
            var connection = GetConnection(filename);
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