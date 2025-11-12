using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Models
{
    internal class HabitStat
    {
        internal Habit Habit { get; set; }
        internal int TotalEntries { get; set; }
        internal decimal TotalQuantity { get; set; }
        internal decimal AveragePerDay { get; set; }
        internal decimal AveragePerEntry { get; set; }
        internal int DaysTracked { get; set; }
        internal decimal ConsistencyRate { get; set; }
        internal decimal BestDayQuantity { get; set; }
        internal string BestDayDate { get; set; }
        internal decimal WorstDayQuantity { get; set; }
        internal string WorstDayDate { get; set; }
    }
}
