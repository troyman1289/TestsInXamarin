﻿using System.IO;
using Net_Standard.Droid;
using SQLite.Net;
using xUnit.IntegrationTest;
using Environment = System.Environment;

[assembly: Xamarin.Forms.Dependency(typeof(SqliteTest))]
namespace Net_Standard.Droid
{
    public class SqliteTest : ISqliteConnectionForTest
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

        public void TeardownAndDelete()
        {
            var connection = GetConnection();
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