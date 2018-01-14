using System.IO;
using Net_Standard.Droid;
using Backend.Interfaces;
using SQLite.Net;
using xUnit.IntegrationTest;
using Environment = System.Environment;

[assembly: Xamarin.Forms.Dependency(typeof(Sqlite))]
namespace Net_Standard.Droid
{
    public class Sqlite : ISqliteConnectionService
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "StandradLibTests.db3";
            var folderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(folderPath, sqliteFilename);
            var platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }
    }
}