using System;

namespace WdCal
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IWorkdayCalendar calendar = new WorkdayCalendar();

            calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
            calendar.SetRecurringHoliday(5, 17);
            calendar.SetHoliday(new DateTime(2004, 5, 27));
            const string format = "dd-MM-yyyy HH:mm";

            // ex 1
            //var start = new DateTime(2004, 5, 24, 18, 5, 0);
            //decimal increment = -5.5m;

            //ex 2
            //var start = new DateTime(2004, 5, 24, 19, 3, 0);
            //decimal increment = 44.723656m;

            //ex 3
            //var start = new DateTime(2004, 5, 24, 18, 3, 0);
            //decimal increment = -6.7470217m;

            //ex 4
            //var start = new DateTime(2004, 5, 24, 8, 3, 0);
            //decimal increment = 12.782709m;

            //ex 5
            var start = new DateTime(2004, 5, 24, 7, 3, 0);
            const decimal increment = 8.276628m;

            //ex 6
            //var start = new DateTime(2004, 5, 24, 12, 3, 0);
            //decimal increment = -1.25m;

            var incrementedDate = calendar.GetWorkdayIncrement(start, increment);
            Console.WriteLine(start.ToString(format) +
                              " with an addition of " +
                              increment +
                              " work days is " +
                              incrementedDate.ToString(format));

            Console.WriteLine("Please type any key to exit");
            Console.ReadKey();
        }
    }
}