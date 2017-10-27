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
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "ShoppingList.db3";
            var folderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(folderPath, sqliteFilename);
            var platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }
    }
}