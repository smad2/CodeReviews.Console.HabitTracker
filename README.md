# Habit Tracker 🎯

Habit Tracker is a small console application to register and inspect the tracking of healthy habits. It is written in C# and uses a local SQLite database to persist habits and their daily entries. The application uses Spectre.Console to provide a friendly console UI with tables, prompts and charts.

## 📋 Quick summary

- Language: C# (net9.0)
- Database: SQLite (file: `habit-Tracker.db` in the app folder)
- UI: Spectre.Console for interactive prompts, tables and charts

## ✨ Features

- Create, update and delete habits (each habit has a name and a unit, e.g. "Push-ups", "reps").
- Log daily entries for a habit (quantity + date).
- Prevent duplicate entries for the same habit and date.
- Visual reports: daily resume (bar chart), overall entries chart, and analytics for a date range (totals, averages, best/worst days).

## 🚀 How it works (high-level)

1. On start, the program runs the seed data initializer (`SeedData`) to ensure the database and some sample records exist.
2. The `UserInterface` class drives the interactive menus: main menu, habits management, logging entries and reports.
3. The `HabitTrackerDatabase` static class encapsulates all database operations (create/read/update/delete) using `Microsoft.Data.Sqlite`.
4. `Analytics` computes aggregated statistics for a date range by calling database helper methods.
5. Data is stored locally in `habit-Tracker.db` (SQLite). All SQL and reading/writing is done with parameterized commands to avoid SQL injection and keep code simple.

## 📊 Reporting Features

### 📅 Daily Resume

* **Visual bar chart** showing habit completion for a specific day
* **Quick overview** of all habits at a glance
* **Color-coded** bars with random colors for better visualization
* **Zero-entry handling** shows habits that weren't tracked that day

### 📈 Habit Analytics

* **Comprehensive statistics** for any date range (last 7, 14, 30 days, custom ranges)
* **Key metrics** : Total entries, average per day, consistency rate, best/worst days
* **Performance insights** : Most consistent habit, most active habit
* **Trend analysis** : Track progress over time with detailed breakdowns

The analytics system calculates:

* **Total Quantity** : Sum of all entries in the period
* **Average Per Day** : Total quantity divided by all days in period (includes zero days)
* **Consistency Rate** : Percentage of days the habit was tracked
* **Best/Worst Days** : Peak performance and challenging days


## 🗂️ Project structure and what each file/folder does

* `Program.cs` — application entry point. It initializes seed data and starts the `UserInterface.MainMenu()` loop.
* `HabitTracker.csproj` — .NET project file. Shows the target framework (net9.0) and package references (Microsoft.Data.Sqlite, Spectre.Console).
* `Database/`
  * `HabitTrackerDatabase.cs` — main database helper. Responsible for:
    * Creating, reading, updating and deleting habits and entries.
    * Queries used by reporting and analytics (e.g. GetAllHabits, GetHabitEntriesForDate, CalculateHabitStatistics).
    * Preventing duplicate daily entries via `HabitEntryExists`.
  * `SeedData.cs` — populates the database with initial sample habits and entries when the app first runs (ensures a minimal dataset for demos).
    * Also includes randomized entries with an 80% daily probability to simulate realistic habit tracking.
* `Models/`
  * `Habit.cs` — simple model for a habit (Id, Name, Unit, CreationDate, TotalEntries). Also has convenience property `DisplayCreationDate`.
  * `HabitEntry.cs` — model for a single tracked entry (Id, HabitId, Quantity, Date).
  * `HabitStat.cs` — aggregated statistics used by analytics (totals, averages, best/worst days, consistency rate).
  * `DateRange.cs` / other small models — small DTOs used to pass date range or analytics results (if present).
* `UI/`
  * `UserInterface.cs` — implements the menus and all user interaction flows (Main menu, Habits menu, Reports menu, insert/update/delete flows).
  * `UIHelpers.cs`
    — helper functions for the UI: common prompts, validation, formatted
    displays, and chart rendering. Uses Spectre.Console primitives like `SelectionPrompt`, `Table`, `BarChart`, `Rule`, etc.
* `Analytics.cs` — orchestrates analytics generation. Calls `HabitTrackerDatabase.CalculateHabitStatistics` for each selected habit in the given date range and computes summary metrics (most consistent habit, most active habit).
* `Enums.cs` — enums that represent menu options and other selection types. Each enum member is annotated with a `Description` attribute used by the UI to show friendly text.
* `Properties/launchSettings.json` — configuration used by the IDE when launching locally (not required to run from command line).

## 💾 Data file

The app uses a local SQLite file named `habit-Tracker.db`
 located in the application's working directory. If it does not exist,
the seed initializer will create tables and insert sample data.

## 🛠️ How to build and run

Open
 a PowerShell terminal and run the following commands from the solution
folder (or use the absolute path to the project file):

**powershell**

```
# From the repository root (where HabitTracker.sln and HabitTracker.csproj live)
dotnet build "c:\Users\Gaizkattore\Desktop\DEVELOPING COMO DEVELOPANDAS\DotNet\C#Academy\CodeReviews.Console.HabitTracker\HabitTracker.csproj"

# Run the app
dotnet run --project "c:\Users\Gaizkattore\Desktop\DEVELOPING COMO DEVELOPANDAS\DotNet\C#Academy\CodeReviews.Console.HabitTracker\HabitTracker.csproj"
```

When
 the app runs it will present a textual menu. Use the keyboard to select
 actions (the UI uses Spectre.Console selection prompts and
multi-selection prompts where relevant).

## 👤 Typical user flow

1. Start the app. The main menu shows options: Log New Entry, Manage Habits, View Reports, Exit.
2. To log an entry: choose the habit, pick today's date or a custom date, and enter the quantity for that habit's unit.
3. To
   manage habits: you can add a new habit (name + unit), update an
   existing one, or delete a habit (deleting also removes its entries).
4. To
   view reports: choose Daily Resume (select a date or range of habits) or
   Habit Analytics (choose a range and habits). Analytics shows totals,
   daily averages, tracked days, best/worst days and a short summary.

## ⚠️ Important behaviors and edge cases

* Duplicate prevention: the database helper throws an exception if an entry already exists for the same habit and date.
* Date validation: UI enforces date formats like `dd-MM-yyyy` and prevents selecting future dates.
* Empty state: if no habits exist, the UI shows a helpful message and prevents actions that require habits.
* Simple
  concurrency: the app uses a single SQLite file with short-lived
  connections; it is intended as a single-user local console app.

## 📦 Dependencies

* Microsoft.Data.Sqlite — low-level [ADO.NET](https://ADO.NET) provider used to interact with the SQLite database.
* Spectre.Console — used to create a nicer console UI (tables, prompts, charts, styles).

## 🔮 Extensions / Next steps

* Add unit tests for database helpers and analytics.
* Add migrations or a small schema check to make table creation explicit.
* Add export (CSV) for analytics results.
* Improve input validation and error messages for corner cases.

## 📄 License

This repository does not include a license file. Add one if you plan to publish or share.


## Made with ❤️ by Salva.
