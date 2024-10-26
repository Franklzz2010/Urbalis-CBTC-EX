using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbalisAts.OBCU
{
    public static class TimeFormatter
    {
        public static int MiliSecondToInt(int ms)
        {
            int totalSeconds = ms / 1000;
            return totalSeconds;
        }

        public static int MiliSecondToInt(double ms)
        {
            int totalSeconds = (int)(ms / 1000);
            return totalSeconds;
        }

        public static string MiliSecondToString(int ms)
        {
            int totalSeconds = ms / 1000;

            string hours = Convert.ToString(totalSeconds / 3600 % 60).PadLeft(2, '0');
            string minutes = Convert.ToString(totalSeconds / 60 % 60).PadLeft(2, '0');
            string seconds = Convert.ToString(totalSeconds % 60).PadLeft(2, '0');
            return hours + ":" + minutes + ":" + seconds;
        }

        public static string MiliSecondToString(double ms)
        {
            int totalSeconds = (int)(ms / 1000);

            string hours = Convert.ToString(totalSeconds / 3600 % 60).PadLeft(2, '0');
            string minutes = Convert.ToString(totalSeconds / 60 % 60).PadLeft(2, '0');
            string seconds = Convert.ToString(totalSeconds % 60).PadLeft(2, '0');
            return hours + ":" + minutes + ":" + seconds;
        }

        public static string MiliSecondToShortString(int ms)
        {
            int totalSeconds = (ms / 1000);

            string hours = Convert.ToString(totalSeconds / 3600 % 60).PadLeft(2, '0');
            string minutes = Convert.ToString(totalSeconds / 60 % 60).PadLeft(2, '0');
            return hours + ":" + minutes;
        }

        public static string MiliSecondToShortString(double ms)
        {
            int totalSeconds = (int)(ms / 1000);

            string hours = Convert.ToString(totalSeconds / 3600 % 60).PadLeft(2, '0');
            string minutes = Convert.ToString(totalSeconds / 60 % 60).PadLeft(2, '0');
            return hours + ":" + minutes;
        }
    }
}
