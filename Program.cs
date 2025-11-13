using HabitTracker.Database;
using HabitTracker.UI;

SeedData seedData = new();
seedData.initDatabase();

UserInterface ui = new UserInterface();
ui.MainMenu();
