using System.IO;
using Backend.Interfaces;
using SQLite.Net;
using Environment = System.Environment;

namespace TestsFinal.DroidNative
{
    public class Sqlite : ISqliteConnectionService
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "Tests.db3";
            var folderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(folderPath, sqliteFilename);
            var platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var connection = new SQLiteConnection(platform, path);

            return connection;
        }
    }
}