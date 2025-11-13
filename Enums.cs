using System.ComponentModel;

namespace HabitTracker
{
    internal static class Enums
    {
        internal enum MenuAction
        {
            [Description("Log New Entry")]
            LogEntry,

            [Description("Manage Habits")]
            ManageHabits,

            [Description("View Reports")]
            Reports,

            [Description("Exit Application")]
            Exit,
        }

        internal enum InsertDateMode
        {
            [Description("Today's Date")]
            Today,

            [Description("Select Different Date")]
            Other,
        }

        internal enum HabitsMenuAction
        {
            [Description("Add New Habit")]
            AddNewHabit,

            [Description("Update Existing Habit")]
            UpdateHabit,

            [Description("Delete Habit")]
            DeleteHabit,

            [Description("Go Back to Main Menu")]
            GoBack,
        }

        internal enum ReportsMenuAction
        {
            [Description("Daily Resume")]
            DailyResume,

            [Description("Habit Analytics")]
            HabitAnalytics,

            [Description("Go Back to Main Menu")]
            GoBack,
        }

        internal enum ResumeDateMenuAction
        {
            [Description("Today")]
            Today,

            [Description("Yesterday")]
            Yesterday,

            [Description("Two Days Ago")]
            TwoDaysAgo,

            [Description("Select Specific Day")]
            SelectDay,

            [Description("Go Back to Reports")]
            GoBack,
        }

        internal enum ResumeHabitsAction
        {
            [Description("All Habits")]
            AllHabits,

            [Description("Select Multiple Habits")]
            HabitSelection,

            [Description("Single Habit")]
            OneHabit,

            [Description("Go Back to Date Selection")]
            GoBack,
        }

        internal enum AnalyticsDateRange
        {
            [Description("Last 7 Days")]
            Last7Days,

            [Description("Last 14 Days")]
            Last14Days,

            [Description("Last 30 Days")]
            Last30Days,

            [Description("This Month")]
            ThisMonth,

            [Description("Last Month")]
            LastMonth,

            [Description("Custom Date Range")]
            CustomRange,

            [Description("Go Back to Analytics")]
            GoBack,
        }
    }
}
