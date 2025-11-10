using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker
{
    internal static class Enums
    {
       internal enum MenuAction
        {
           ViewAllRecords,
           InsertRecord,
           DeleteRecord,
           UpdateRecord,
           Exit

        }

        internal enum InsertDateMode
        {
            Today,
            Other
        }

    }
}
