using System;

public class ResetTime
{
    public static string Get()
    {
        string timeLeft = "";
        int hour;

        if (DateTime.UtcNow.Hour >= 8)
        {
            hour = 24 - DateTime.UtcNow.Hour + 8;
        }
        else
        {
            hour = 8 - DateTime.UtcNow.Hour;
        }
        int minutes = 60 - DateTime.UtcNow.Minute;
        if(minutes == 60)
        {
            minutes -= 1;
            hour += 1;
        }
        int seconds = 60 - DateTime.UtcNow.Second;

        if (hour != 0)
        {
            timeLeft = hour + "h " + minutes + "m ";
        }
        else if (minutes != 0)
        {
            timeLeft = minutes + "m " + seconds + "s";
        }
        else
        {
            timeLeft = seconds + "s";
        }

        return "New Offers In " + timeLeft;
    }
}
