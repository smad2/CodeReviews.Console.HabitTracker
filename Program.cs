// See https://aka.ms/new-console-template for more information

using HabitTracker;
using HabitTracker.Database;

HabitTrackerDatabase db = new HabitTrackerDatabase();
UserInterface ui = new UserInterface(db);
ui.MainMenu();
