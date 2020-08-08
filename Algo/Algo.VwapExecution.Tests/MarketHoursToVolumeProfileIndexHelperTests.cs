using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuantConnect.Securities;

namespace Algo.VwapExecution.Tests
{
    public class MarketHoursToVolumeProfileIndexHelperTests
    {
        private MarketHoursToVolumeProfileIndexHelper sut;

        [SetUp]
        public void Setup()
        {
            sut = new MarketHoursToVolumeProfileIndexHelper();
        }

        [Test, TestCaseSource(nameof(MarketHoursShouldBeConvertedToIndexProperlySource))]
        public void MarketHoursShouldBeConvertedToIndexProperly(List<MarketHoursSegment> segments, DateTime time, int expectedIndex)
        {
            var actual = sut.ToIndex(segments, time);
            Assert.AreEqual(expectedIndex, actual);
        }

        public static IEnumerable<TestCaseData> MarketHoursShouldBeConvertedToIndexProperlySource()
        {
            var segments = new List<MarketHoursSegment>
                {
                    new MarketHoursSegment(MarketHoursState.Market, new TimeSpan(9, 30, 0), new TimeSpan(12, 30, 0))
                };

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(9, 0, 0), -1
            ).SetName("Before market open should return -1 -- #1");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(9, 29, 00), -1
            ).SetName("Before market open should return -1 -- #2");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(9, 29, 59), -1
            ).SetName("Before market open should return -1 -- #3");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(9, 30, 0), 0
            ).SetName("Market opened should return the minute elapsed rounded down -- #1");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(9, 30, 1), 0
            ).SetName("Market opened should return the minute elapsed rounded down -- #2");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(9, 30, 15), 0
            ).SetName("Market opened should return the minute elapsed rounded down -- #3");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(9, 30, 50), 0
            ).SetName("Market opened should return the minute elapsed rounded down -- #4");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(9, 30, 59), 0
            ).SetName("Market opened should return the minute elapsed rounded down -- #5");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(9, 31, 0), 1
            ).SetName("Market opened should return the minute elapsed rounded down -- #6");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(9, 31, 59), 1
            ).SetName("Market opened should return the minute elapsed rounded down -- #7");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(12, 30, 00), 180
            ).SetName("After market closed should return the number of market open minutes -- #1");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(12, 30, 01), 180
            ).SetName("After market closed should return the number of market open minutes -- #2");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(12, 35, 00), 180
            ).SetName("After market closed should return the number of market open minutes -- #3");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(15, 00, 00), 180
            ).SetName("After market closed should return the number of market open minutes -- #4");

            yield return new TestCaseData
            (
                segments, DateTime.Today + new TimeSpan(23, 59, 59), 180
            ).SetName("After market closed should return the number of market open minutes -- #5");
        }
    }
}
