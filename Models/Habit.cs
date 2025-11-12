using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Models
{
    internal class Habit
    {
        internal int Id { get; set; }
        internal string Name { get; set; }

        internal string Unit { get; set; }

        internal DateTime CreationDate { get; set; }

        internal string DisplayCreationDate => CreationDate.ToString("yyyy-MM-dd");

        internal int TotalEntries { get; set; }

        internal Habit()
        {

        }

        internal Habit(string name, string unit, DateTime creationDate)
        {
            Name = name;
            Unit = unit;
            CreationDate = creationDate;
        }

    }
}
