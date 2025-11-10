using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitTracker.Database;
using Spectre.Console;
using static HabitTracker.Enums;

namespace HabitTracker
{
    internal class UserInterface
    {
        private HabitTrackerDatabase _db { get; set; }

        public UserInterface(HabitTrackerDatabase database)
        {
            _db = database;
        }

        internal void MainMenu()
        {
            bool exitApp = false;
            while (!exitApp)
            {
                displayHeader();

                var selectedAction = AnsiConsole.Prompt(
                    new SelectionPrompt<MenuAction>()
                        .Title("What would you like to do?")
                        .AddChoices(Enum.GetValues<MenuAction>())
                );

                switch (selectedAction)
                {
                    case MenuAction.InsertRecord:
                        InsertRecord();
                        break;
                    case MenuAction.DeleteRecord:
                        break;
                    case MenuAction.UpdateRecord:
                        break;
                    case MenuAction.ViewAllRecords:
                        break;
                    case MenuAction.Exit:
                        exitApp = true;
                        break;
                }
            }
        }

        internal void InsertRecord()
        {
            var selectedAction = AnsiConsole.Prompt(
                new SelectionPrompt<InsertDateMode>()
                    .Title("Insert date:")
                    .AddChoices(Enum.GetValues<InsertDateMode>())
            );

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

            int quantity = PromptInt("Please enter a number:");
            _db.Insert("drinking_water", date.ToString("d-MM-yy"), quantity);
        }

        private void displayHeader()
        {
            Console.Clear();

            AnsiConsole.Write(new FigletText("HabitTracker").Centered().Color(Color.Red));
        }

        public DateTime PromptDate()
        {
            string userInput = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Please enter date (dd-MM-yyyy):[/]")
                    .ValidationErrorMessage("[red]Invalid date format![/]")
                    .Validate(input =>
                    {
                        string[] formats = { "dd-MM-yyyy", "d-MM-yy", "dd-MM-yy", "d-MM-yyyy" };

                        if (DateTime.TryParseExact(input, formats,
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out _))
                        {
                            return ValidationResult.Success();
                        }
                        return ValidationResult.Error("[red]Use formats like: 25-12-2024 or 5-9-24[/]");
                    })
            );

            string[] formats = { "dd-MM-yyyy", "d-MM-yy", "dd-MM-yy", "d-MM-yyyy" };
            return DateTime.ParseExact(userInput, formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None);
        }

        public int PromptInt(string message)
        {
            int userAnswer = AnsiConsole.Prompt(new TextPrompt<int>($"[yellow]{message}[/]"));
            return userAnswer;
        }
    }
}
