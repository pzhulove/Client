using System;
using System.Diagnostics;
using System.Runtime.Serialization;


namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Time
        {
            private const long CONST_TICKS_PER_SECOND = 10000000L;
            private const float CONST_SECOND_PER_TICKS = 1E-07F;

            private const long CONST_TICKS_PER_MICRO_SECOND = 10000L;
            private const float CONST_MICRO_SECOND_PER_TICKS = 1E-04F;

            public static float TicksToSeconds(long ticks)
            {
                return ticks * CONST_SECOND_PER_TICKS;
            }

            public static long SecondsToTicks(float seconds)
            {
                return (long)(seconds * CONST_TICKS_PER_SECOND);
            }

            public static float TicksToMicroseconds(long ticks)
            {
                return ticks * CONST_MICRO_SECOND_PER_TICKS;
            }

            public static long MicrosecondsToTicks(float microseconds)
            {
                return (long)(microseconds * CONST_TICKS_PER_MICRO_SECOND);
            }

            public static DateTime TicksToDateTime(long ticks)
            {
                return new DateTime(ticks);
            }

            public static long GetTicksNow()
            {
                return System.DateTime.Now.Ticks;
            }
        }
    }
}

