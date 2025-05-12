namespace Cachara.Shared.Domain.Utils;

public static class DateUtils
{
    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }
}
