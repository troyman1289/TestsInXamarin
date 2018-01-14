using System.IO;
using Backend.Interfaces;
using Backend.Model;
using NUnit.IntegrationTest.Droid;
using SQLite.Net;
using Environment = System.Environment;

[assembly: Xamarin.Forms.Dependency(typeof(Sqlite))]
namespace NUnit.IntegrationTest.Droid
{
    public class Sqlite : ISqliteConnectionService
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "NUnitTest.db3";
            var folderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(folderPath, sqliteFilename);
            var platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }
    }
}