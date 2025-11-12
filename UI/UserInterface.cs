using System;
using System.Linq;
using System.Text.RegularExpressions;
using HabitTracker.Database;
using HabitTracker.Models;
using Spectre.Console;
using static HabitTracker.Database.HabitTrackerDatabase;
using static HabitTracker.Enums;
using static HabitTracker.UI.UIHelpers;

namespace HabitTracker.UI
{
    internal class UserInterface
    {
        internal void MainMenu()
        {
            bool exitApp = false;
            while (!exitApp)
            {
                DisplayHeader();

                var selectedAction = PromptSelection<MenuAction>("Select an action:");

                switch (selectedAction)
                {
                    case MenuAction.LogEntry:
                        InsertNewEntry();
                        break;
                    case MenuAction.ManageHabits:
                        HabitsMenu();
                        break;
                    case MenuAction.Reports:
                        ReportMenu();
                        break;
                    case MenuAction.Exit:
                        exitApp = true;
                        break;
                }
            }
        }

        internal void InsertNewEntry()
        {
            try
            {
                var selectedHabit = PromptHabitsSelection();
                if (selectedHabit == null)
                    return;

                DisplayRule($"Creating new entry in {selectedHabit.Name}");

                var selectedAction = PromptSelection<InsertDateMode>("Insert date:");

                DateTime date = new DateTime();
                switch (selectedAction)
                {
                    case InsertDateMode.Today:
                        date = DateTime.Today;
                        break;
                    case InsertDateMode.Other:
                        date = PromptDate();
                        break;
                }
                HabitEntry newEntry = new(selectedHabit, date);

                HabitEntryExists(newEntry);
                int quantity = PromptInt($"Please enter {selectedHabit.Unit}:");
                newEntry.Quantity = quantity;

                InsertEntry(newEntry);
                DisplaySuccess("Entry added successfully!");
                DisplayMainMenu();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
                DisplayMainMenu();
            }
        }

