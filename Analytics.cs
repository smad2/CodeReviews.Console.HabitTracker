using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitTracker.Models;
using static HabitTracker.Database.HabitTrackerDatabase;


namespace HabitTracker
{
    internal class Analytics
    {

        internal static AnalyticsResult GetHabitAnalytics(DateTime startDate, DateTime endDate, List<Habit> habits)
        {
            var result = new AnalyticsResult
            {
                DateRange = $"{startDate:dd-MM-yyyy} to {endDate:dd-MM-yyyy}",
                TotalDays = (endDate - startDate).Days + 1
            };

            var habitStats = new List<HabitStat>();

            foreach (var habit in habits)
            {
                var stats = CalculateHabitStatistics(habit, startDate, endDate);
                if (stats != null)
                {
                    habitStats.Add(stats);
                }
            }

            result.HabitStats = habitStats;
            CalculateSummaryStatistics(result);

            return result;
        }

        internal static void CalculateSummaryStatistics(AnalyticsResult result)
        {
            if (!result.HabitStats.Any()) return;

            result.MostConsistentHabit = result.HabitStats
                .OrderByDescending(h => h.ConsistencyRate)
                .First();

            result.MostActiveHabit = result.HabitStats
                .OrderByDescending(h => h.TotalQuantity)
                .First();

        }
    }
}
