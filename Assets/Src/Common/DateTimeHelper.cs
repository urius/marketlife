using System;

public class DateTimeHelper
{
    public static DateTime GetDateTimeByUnitTimestamp(int unixTimestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().AddSeconds(unixTimestamp);
    }

    public static bool IsNextDay(DateTime previousDate, DateTime targetDate)
    {
        if (targetDate.DayOfYear == previousDate.DayOfYear + 1)
        {
            return true;
        }
        else
        {
            var lastDayOfPrevYear = new DateTime(previousDate.Year, 12, 31);
            return targetDate.DayOfYear == 1 && lastDayOfPrevYear.DayOfYear == previousDate.DayOfYear;
        }
    }
}
