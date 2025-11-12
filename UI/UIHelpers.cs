using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using HabitTracker.Database;
using HabitTracker.Models;
using Spectre.Console;
using static HabitTracker.Database.HabitTrackerDatabase;
using static HabitTracker.Enums;

namespace HabitTracker.UI
{
    internal static class UIHelpers
    {
        private static readonly Color[] availableColorsBarChart = new[]
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Yellow,
            Color.Purple,
            Color.Orange1,
            Color.Cyan1,
            Color.Magenta1,
            Color.Gold1,
            Color.DarkSeaGreen1,
            Color.LightSkyBlue1,
            Color.Pink1,
            Color.LightGreen,
            Color.DarkCyan,
            Color.LightSalmon1,
            Color.Lime,
            Color.DarkBlue,
            Color.Olive,
            Color.DarkOrange,
            Color.MistyRose1,
        };

        public static void DisplayHeader()
        {
            Console.Clear();
            AnsiConsole.Write(new FigletText("HabitTracker").Centered().Color(Color.Red));
        }

        public static void DisplayRule(string message)
        {
            var rule = new Rule($"[darkseagreen1]{message}[/]").Centered();
            rule.Style = Style.Parse("red dim");
            AnsiConsole.Write(rule);
        }

        public static void DisplayError(string message)
        {
            AnsiConsole.MarkupLine($"[red]{message}[/]");
        }

        public static void DisplaySuccess(string message)
        {
            AnsiConsole.MarkupLine($"[green]{message}[/]");
        }

        public static void DisplayInfo(string message)
        {
            AnsiConsole.MarkupLine($"[yellow]{message}[/]");
        }

        public static void DisplayMainMenu()
        {
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey();
        }

        public static bool DisplayConfirmation(string message)
        {
            return AnsiConsole.Confirm($"[yellow]{message}[/]");
        }

        public static T PromptSelection<T>(string title)
            where T : struct, Enum
        {
            var prompt = new SelectionPrompt<T>()
                .Title(title)
                .AddChoices(Enum.GetValues<T>())
                .UseConverter(e => FormatEnumValue(e));

            return AnsiConsole.Prompt(prompt);
        }

        private static string GetEnumDescription(Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            var descriptionAttribute =
                field?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault()
                as DescriptionAttribute;

            return descriptionAttribute?.Description ?? enumValue.ToString();
        }

