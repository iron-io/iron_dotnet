using System;

namespace IronIO.Core
{
    public static class DateTimeHelpers
    {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

        public static int SecondsSinceEpoch(DateTime value)
        {
            return (value - UnixEpoch).Seconds;
        }
    }
}