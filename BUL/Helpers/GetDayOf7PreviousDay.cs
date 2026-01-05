using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class GetDayOf7PreviousDay
    {
        public static DateTime GetDateOf7PreviousDay(DateTime? date)
        {
            if (date == null || !date.HasValue)
            {
                throw new ArgumentException("Ngày không hợp lệ");
            }
            Console.WriteLine($"date: {date}");


            DateTime dateOf7PreviousDay = date.Value.AddDays(-7);
            Console.WriteLine($"dateOf7PreviousDay: {dateOf7PreviousDay}");
            return dateOf7PreviousDay;
        }
    }
}
