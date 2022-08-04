using System;
using System.Collections.Generic;
using WdCal.ExtensionMethods;

namespace WdCal
{
    public class WorkdayCalendar : IWorkdayCalendar
    {
        private readonly List<DateTime> _holidays = new List<DateTime>();
        private readonly List<Tuple<int, int>> _recurringHolidays = new List<Tuple<int, int>>();
        private TimeSpan _workdayStartTime = TimeSpan.Zero;
        private TimeSpan _workdayEndTime = TimeSpan.Zero;

        public DateTime GetWorkdayIncrement(DateTime startDate, decimal incrementInWorkdays)
        {
            var incrementDaysSign = Math.Sign(incrementInWorkdays);
            var incrementInWorkdaysAsPositiveValue = decimal.ToDouble(Math.Abs(incrementInWorkdays));

            return startDate
                .AddFullBusinessDays(incrementDaysSign, incrementInWorkdaysAsPositiveValue, _recurringHolidays, _holidays)
                .CalculateBusinessHours(incrementInWorkdaysAsPositiveValue,  incrementDaysSign, _workdayEndTime, _workdayStartTime);

        }

        public void SetHoliday(DateTime date)
        {
            _holidays.Add(date);
        }

        public void SetRecurringHoliday(int month, int day)
        {
            _recurringHolidays.Add(new Tuple<int, int>(month, day));
        }

        public void SetWorkdayStartAndStop(int startHours, int startMinutes, int stopHours, int stopMinutes)
        {
            const int timeSpanSeconds = 0;
            _workdayStartTime = new TimeSpan(startHours, startMinutes, timeSpanSeconds);
            _workdayEndTime = new TimeSpan(stopHours, stopMinutes, timeSpanSeconds);
        }
    }
}