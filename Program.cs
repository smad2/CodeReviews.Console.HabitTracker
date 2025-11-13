using HabitTracker.Database;
using HabitTracker.UI;

SeedData seedData = new();
seedData.InitDatabase();

UserInterface ui = new UserInterface();
ui.MainMenu();
