using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class GetStartOf
    {
        public static DateTime convertWeek(int weekNumber)
        {
            int year = DateTime.Now.Year;

            DateTime jan4 = new DateTime(year, 1, 4);

            int daysOffset = DayOfWeek.Monday - jan4.DayOfWeek;
            DateTime firstMonday = jan4.AddDays(daysOffset);

            DateTime result = firstMonday.AddDays((weekNumber - 1) * 7);

            return result;
        }
        public static DateTime convertMonth( int month)
        {
            int year = DateTime.Now.Year;
            return new DateTime(year, month, 1);
        }

    }

}
