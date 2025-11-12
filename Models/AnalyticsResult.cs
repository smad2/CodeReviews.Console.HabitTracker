using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Models
{
    internal class AnalyticsResult
    {
        internal string DateRange { get; set; }
        internal int TotalDays { get; set; }
        internal List<HabitStat> HabitStats { get; set; } = new();
        internal HabitStat MostConsistentHabit { get; set; }
        internal HabitStat MostActiveHabit { get; set; }
    }

}
