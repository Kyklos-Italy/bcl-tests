using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;

namespace Kyklos.Kernel.TimeSupport.Test.TimeSupport
{
    public class TimeSupportBaseTestMethods 
    {
        protected void ClosestTimeUnitMustBeHoursCore()
        {
            TimeExtMethods.FormatIntervalTimeUnit expectedTimeUnit = TimeExtMethods.FormatIntervalTimeUnit.Hours;
            TimeSpan interval = DateTime.Now.TimeOfDay;
            var closestTimeUnit = TimeExtMethods.DetectClosestTimeUnit(interval);
            Assert.True(closestTimeUnit == expectedTimeUnit);
        }

        protected void ClosestTimeUnitMustBeMinutesCore()
        {
            TimeExtMethods.FormatIntervalTimeUnit expectedTimeUnit = TimeExtMethods.FormatIntervalTimeUnit.Minutes;
            TimeSpan interval = TimeSpan.FromMinutes(40);
            var closestTimeUnit = TimeExtMethods.DetectClosestTimeUnit(interval);
            Assert.True(closestTimeUnit == expectedTimeUnit);
        }

        protected void ClosestTimeUnitMustBeSecondsCore()
        {
            TimeExtMethods.FormatIntervalTimeUnit expectedTimeUnit = TimeExtMethods.FormatIntervalTimeUnit.Seconds;
            TimeSpan interval = TimeSpan.FromSeconds(40);
            var closestTimeUnit = TimeExtMethods.DetectClosestTimeUnit(interval);
            Assert.True(closestTimeUnit == expectedTimeUnit);
        }

        protected void ClosestTimeUnitMustBeDaysCore()
        {
            TimeExtMethods.FormatIntervalTimeUnit expectedTimeUnit = TimeExtMethods.FormatIntervalTimeUnit.Days;
            TimeSpan interval = TimeSpan.FromDays(40);
            var closestTimeUnit = TimeExtMethods.DetectClosestTimeUnit(interval);
            Assert.True(closestTimeUnit == expectedTimeUnit);
        }

        protected void IntervalParsedMustBeEqualInSecondsToFormattedIntervalCore()
        {
            TimeSpan expectedInterval = 2.5.Seconds();
            string expectedIntervalFormatted = TimeExtMethods.FormatInterval(expectedInterval, TimeExtMethods.FormatIntervalTimeUnit.Seconds);
            TimeSpan? actualInterval = TimeExtMethods.ParseInterval(expectedIntervalFormatted);
            Assert.Equal(expectedInterval.TotalSeconds, actualInterval.Value.TotalSeconds);
        }

        protected void IntervalFormattedMustBeEqualInSecondsToParsedIntervalCore()
        {
            TimeSpan expectedInterval = 2.5.Seconds();
            string actualIntervalFormatted = TimeExtMethods.FormatInterval(expectedInterval, TimeExtMethods.FormatIntervalTimeUnit.Seconds);
            TimeSpan? actualInterval = TimeExtMethods.ParseInterval(actualIntervalFormatted);
            Assert.Equal(expectedInterval.TotalSeconds, actualInterval.Value.TotalSeconds);
        }

        protected void IntervalParsedMustBeEqualInHoursToFormattedIntervalCore()
        {
            TimeSpan expectedInterval = 2.5.Hours();
            string expectedIntervalFormatted = TimeExtMethods.FormatInterval(expectedInterval, TimeExtMethods.FormatIntervalTimeUnit.Hours);
            TimeSpan? actualInterval = TimeExtMethods.ParseInterval(expectedIntervalFormatted);
            Assert.Equal(expectedInterval.TotalHours, actualInterval.Value.TotalHours);
        }

        protected void IntervalFormattedMustBeEqualInHoursToParsedIntervalCore()
        {
            TimeSpan expectedInterval = 2.5.Hours();
            string actualIntervalFormatted = TimeExtMethods.FormatInterval(expectedInterval, TimeExtMethods.FormatIntervalTimeUnit.Hours);
            TimeSpan? actualInterval = TimeExtMethods.ParseInterval(actualIntervalFormatted);
            Assert.Equal(expectedInterval.TotalHours, actualInterval.Value.TotalHours);
        }

        protected void IntervalParsedMustBeEqualInDaysToFormattedIntervalCore()
        {
            TimeSpan expectedInterval = TimeSpan.FromDays(2.5F);
            string expectedIntervalFormatted = TimeExtMethods.FormatInterval(expectedInterval, TimeExtMethods.FormatIntervalTimeUnit.Days);
            TimeSpan? actualInterval = TimeExtMethods.ParseInterval(expectedIntervalFormatted);
            Assert.Equal(expectedInterval.TotalDays, actualInterval.Value.TotalDays);
        }

        protected void IntervalFormattedMustBeEqualInDaysToParsedIntervalCore()
        {
            TimeSpan expectedInterval = TimeSpan.FromDays(2.5F);
            string actualIntervalFormatted = TimeExtMethods.FormatInterval(expectedInterval, TimeExtMethods.FormatIntervalTimeUnit.Days);
            TimeSpan? actualInterval = TimeExtMethods.ParseInterval(actualIntervalFormatted);
            Assert.Equal(expectedInterval.TotalDays, actualInterval.Value.TotalDays);
        }

        protected void IntervalParsedMustBeEqualInMinutesToFormattedIntervalCore()
        {
            TimeSpan expectedInterval = 2.5.Minutes();
            string expectedIntervalFormatted = TimeExtMethods.FormatInterval(expectedInterval, TimeExtMethods.FormatIntervalTimeUnit.Minutes);
            TimeSpan? actualInterval = TimeExtMethods.ParseInterval(expectedIntervalFormatted);
            Assert.Equal(expectedInterval.TotalMinutes, actualInterval.Value.TotalMinutes);
        }

        protected void IntervalFormattedMustBeEqualInMinutesToParsedIntervalCore()
        {
            TimeSpan expectedInterval = 2.5.Minutes();
            string actualIntervalFormatted = TimeExtMethods.FormatInterval(expectedInterval, TimeExtMethods.FormatIntervalTimeUnit.Minutes);
            TimeSpan? actualInterval = TimeExtMethods.ParseInterval(actualIntervalFormatted);
            Assert.Equal(expectedInterval.TotalMinutes, actualInterval.Value.TotalMinutes);
        }

        protected void IntervalFromSumOfTimeSpanMustBeEqualToTimeSpanCore()
        {
            TimeSpan expectedInterval = TimeSpan.FromHours(1.3F) + TimeSpan.FromMinutes(2.5F) + TimeSpan.FromSeconds(1);
            TimeSpan actualInterval = 1.3.Hours() + 2.5.Minutes() + 1.Seconds();
            Assert.Equal(expectedInterval, actualInterval);
        }
    }
}
