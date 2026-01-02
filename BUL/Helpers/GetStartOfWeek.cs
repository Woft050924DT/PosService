using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class GetStartOf
    {
        public static DateTime convertWeek(int year, int week)
        {
            if (week < 1 || week > 53)
                throw new ArgumentOutOfRangeException(nameof(week));

            return ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        }
        public static DateTime convertMonth( int month)
        {
            int year = DateTime.Now.Year;
            return new DateTime(year, month, 1);
        }

    }

}
