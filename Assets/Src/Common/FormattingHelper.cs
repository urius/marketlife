public class FormattingHelper
{
    public static string ToSeparatedTimeFormat(int timeSeconds)
    {
        var hours = timeSeconds / 3600;
        var restSeconds = timeSeconds % 3600;
        var minutes = restSeconds / 60;
        restSeconds %= 60;
        if (hours > 0)
        {
            return $"{GetTwoDigitsString(hours)}:{GetTwoDigitsString(minutes)}:{GetTwoDigitsString(restSeconds)}";
        }
        else
        {
            return $"{GetTwoDigitsString(minutes)}:{GetTwoDigitsString(restSeconds)}";
        }
    }

    public static string ToCommaSeparatedNumber(int amount)
    {
        return string.Format("{0:n0}", amount);
    }

    private static string GetTwoDigitsString(int value)
    {
        return value < 10 ? $"0{value}" : value.ToString();
    }
}