        public static List<Habit> PromptMultipleHabitsSelection()
        {
            var habits = GetAllHabits();
            var prompt = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Habit>()
                    .Title("[yellow]Select habits: [/]")
                    .Required() //
                    .PageSize(10)
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a habit, "
                            + "[green]<enter>[/] to accept)[/]"
                    )
                    .AddChoices(habits)
                    .UseConverter(h => h.Name)
            );
            return prompt;
        }

        private static string FormatEnumValue(Enum enumValue)
        {
            var description = GetEnumDescription(enumValue);

            var enumName = enumValue.ToString().ToLower();

            if (enumName.Contains("goback") || enumName.Contains("cancel"))
                return $"[yellow]{description}[/]";

            if (enumName.Contains("exit") || enumName.Contains("delete"))
                return $"[red]{description}[/]";
            return description;
        }

        public static Habit? PromptHabitsSelection()
        {
            var habits = GetAllHabits();

            if (!habits.Any())
            {
                DisplayError("No habits available.");
                return null;
            }

            var choices = habits.ToList();
            choices.Add(new Habit { Id = -1, Name = "Go Back" });

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<Habit>()
                    .Title("Select Habit:")
                    .AddChoices(choices)
                    .UseConverter(h => h.Id == -1 ? "[yellow]Go Back[/]" : h.Name)
            );

            return selected.Id == -1 ? null : selected;
        }

        public static List<Habit> PromptHabitsMenuSelection()
        {
            var habitSelection = PromptSelection<ResumeHabitsAction>("Select Habit/s:");
            List<Habit> habitSelectionList = new();

            switch (habitSelection)
            {
                case ResumeHabitsAction.AllHabits:
                    habitSelectionList = GetAllHabits();
                    break;
                case ResumeHabitsAction.HabitSelection:
                    habitSelectionList = PromptMultipleHabitsSelection();
                    break;
                case ResumeHabitsAction.OneHabit:
                    var singleHabit = PromptHabitsSelection();
                    if (singleHabit != null)
                        habitSelectionList.Add(singleHabit);
                    break;
                case ResumeHabitsAction.GoBack:
                    return null;
            }

            return habitSelectionList;
        }

        public static DateTime PromptDate()
        {
            string userInput = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Please enter date (dd-MM-yyyy):[/]")
                    .ValidationErrorMessage("[red]Invalid date format![/]")
                    .Validate(input =>
                    {
                        string[] formats = { "dd-MM-yyyy", "d-MM-yy", "dd-MM-yy", "d-MM-yyyy" };
                        DateTime validDate = new();

                        if (
                            DateTime.TryParseExact(
                                input,
                                formats,
                                System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None,
                                out validDate
                            )
                        )
                        {
                            if (validDate <= DateTime.Today)
                            {
                                return ValidationResult.Success();
                            }
                            else
                            {
                                return ValidationResult.Error(
                                    "[red]Date cannot be greater than today[/]"
                                );
                            }
                        }
                        return ValidationResult.Error(
                            "[red]Use formats like: 25-12-2024 or 5-9-24[/]"
                        );
                    })
            );

            string[] formats = { "dd-MM-yyyy", "d-MM-yy", "dd-MM-yy", "d-MM-yyyy" };
            return DateTime.ParseExact(
                userInput,
                formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None
            );
        }

        public static DateRange PromptDateRangeSelection()
        {
            var rangeType = PromptSelection<AnalyticsDateRange>("Select date range");
            DateRange range = new();

            switch (rangeType)
            {
                case (AnalyticsDateRange.Last7Days):
                    range = new DateRange
                    {
                        StartDate = DateTime.Today.AddDays(-7),
                        EndDate = DateTime.Today,
                        Description = "Last 7 Days",
                    };
                    break;

                case (AnalyticsDateRange.Last14Days):
                    range = new DateRange
                    {
                        StartDate = DateTime.Today.AddDays(-14),
                        EndDate = DateTime.Today,
                        Description = "Last 14 Days",
                    };
                    break;
                case (AnalyticsDateRange.Last30Days):
                    range = new DateRange
                    {
                        StartDate = DateTime.Today.AddDays(-30),
                        EndDate = DateTime.Today,
                        Description = "Last 30 Days",
                    };
                    break;

                case (AnalyticsDateRange.ThisMonth):
                    range = new DateRange
                    {
                        StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                        EndDate = DateTime.Today,
                        Description = "This Month",
                    };
                    break;

                case (AnalyticsDateRange.LastMonth):
                    range = new DateRange
                    {
                        StartDate = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            1
                        ).AddMonths(-1),
                        EndDate = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            1
                        ).AddDays(-1),
                        Description = "Last Month",
                    };
                    break;
                case (AnalyticsDateRange.CustomRange):
                    range = PromptCustomDateRange();
                    break;
            }

            return range;
        }

        private static DateRange PromptCustomDateRange()
        {
            DisplayInfo("Select start date:");
            var startDate = PromptDate();

            DisplayInfo("Select end date:");
            var endDate = PromptDate();

            if (endDate < startDate)
            {
                DisplayError("End date cannot be before start date");
                return PromptCustomDateRange();
            }

            return new DateRange
            {
                StartDate = startDate,
                EndDate = endDate,
                Description = $"{startDate:dd-MM-yy} to {endDate:dd-MM-yy}",
            };
        }

        public static int PromptInt(string message)
        {
            return AnsiConsole.Prompt(new TextPrompt<int>($"[yellow]{message}[/]"));
        }

        public static string PromptValidHabitName(string message = "Enter habit name:")
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>(message).Validate(name =>
                {
                    var trimmedName = name.Trim();

                    if (string.IsNullOrWhiteSpace(trimmedName))
                        return ValidationResult.Error("Name cannot be empty");

                    if (HabitExists(trimmedName))
                        return ValidationResult.Error("Habit already exists");

                    return ValidationResult.Success();
                })
            );
        }

        public static string PromptValidUnitName(string message = "Enter unit of measurement:")
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>(message).Validate(name =>
                {
                    var trimmedName = name.Trim();

                    if (string.IsNullOrWhiteSpace(trimmedName))
                        return ValidationResult.Error("Name cannot be empty");

                    return ValidationResult.Success();
                })
            );
        }

        private static string PascalCaseToWords(string pascalCaseText)
        {
            Regex r = new Regex(@"(?!^)(?=[A-Z])");
            return r.Replace(pascalCaseText, " ");
        }

        public static void DisplayHabitsTable()
        {
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
        }

        public static void DisplayBarChart(List<Habit> habits, List<HabitEntry> habitEntries)
        {
            var barChart = new BarChart().Width(70);

            var random = new Random();
            int colorCount = 0;

            foreach (var entry in habitEntries)
            {
                var habit = habits.Find(h => h.Id == entry.HabitId);
                if (habit != null)
                {
                    if (colorCount == availableColorsBarChart.Length)
                        colorCount = 0;
                    var selectedColor = availableColorsBarChart[colorCount];
                    colorCount++;
                    barChart.AddItem($"{habit.Name} ({habit.Unit})", entry.Quantity, selectedColor);
                }
            }
            AnsiConsole.Write(new Align(barChart, HorizontalAlignment.Center));
        }

        public static void DisplayEntriesCountBarChart(
            List<(string HabitName, int EntryCount)> habitEntriesCount
        )
        {
            var barChart = new BarChart().Width(70);

            var random = new Random();
            int colorCount = 0;

            foreach (var habitEntryCount in habitEntriesCount)
            {
                if (colorCount == availableColorsBarChart.Length)
                    colorCount = 0;
                var selectedColor = availableColorsBarChart[colorCount];
                colorCount++;

                barChart.AddItem(
                    habitEntryCount.HabitName,
                    habitEntryCount.EntryCount,
                    selectedColor
                );
            }
            AnsiConsole.Write(new Align(barChart, HorizontalAlignment.Center));
        }
    }
}
