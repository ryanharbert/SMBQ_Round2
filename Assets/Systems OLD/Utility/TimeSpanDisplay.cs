using System;

public class TimeSpanDisplay
{
    public static string Format(TimeSpan t)
    {
        return Format(t, 2);
    }

    public static string Format(TimeSpan t, int detail)
    {
        int i = 0;
        string timer = "";
        if (t.Days != 0 && i < detail)
        {
            timer += t.Days + "d";
            i++;
        }
        if (t.Hours != 0 && i < detail)
        {
            if(i != 0)
            {
                timer += " ";
            }
            timer += t.Hours + "h";
            i++;
        }
        if (t.Minutes != 0 && i < detail)
        {
            if (i != 0)
            {
                timer += " ";
            }
            timer += t.Minutes + "m";
            i++;
        }
        if (t.Seconds != 0 && i < detail)
        {
            if (i != 0)
            {
                timer += " ";
            }
            timer += t.Seconds + "s";
            i++;
        }
        return timer;
    }
}
