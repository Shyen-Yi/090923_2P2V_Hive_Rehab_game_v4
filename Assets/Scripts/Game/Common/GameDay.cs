using System;

namespace com.hive.projectr
{
    /// @ingroup GameCommon
    /// @class GameDay
    /// @brief Represents a single game day and handles related events.
    ///
    /// The GameDay class is used to represent a specific day in the game, managing related events and
    /// interactions that occur within that time frame.
    [Serializable]
    public struct GameDay : IComparable
    {
        public int day;

        private static readonly int YearScale = 1000;

        public GameDay(DateTime time)
        {
            day = time.Year * YearScale + time.DayOfYear;
        }

        public DateTime ToDateTime()
        {
            var dayOfYear = day % YearScale;
            var year = day / YearScale;
            var dateTime = new DateTime(year, 1, 1).AddDays(dayOfYear);
            return dateTime;
        }

        public int CompareTo(object obj)
        {
            if (obj is GameDay gameDay)
            {
                return day - gameDay.day;
            }

            return 0;
        }
    }
}