        internal void InsertNewHabit()
        {
            try
            {
                DisplayRule($"Creating new habit");

                string name = PromptValidHabitName();
                string unit = PromptValidUnitName();

                var newHabit = new Habit(name, unit, DateTime.Now);

                var confirmation = DisplayConfirmation(
                    $"Create new habit: [yellow]'{name}'[/]\n"
                        + $"With unit: [cyan]{unit}[/]\n"
                        + $"This habit will be available for tracking immediately."
                );
                if (!confirmation)
                    return;

                InsertHabit(newHabit);
                DisplaySuccess("Habit added successfully!");
                DisplayMainMenu();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        internal void UpdateHabit()
        {
            try
            {
                var selectedHabit = PromptHabitsSelection();

                if (selectedHabit == null)
                    return;

                DisplayRule($"Updating {selectedHabit.Name} habit.");

                string habitName = PromptValidHabitName(
                    $"[yellow]Current name is {selectedHabit.Name}.[/]\nEnter new name: "
                );
                string unitName = PromptValidUnitName(
                    $"[yellow]Current unit is {selectedHabit.Unit}.[/]\nEnter new unit: "
                );

                var confirmation = DisplayConfirmation(
                    $"Update habit to '[yellow]{habitName}[/]' with unit '[cyan]{unitName}[/]'?"
                );
                if (!confirmation)
                    return;
                HabitTrackerDatabase.UpdateHabit(selectedHabit.Id, habitName, unitName);

                DisplaySuccess("Habit updated successfully!");
                DisplayMainMenu();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        internal void DeleteHabit()
        {
            try
            {
                DisplayRule($"Delete habit");

                var selectedHabit = PromptHabitsSelection();
                if (selectedHabit == null)
                    return;

                var confirmation = DisplayConfirmation(
                    $"Are you sure you want to delete [yellow]'{selectedHabit.Name}'[/]?\n"
                        + $"This will permanently delete [red]{selectedHabit.TotalEntries} entries[/] and cannot be undone!"
                );

                if (!confirmation)
                    return;

                HabitTrackerDatabase.DeleteHabit(selectedHabit.Id);

                DisplaySuccess("Habit deleted successfully!");

                DisplayMainMenu();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        internal void HabitsMenu()
        {
            bool exitHabitsMenu = false;

            while (!exitHabitsMenu)
            {
                Console.Clear();
                var table = new Table().Centered();
                table.Border(TableBorder.Rounded);
                table.Title("[blue]Habits[/]");
                table.AddColumn(new TableColumn("[yellow]Name[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Unit[/]").Centered());
                table.AddColumn(new TableColumn("[cyan]Creation Date[/]").Centered());
                table.AddColumn(new TableColumn("[green]Entries[/]").Centered());

                var habits = GetAllHabits();

                foreach (var habit in habits)
                {
                    table.AddRow(
                        $"[yellow]{habit.Name}[/]",
                        $"[yellow]{habit.Unit}[/]",
                        $"[cyan]{habit.DisplayCreationDate}[/]",
                        $"[green]{habit.TotalEntries}[/]"
                    );
                }

                AnsiConsole.Write(table);

                var selectedOption = PromptSelection<HabitsMenuAction>("Choose an action:");

                switch (selectedOption)
                {
                    case HabitsMenuAction.AddNewHabit:
                        InsertNewHabit();
                        break;
                    case HabitsMenuAction.UpdateHabit:
                        UpdateHabit();
                        break;
                    case HabitsMenuAction.DeleteHabit:
                        DeleteHabit();
                        break;
                    case HabitsMenuAction.GoBack:
                        exitHabitsMenu = true;
                        break;
                }
            }
        }

        internal void ReportMenu()
        {
            bool exitReportMenu = false;

            while (!exitReportMenu)
            {
                Console.Clear();
                DisplayRule("Total habit entries");
                AnsiConsole.WriteLine();
                var habits = GetAllHabits();
                var habitEntriesCount = GetAllHabitEntries();

                DisplayEntriesCountBarChart(habitEntriesCount);

                AnsiConsole.WriteLine();

                var selectedOption = PromptSelection<ReportsMenuAction>("Choose a report:");

                AnsiConsole.WriteLine();

                switch (selectedOption)
                {
                    case ReportsMenuAction.DailyResume:
                        CreateResume();
                        break;
                    case ReportsMenuAction.HabitAnalytics:
                        AnalyticsMenu();
                        break;
                    case ReportsMenuAction.GoBack:
                        exitReportMenu = true;
                        break;
                }
            }
        }

        internal void CreateResume()
        {
            try
            {
                DisplayRule("Creating Daily Resume");

                var selectedDate = PromptSelection<ResumeDateMenuAction>("Choose day:");
                DateTime date = new();

                switch (selectedDate)
                {
                    case ResumeDateMenuAction.Today:
                        date = DateTime.Today;
                        break;
                    case ResumeDateMenuAction.Yesterday:
                        date = DateTime.Today.AddDays(-1);
                        break;
                    case ResumeDateMenuAction.TwoDaysAgo:
                        date = DateTime.Today.AddDays(-2);
                        break;
                    case ResumeDateMenuAction.SelectDay:
                        date = PromptDate();
                        break;
                }

                var habitSelectionList = PromptHabitsMenuSelection();
                if (habitSelectionList == null)
                    return;

                var habitEntriesResult = GetHabitEntriesForDate(date, habitSelectionList);

                Console.Clear();
                DisplayRule($"Resume of day {date:dd-MM-yyyy}");
                AnsiConsole.WriteLine("");

                DisplayBarChart(habitSelectionList, habitEntriesResult);

                DisplayMainMenu();
            }
            catch (Exception e)
            {
                DisplayMainMenu();
            }
        }

        internal void AnalyticsMenu()
        {
            DisplayRule("Habit Analytics Menu");

            AnsiConsole.WriteLine();

            var dateRange = PromptDateRangeSelection();
            if (dateRange == null)
                return;

            var selectedHabits = PromptHabitsMenuSelection();
            if (selectedHabits == null)
                return;

            var analytics = Analytics.GetHabitAnalytics(
                dateRange.StartDate,
                dateRange.EndDate,
                selectedHabits
            );

            DisplayAnalyticsReport(analytics);

            DisplayMainMenu();
        }

        private void DisplayAnalyticsReport(AnalyticsResult analytics)
        {
            Console.Clear();

            DisplayRule(
                $"HABIT ANALYTICS\n[bold]{analytics.DateRange}[/] ({analytics.TotalDays} days)"
            );

            AnsiConsole.WriteLine();
            var table = new Table().Centered();
            table.Border(TableBorder.Rounded);
            table.AddColumn(new TableColumn("[bold yellow]Habit[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold yellow]Total[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold yellow]Daily average[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold yellow]Tracked days[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold lime]Best day[/]").RightAligned());
            table.AddColumn(new TableColumn("[bold red]Worst day[/]").RightAligned());

            foreach (var stat in analytics.HabitStats)
            {
                table.AddRow(
                    $"{stat.Habit.Name}",
                    $"{stat.TotalQuantity} ({stat.Habit.Unit})",
                    $"{stat.AveragePerDay}",
                    $"{stat.DaysTracked}",
                    $"{stat.BestDayQuantity} {stat.Habit.Unit} ({stat.BestDayDate})",
                    $"{stat.WorstDayQuantity} {stat.Habit.Unit} ({stat.WorstDayDate})"
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            // Resumen general
            DisplayAnalyticsSummary(analytics);
        }

        private void DisplayAnalyticsSummary(AnalyticsResult analytics)
        {
            if (!analytics.HabitStats.Any())
                return;

            var summaryPanel = new Panel(
                new Columns(
                    new Markup(
                        "[bold underline]OVERALL SUMMARY[/]\n"
                            + $"[green]Most Consistent: {analytics.MostConsistentHabit.Habit.Name} ({analytics.MostConsistentHabit.ConsistencyRate}%)[/]\n"
                            + $"[blue]Most Active: {analytics.MostActiveHabit.Habit.Name} ({analytics.MostActiveHabit.TotalQuantity} {analytics.MostActiveHabit.Habit.Unit})[/]\n"
                            + $"[yellow]Total Habits Analyzed: {analytics.HabitStats.Count}[/]"
                    )
                )
            )
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.DarkSeaGreen1),
                Padding = new Padding(2, 1, 2, 1),
                Expand = true,
            };

            AnsiConsole.Write(summaryPanel);
        }
    }
}
