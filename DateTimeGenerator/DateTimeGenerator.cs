using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facker;

namespace DateTimeGenerator
{
    public class DateTimeGenerator : Generator<DateTime>
    {
        public override DateTime ObjectGeneration(Random random)
        {
           
            int Year = random.Next(1, 9999);
            int Month = random.Next(1, 12);
            int Day = random.Next(1, 28);
            int Hour = random.Next(0, 23);
            int Minute = random.Next(0, 59);
            int Second = random.Next(0, 59);

            DateTime result = new DateTime(Year, Month, Day, Hour, Minute, Second);
            return result;
        }
    }
}
