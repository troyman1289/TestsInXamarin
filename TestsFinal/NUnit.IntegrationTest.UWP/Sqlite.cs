using System.IO;
using Windows.Storage;
using Backend.Model;
using Backend.Interfaces;
using NUnit.IntegrationTest.UWP;
using SQLite.Net;

[assembly: Xamarin.Forms.Dependency(typeof(Sqlite))]
namespace NUnit.IntegrationTest.UWP
{
    public class Sqlite : ISqliteConnectionService
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "NUnitTest.db3";
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);
            var platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }
    }
}
