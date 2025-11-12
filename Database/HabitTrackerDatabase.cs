using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HabitTracker.Models;
using Microsoft.Data.Sqlite;

namespace HabitTracker.Database
{
    internal static class HabitTrackerDatabase
    {
        internal static readonly string connectionString = @"Data Source=habit-Tracker.db";
        internal static readonly string habitsTableName = "habits";
        internal static readonly string habitsEntriesTableName = "habit_entries";

        internal static void InsertHabit(Habit newHabit, SqliteConnection connection)
        {
            var insertHabit = connection.CreateCommand();
            insertHabit.CommandText =
                $"INSERT INTO {habitsTableName}(Name, Unit, CreationDate) VALUES (@name,@unit,@creationDate)";
            insertHabit.Parameters.AddWithValue("@name", newHabit.Name);
            insertHabit.Parameters.AddWithValue("@unit", newHabit.Unit);
            insertHabit.Parameters.AddWithValue("@creationDate", newHabit.DisplayCreationDate);
            insertHabit.ExecuteNonQuery();
        }

        internal static void InsertHabit(Habit newHabit)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var insertHabit = connection.CreateCommand();
                insertHabit.CommandText =
                    $"INSERT INTO {habitsTableName}(Name, Unit, CreationDate) VALUES (@name,@unit,@creationDate)";
                insertHabit.Parameters.AddWithValue("@name", newHabit.Name);
                insertHabit.Parameters.AddWithValue("@unit", newHabit.Unit);
                insertHabit.Parameters.AddWithValue("@creationDate", newHabit.DisplayCreationDate);
                insertHabit.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal static void UpdateHabit(int habitId, string newName, string newUnit)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                    @"
            UPDATE habits 
            SET Name = @name, Unit = @unit 
            WHERE Id = @id";

                command.Parameters.AddWithValue("@name", newName);
                command.Parameters.AddWithValue("@unit", newUnit);
                command.Parameters.AddWithValue("@id", habitId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException("Habit not found or no changes made");
                }

