using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace HabitTracker.Database
{
    public class HabitTrackerDatabase
    {
        protected readonly string connectionString = @"Data Source=habit-Tracker.db";

        public HabitTrackerDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS drinking_water(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Quantity INTEGER
                    )";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void Insert(string table, string date, int quantity)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"INSERT INTO {table}(date, quantity) VALUES ('{date}', '{quantity}')";


                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
