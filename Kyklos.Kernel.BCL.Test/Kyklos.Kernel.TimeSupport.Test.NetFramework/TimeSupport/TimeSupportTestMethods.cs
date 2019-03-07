using Kyklos.Kernel.TimeSupport.Test.TimeSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kyklos.Kernel.TimeSupport.Test.NetFramework.TimeSupport
{
    public class TimeSupportTestMethods : TimeSupportBaseTestMethods
    {
        [Fact(DisplayName = "ClosestTimeUnitMustBeHours")]
        public void ClosestTimeUnitMustBeHours()
        {
            ClosestTimeUnitMustBeHoursCore();
        }

        [Fact(DisplayName = "ClosestTimeUnitMustBeMinutes")]
        public void ClosestTimeUnitMustBeMinutes()
        {
            ClosestTimeUnitMustBeMinutesCore();
        }

        [Fact(DisplayName = "ClosestTimeUnitMustBeSeconds")]
        public void ClosestTimeUnitMustBeSeconds()
        {
            ClosestTimeUnitMustBeSecondsCore();
        }

        [Fact(DisplayName = "ClosestTimeUnitMustBeDays")]
        public void ClosestTimeUnitMustBeDays()
        {
            ClosestTimeUnitMustBeDaysCore();
        }

        [Fact(DisplayName = "IntervalParsedMustBeEqualInSecondsToFormattedInterval")]
        public void IntervalParsedMustBeEqualInSecondsToFormattedInterval()
        {
            IntervalParsedMustBeEqualInSecondsToFormattedIntervalCore();
        }

        [Fact(DisplayName = "IntervalFormattedMustBeEqualInSecondsToParsedInterval")]
        public void IntervalFormattedMustBeEqualInSecondsToParsedInterval()
        {
            IntervalFormattedMustBeEqualInSecondsToParsedIntervalCore();
        }

        [Fact(DisplayName = "IntervalParsedMustBeEqualInHoursToFormattedInterval")]
        public void IntervalParsedMustBeEqualInHoursToFormattedInterval()
        {
            IntervalParsedMustBeEqualInHoursToFormattedIntervalCore();
        }

        [Fact(DisplayName = "IntervalFormattedMustBeEqualInHoursToParsedInterval")]
        public void IntervalFormattedMustBeEqualInHoursToParsedInterval()
        {
            IntervalFormattedMustBeEqualInHoursToParsedIntervalCore();
        }

        [Fact(DisplayName = "IntervalParsedMustBeEqualInDaysToFormattedInterval")]
        public void IntervalParsedMustBeEqualInDaysToFormattedInterval()
        {
            IntervalParsedMustBeEqualInDaysToFormattedIntervalCore();
        }

        [Fact(DisplayName = "IntervalFormattedMustBeEqualInDaysToParsedInterval")]
        public void IntervalFormattedMustBeEqualInDaysToParsedInterval()
        {
            IntervalFormattedMustBeEqualInDaysToParsedIntervalCore();
        }

        [Fact(DisplayName = "IntervalParsedMustBeEqualInMinutesToFormattedInterval")]
        public void IntervalParsedMustBeEqualInMinutesToFormattedInterval()
        {
            IntervalParsedMustBeEqualInMinutesToFormattedIntervalCore();
        }

        [Fact(DisplayName = "IntervalFormattedMustBeEqualInMinutesToParsedInterval")]
        public void IntervalFormattedMustBeEqualInMinutesToParsedInterval()
        {
            IntervalFormattedMustBeEqualInMinutesToParsedIntervalCore();
        }

        [Fact(DisplayName = "IntervalFromSumOfTimeSpanMustBeEqualToTimeSpan")]
        public void IntervalFromSumOfTimeSpanMustBeEqualToTimeSpan()
        {
            IntervalFromSumOfTimeSpanMustBeEqualToTimeSpanCore();
        }
    }
}
