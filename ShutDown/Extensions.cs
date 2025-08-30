using System;

namespace ShutDown
{
    public static class Extensions
    {
        public static string ToFormatedString(this TimeSpan ts)
        {
            if (ts.TotalHours > 1.0)
            {
                if (Math.Abs(ts.Minutes) > 0.01)
                    return $"{((int)Math.Floor(ts.TotalHours))}h:{(int)ts.Minutes}min";
                return $"{((int)Math.Floor(ts.TotalHours))}h";
            }
            return $"{(int)ts.TotalMinutes}min";
        }
    }
}
