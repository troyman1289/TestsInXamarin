using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Backend.Interfaces;
using Backend.Model;
using SQLite.Net;
using xUnit.IntegrationTest.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(Sqlite))]
namespace xUnit.IntegrationTest.UWP
{
    public class Sqlite : ISqliteConnectionService
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "TestTests.db3";
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);
            var platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }
    }
}
