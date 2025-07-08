public class DateTimeWrapper
{
    public string ToShortDateString()
    {
        return DateTime.Now.ToShortDateString();
    }

    public string ToLongDateString()
    {
        return DateTime.Now.ToLongDateString();
    }

    public string CurrentTime()
    {
        return DateTime.Now.ToString("T");
    }

    public string CurrentDateTime()
    {
        return DateTime.Now.ToString("F");
    }
    
    public string GetDayOfWeek()
    {
        return DateTime.Now.DayOfWeek.ToString();
    }
}
