using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Models
{
    internal class HabitEntry
    {

        internal int Id { get; set; }

        internal int HabitId { get; set; }

        internal double Quantity { get; set; }

        internal DateTime Date { get; set; }

        internal string DisplayDate => Date.ToString("yyyy-MM-dd");

        internal HabitEntry()
        {

        }

        internal HabitEntry(Habit habit, DateTime dateTime)
        {
            HabitId = habit.Id;
            Date = dateTime;
        }

        internal HabitEntry(Habit habit, int quantity, DateTime dateTime)
        {
            HabitId = habit.Id;
            Quantity = quantity;
            Date = dateTime;
        }

    }
}