                connection.Close();
            }
        }

        internal static bool DeleteHabit(int habitId)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"DELETE FROM {habitsTableName} WHERE Id = @id";
                command.Parameters.AddWithValue("@id", habitId);

                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                return rowsAffected > 0;
            }
        }

        internal static void InsertEntry(HabitEntry newEntry, SqliteConnection connection)
        {
            var insertEntry = connection.CreateCommand();
            insertEntry.CommandText =
                $"INSERT INTO {habitsEntriesTableName}(HabitId, Quantity, Date) VALUES (@habitId,@quantity,@date)";
            insertEntry.Parameters.AddWithValue("@habitId", newEntry.HabitId);
            insertEntry.Parameters.AddWithValue("@quantity", newEntry.Quantity);
            insertEntry.Parameters.AddWithValue("@date", newEntry.DisplayDate);
            insertEntry.ExecuteNonQuery();
        }

        internal static void InsertEntry(HabitEntry newEntry)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var insertEntry = connection.CreateCommand();
                insertEntry.CommandText =
                    @$"
            INSERT INTO {habitsEntriesTableName}(HabitId, Quantity, Date)
            VALUES (@habitId, @quantity, @date)";
                insertEntry.Parameters.AddWithValue("@habitId", newEntry.HabitId);
                insertEntry.Parameters.AddWithValue("@quantity", newEntry.Quantity);
                insertEntry.Parameters.AddWithValue("@date", newEntry.DisplayDate);
                insertEntry.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal static List<Habit> GetAllHabits()
        {
            var habits = new List<Habit>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var getHabitsCommand = connection.CreateCommand();
                getHabitsCommand.CommandText =
                    @$"
            SELECT
                h.Id,
                h.Name,
                h.Unit,
                h.CreationDate,
                COUNT(he.Id) as TotalEntries
            FROM {habitsTableName} h
            LEFT JOIN habit_entries he ON h.Id = he.HabitId
            GROUP BY h.Id, h.Name, h.Unit, h.CreationDate
            ORDER BY h.Name";

                using (var reader = getHabitsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var habit = new Habit
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Unit = reader.GetString("Unit"),
                            CreationDate = DateTime.Parse(reader.GetString("CreationDate")),
                            TotalEntries = reader.GetInt32("TotalEntries"),
                        };
                        habits.Add(habit);
                    }
                }

                connection.Close();
            }
            return habits;
        }

        internal static List<HabitEntry> GetHabitEntriesForDate(DateTime date, List<Habit> habits)
        {
            var habitEntries = new List<HabitEntry>();

            if (!habits.Any())
                return habitEntries;

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                foreach (var habit in habits)
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @$"
                SELECT
                    Id,
                    HabitId,
                    Quantity,
                    Date
                FROM {habitsEntriesTableName}
                WHERE Date = @date
                AND HabitId = @habitId
                LIMIT 1";

                    command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@habitId", habit.Id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            habitEntries.Add(
                                new HabitEntry
                                {
                                    Id = reader.GetInt32("Id"),
                                    HabitId = reader.GetInt32("HabitId"),
                                    Quantity = (double)reader.GetDecimal("Quantity"),
                                    Date = DateTime.Parse(reader.GetString("Date")),
                                }
                            );
                        }
                        else
                        {
                            habitEntries.Add(
                                new HabitEntry
                                {
                                    Id = 0,
                                    HabitId = habit.Id,
                                    Quantity = 0,
                                    Date = DateTime.Parse(date.ToString("yyyy-MM-dd")),
                                }
                            );
                        }
                    }
                }

                connection.Close();
            }
            return habitEntries;
        }

        internal static List<(string HabitName, int EntryCount)> GetAllHabitEntries()
        {
            var habitCounts = new List<(string, int)>();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
            SELECT h.Name, COUNT(he.Id) as EntryCount
            FROM {habitsTableName} h
            LEFT JOIN {habitsEntriesTableName} he ON h.Id = he.HabitId
            GROUP BY h.Id, h.Name
            ORDER BY h.Name";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        habitCounts.Add((reader.GetString("Name"), reader.GetInt32("EntryCount")));
                    }
                }

                connection.Close();
            }
            return habitCounts;
        }

        internal static bool HabitExists(string name)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @$"SELECT COUNT(*) FROM {habitsTableName} WHERE Name = @name";
                command.Parameters.AddWithValue("@name", name);

                var count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        internal static void HabitEntryExists(HabitEntry newEntry)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var checkCmd = connection.CreateCommand();

                checkCmd.CommandText =
                    @$"
            SELECT COUNT(*)
            FROM {habitsEntriesTableName}
            WHERE HabitId = @habitId AND Date = @date";
                checkCmd.Parameters.AddWithValue("@habitId", newEntry.HabitId);
                checkCmd.Parameters.AddWithValue("@date", newEntry.DisplayDate);

                var existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (existingCount > 0)
                {
                    throw new InvalidOperationException(
                        $"There is already a record for this habit on that date {newEntry.DisplayDate}. "
                    );
                }
            }
        }

        internal static HabitStat CalculateHabitStatistics(
            Habit habit,
            DateTime startDate,
            DateTime endDate
        )
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
            SELECT
                COUNT(he.Id) as TotalEntries,
                COALESCE(SUM(he.Quantity), 0) as TotalQuantity,
                COUNT(DISTINCT he.Date) as DaysTracked,
                COALESCE(MAX(he.Quantity), 0) as BestDayQuantity,
                (SELECT Date FROM habit_entries
                 WHERE HabitId = @habitId AND Quantity = (SELECT MAX(Quantity) FROM habit_entries WHERE HabitId = @habitId AND Date BETWEEN @startDate AND @endDate)
                 LIMIT 1) as BestDayDate,
                COALESCE(MIN(he.Quantity), 0) as WorstDayQuantity,
                (SELECT Date FROM habit_entries
                 WHERE HabitId = @habitId AND Quantity = (SELECT MIN(Quantity) FROM habit_entries WHERE HabitId = @habitId AND Quantity > 0 AND Date BETWEEN @startDate AND @endDate)
                 LIMIT 1) as WorstDayDate
            FROM {habitsTableName} h
            LEFT JOIN {habitsEntriesTableName} he ON h.Id = he.HabitId
                AND he.Date BETWEEN @startDate AND @endDate
            WHERE h.Id = @habitId
            GROUP BY h.Id";

                command.Parameters.AddWithValue("@habitId", habit.Id);
                command.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (habit == null)
                            return null;

                        var totalDays = (endDate - startDate).Days + 1;
                        var daysTracked = reader.GetInt32("DaysTracked");
                        var consistencyRate =
                            totalDays > 0 ? (decimal)daysTracked / totalDays * 100 : 0;
                        var totalQuantity = reader.GetDecimal("TotalQuantity");
                        var averagePerDay = totalDays > 0 ? totalQuantity / totalDays : 0;

                        return new HabitStat
                        {
                            Habit = habit,
                            TotalEntries = reader.GetInt32("TotalEntries"),
                            TotalQuantity = totalQuantity,
                            AveragePerDay = Math.Round(averagePerDay, 2),
                            DaysTracked = daysTracked,
                            ConsistencyRate = Math.Round(consistencyRate, 1),
                            BestDayQuantity = reader.GetDecimal("BestDayQuantity"),
                            BestDayDate = FormatDatabaseDate(reader, "BestDayDate"),
                            WorstDayQuantity = reader.GetDecimal("WorstDayQuantity"),
                            WorstDayDate = FormatDatabaseDate(reader, "WorstDayDate"),
                        };
                    }
                }

                connection.Close();
            }

            return null;
        }

        private static string FormatDatabaseDate(SqliteDataReader reader, string columnName)
        {
            if (reader.IsDBNull(columnName))
                return "N/A";

            var dateString = reader.GetString(columnName);

            string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy", "yyyy/MM/dd" };

            if (
                DateTime.TryParseExact(
                    dateString,
                    formats,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime date
                )
            )
            {
                return date.ToString("dd-MM-yyyy");
            }

            return dateString;
        }
    }
}
