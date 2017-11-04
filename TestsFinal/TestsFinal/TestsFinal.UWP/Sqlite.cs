using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Backend.Interfaces;
using SQLite.Net;
using TestsFinal.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(Sqlite))]
namespace TestsFinal.UWP
{
    public class Sqlite : ISqliteConnectionService
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "Tests.db3";
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);
            var platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }
    }
}
