using HabitTracker.Models;
using Microsoft.Data.Sqlite;
using static HabitTracker.Database.HabitTrackerDatabase;

namespace HabitTracker.Database
{
    internal class SeedData
    {
        public void InitDatabase()
        {
            CreateTables();
            InsertSeedHabits();
            InsertSeedEntries();
        }

        private void CreateTables()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var createHabitsTable = connection.CreateCommand();
                createHabitsTable.CommandText =
                    @$"CREATE TABLE IF NOT EXISTS {habitsTableName}(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    CreationDate TEXT,
                    Unit TEXT
                    )";
                createHabitsTable.ExecuteNonQuery();

                var createEntriesTable = connection.CreateCommand();
                createEntriesTable.CommandText =
                    @$"CREATE TABLE IF NOT EXISTS {habitsEntriesTableName}(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    HabitId INTEGER NOT NULL,
                    Quantity DECIMAL (10,2),
                    Date TEXT,
                    FOREIGN KEY (HabitId) REFERENCES habits(Id) ON DELETE CASCADE
                    )";
                createEntriesTable.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void InsertSeedHabits()
        {
            var habits = new List<Habit>
            {
                new Habit
                {
                    Name = "Drinking Water",
                    Unit = "glasses",
                    CreationDate = DateTime.Now.AddMonths(-3),
                },
                new Habit
                {
                    Name = "Running",
                    Unit = "km",
                    CreationDate = DateTime.Now.AddMonths(-3),
                },
                new Habit
                {
                    Name = "Reading",
                    Unit = "pages",
                    CreationDate = DateTime.Now.AddMonths(-3),
                },
                new Habit
                {
                    Name = "Meditation",
                    Unit = "minutes",
                    CreationDate = DateTime.Now.AddMonths(-3),
                },
                new Habit
                {
                    Name = "Exercise",
                    Unit = "minutes",
                    CreationDate = DateTime.Now.AddMonths(-3),
                },
                new Habit
                {
                    Name = "Sleep",
                    Unit = "hours",
                    CreationDate = DateTime.Now.AddMonths(-3),
                },
                new Habit
                {
                    Name = "Fruits",
                    Unit = "portions",
                    CreationDate = DateTime.Now.AddMonths(-3),
                },
            };

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var checkIfSeeded = connection.CreateCommand();
                checkIfSeeded.CommandText = $"SELECT COUNT(*) FROM {habitsTableName}";
                var existingCount = Convert.ToInt64(checkIfSeeded.ExecuteScalar());

                if (existingCount > 0)
                    return;

                foreach (var habit in habits)
                {
                    InsertHabit(habit, connection);
                }

                connection.Close();
            }
        }

        private void InsertSeedEntries()
        {
            var random = new Random();
            var startDate = DateTime.Now.AddMonths(-3);
            var endDate = DateTime.Now;

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkIfSeeded = connection.CreateCommand();
                checkIfSeeded.CommandText = $"SELECT COUNT(*) FROM {habitsEntriesTableName}";
                var existingCount = Convert.ToInt64(checkIfSeeded.ExecuteScalar());

                if (existingCount > 0)
                    return;

                var getHabitsCmd = connection.CreateCommand();
                getHabitsCmd.CommandText = $"SELECT Id, Name, Unit FROM {habitsTableName}";

                var habits = new List<(int Id, string Name, string Unit)>();

                using (var reader = getHabitsCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        habits.Add((reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                    }
                }

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    foreach (var habit in habits)
                    {
                        if (random.NextDouble() > 0.2)
                        {
                            var quantity = GenerateRealisticQuantity(
                                habit.Name,
                                habit.Unit,
                                random
                            );

                            HabitEntry newEntry = new();
                            newEntry.HabitId = habit.Id;
                            newEntry.Quantity = quantity;
                            newEntry.Date = date;

                            InsertEntry(newEntry, connection);
                        }
                    }
                }

                connection.Close();
            }
        }

        private int GenerateRealisticQuantity(string habitName, string unit, Random random)
        {
            return habitName switch
            {
                "Drinking Water" => random.Next(4, 12), // 4-11 glasses
                "Running" => random.Next(2, 10), // 2-9 km
                "Reading" => random.Next(10, 50), // 10-49 pages
                "Meditation" => random.Next(5, 30), // 5-29 mins
                "Exercise" => random.Next(15, 90), // 15-89 mins
                "Sleep" => random.Next(5, 9), // 5-8 hours
                "Fruits" => random.Next(1, 5), // 1-4 portions
                _ => random.Next(1, 10),
            };
        }
    }
}
