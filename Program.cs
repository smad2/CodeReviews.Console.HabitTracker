// See https://aka.ms/new-console-template for more information

using HabitTracker.Database;
using HabitTracker.UI;

SeedData seedData = new();
seedData.initDatabase();


UserInterface ui = new UserInterface();
ui.MainMenu();
