using System;
using System.Collections.Generic;
using System.Text;

namespace BlackLegionBot.CommandHandling.SpecialsOperatorsHandling
{
    public struct DateTimeSpan
    {
        public int Years { get; }
        public int Months { get; }
        public int Days { get; }
        public int Hours { get; }
        public int Minutes { get; }
        public int Seconds { get; }
        public int Milliseconds { get; }

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            Years = years;
            Months = months;
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds;
        }

        // TODO: Should really be tested
        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            var (date1PostMonths, months) = GetMonths(date1, date2);
            var years = (int) Math.Floor((decimal) months / 12);
            months %= 12;
            var dateTimeSpan =  GetDaysAndBelow(date1PostMonths, date2, years, months);
            return dateTimeSpan;
        }

        private static (DateTime date, int months) GetMonths(DateTime date1, DateTime date2)
        {
            var monthCount = 0;
            while (date1.Year < date2.Year || date1.Month < date2.Month - 1 || (date1.Month == date2.Month - 1 && date1.Day < date2.Day || (date1.Day == date2.Day && (date2 - date1).Days > 0)))
            {
                date1 = date1.AddMonths(1);
                monthCount++;
            }
            return (date1, monthCount);
        }

        private static DateTimeSpan GetDaysAndBelow(DateTime date1, DateTime date2, int years, int months)
        {
            var timeSpan = date2 - date1;
            return new DateTimeSpan(years, months, timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }
    }
}