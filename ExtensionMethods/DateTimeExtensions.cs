using System;
using System.Collections.Generic;

namespace WdCal.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        public static DateTime CalculateBusinessHours(
            this DateTime finalDate,
            double incrementInWorkdaysAsPositiveValue,
            int incrementDaysSign,
            TimeSpan workdayEndTime,
            TimeSpan workdayStartTime)
        {
            var incrementRemainingFraction = incrementInWorkdaysAsPositiveValue - Math.Floor(incrementInWorkdaysAsPositiveValue);
            var businessHours = workdayEndTime.Subtract(workdayStartTime);
            var incrementFractionInBusinessHours = incrementRemainingFraction * businessHours;
            var currentFinalDateTime = new TimeSpan(finalDate.Hour, finalDate.Minute, finalDate.Second);

            if (ShouldAddTime(incrementDaysSign))
            {
                finalDate = AddBusinessHoursTime(finalDate,
                    currentFinalDateTime,
                    incrementFractionInBusinessHours,
                    workdayStartTime,
                    workdayEndTime);
            }
            else
            {
                finalDate = SubtractBusinessHoursTime(finalDate,
                    currentFinalDateTime,
                    incrementFractionInBusinessHours,
                    workdayStartTime,
                    workdayEndTime);
            }

            return finalDate;
        }

        private static bool ShouldAddTime(int incrementInWorkDaysSign)
        {
            return incrementInWorkDaysSign > 0;
        }

        private static DateTime AddBusinessHoursTime(
            DateTime finalDate,
            TimeSpan currentFinalDateTime,
            TimeSpan incrementFractionInBusinessHours,
            TimeSpan workdayStartTime,
            TimeSpan workdayEndTime)
        {
            var remainingBusinessHoursAvailable = workdayEndTime - currentFinalDateTime;
            if (IsBeforeBusinessHours(currentFinalDateTime, workdayStartTime))
            {
                finalDate = finalDate.Date + (workdayStartTime + incrementFractionInBusinessHours);
            }
            else if (IsInBusinessHours(currentFinalDateTime, workdayStartTime, workdayEndTime))
            {
                if (remainingBusinessHoursAvailable > incrementFractionInBusinessHours)
                {
                    var newTime = finalDate.TimeOfDay + incrementFractionInBusinessHours;
                    finalDate = finalDate.Date + newTime;
                }
                else
                {
                    var delta = incrementFractionInBusinessHours - remainingBusinessHoursAvailable;
                    finalDate = finalDate.AddDays(1);
                    finalDate = finalDate.Date + (workdayStartTime + delta);
                }
            }
            else if (IsOutsideBusinessHours(currentFinalDateTime, workdayEndTime))
            {
                finalDate = finalDate.AddDays(1);
                finalDate = finalDate.Date + (workdayStartTime + incrementFractionInBusinessHours);
            }

            return finalDate;
        }

        private static DateTime SubtractBusinessHoursTime(
            DateTime finalDate,
            TimeSpan currentFinalDateTime,
            TimeSpan incrementFractionInBusinessHours,
            TimeSpan workdayStartTime,
            TimeSpan workdayEndTime)
        {
            var remainingBusinessHoursAvailable = currentFinalDateTime - workdayStartTime;

            if (IsBeforeBusinessHours(currentFinalDateTime, workdayStartTime))
            {
                finalDate = finalDate.AddDays(-1);
                finalDate = finalDate.Date + (workdayEndTime - incrementFractionInBusinessHours);
            }
            else if (IsInBusinessHours(currentFinalDateTime, workdayStartTime, workdayEndTime))
            {
                if (remainingBusinessHoursAvailable > incrementFractionInBusinessHours)
                {
                    var newTime = finalDate.TimeOfDay - incrementFractionInBusinessHours;
                    finalDate = finalDate.Date + newTime;
                }
                else
                {
                    var delta = incrementFractionInBusinessHours - remainingBusinessHoursAvailable;
                    finalDate = finalDate.AddDays(-1);
                    finalDate = finalDate.Date + (workdayEndTime - delta);
                }
            }
            else if (IsOutsideBusinessHours(currentFinalDateTime, workdayEndTime))
            {
                finalDate = finalDate.Date + (workdayEndTime - incrementFractionInBusinessHours);
            }

            return finalDate;
        }

        private static bool IsOutsideBusinessHours(TimeSpan currentFinalDateTime, TimeSpan workdayEndTime)
        {
            return currentFinalDateTime > workdayEndTime;
        }

        private static bool IsInBusinessHours(TimeSpan currentFinalDateTime, TimeSpan workdayStartTime, TimeSpan workdayEndTime)
        {
            return currentFinalDateTime >= workdayStartTime && currentFinalDateTime < workdayEndTime;
        }

        private static bool IsBeforeBusinessHours(TimeSpan currentFinalDateTime, TimeSpan workdayStartTime)
        {
            return currentFinalDateTime < workdayStartTime;
        }

        public static DateTime AddFullBusinessDays(
            this DateTime finalDate,
            int incrementInWorkDaysSign,
            double incrementInWorkdaysAsPositiveValue,
            List<Tuple<int, int>> recurringHolidays,
            List<DateTime> holidays)
        {
            const int oneDayInt = 1;
            if (incrementInWorkdaysAsPositiveValue < oneDayInt)
            {
                return finalDate;
            }

            var workdays = Convert.ToInt32(Math.Floor(incrementInWorkdaysAsPositiveValue));
            while (workdays >= oneDayInt)
            {
                finalDate = finalDate.AddDays(oneDayInt * incrementInWorkDaysSign);

                if (IsWeekend(finalDate) ||
                    recurringHolidays.Contains(new Tuple<int, int>(finalDate.Month, finalDate.Day)) ||
                    holidays.Contains(finalDate.Date))
                {
                    continue;
                }

                workdays--;
            }

            return finalDate;
        }

        private static bool IsWeekend(DateTime day)
        {
            return day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}