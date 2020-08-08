using System.Collections.Generic;
using System.Linq;
using Algo.Common;
using NUnit.Framework;

namespace Algo.VwapExecution.Tests
{
    public class VwapCalculatorTests
    {
        private VwapCalculator sut;

        [SetUp]
        public void Setup()
        {
            sut = new VwapCalculator();
        }

        [Test, TestCaseSource(nameof(OnTick_ShouldCalculateAverageSource))]
        public void OnTick_ShouldCalculateAverage(decimal[] values, long[] weights, double[] expected)
        {
            Assert.AreEqual(values.Length, weights.Length,
                $"Length of {nameof(values)} does not match that of {nameof(weights)}");
            Assert.AreEqual(values.Length, expected.Length,
                $"Length of {nameof(values)} does not match that of {nameof(expected)}");
            Assert.AreNotEqual(0, values.Length, $"Length of {nameof(values)} must not be 0");

            for (var i = 0; i < values.Length; i++)
            {
                sut.Tick(values[i], weights[i]);
                var actual = sut.Average;
                Assert.IsTrue(Utilities.ApproximatelyEquals(expected[i], actual),
                    $"Average is incorrect at step i={i}. Expected={expected[i]}, Actual={actual}");
            }
        }

        public static IEnumerable<TestCaseData> OnTick_ShouldCalculateAverageSource()
        {
            yield return new TestCaseData(
                new[] {123m},
                new[] {456L},
                new[] {123.0}
            ).SetName("Single data point should return the input value");

            // data copied from a working Excel vwap implementation.
            var indicativePrice = new[]
            {
                245.0504667m, 244.7635667m, 245.3166667m, 245.86m, 245.62m, 246.1135333m, 246.4533333m, 246.4183333m,
                246.5306m, 246.7006667m, 246.5433333m, 246.17m, 245.925m, 245.9178333m, 246.41m, 246.2655333m,
                246.4752333m, 247.18m
            };
            var volume = new long[]
            {
                103033, 21168, 36544, 30057, 26301, 31494, 24271, 37951, 15324, 23285, 23365, 16130, 27227, 14464,
                17156, 23938, 70833, 59743
            };
            var vwap = new[]
            {
                245.0504667, 245.0015693, 245.073204, 245.1971478, 245.2483744, 245.3579787, 245.4554081, 245.5729812,
                245.6179755, 245.6901233, 245.7435987, 245.761283, 245.7719944, 245.776893, 245.80115, 245.8247163,
                245.9096465, 246.035658
            };

            yield return new TestCaseData(indicativePrice, volume, vwap).SetName("Sunny case");

            const decimal value = 123.45678901234567890m;
            volume = new[]
            {
                10000000 /* 1e7 */,
                100000000 /* 1e8 */,
                1000000000 /* 1e9 */,
                10000000000 /* 1e10 */,
                987654321,
                1234567890,
                1234560000000000 /* 123456e10 */,
                0,
                long.MaxValue / 2
            };

            yield return new TestCaseData(
                Enumerable.Repeat(value, volume.Length).ToArray(),
                volume,
                Enumerable.Repeat((double) value, volume.Length).ToArray()
            ).SetName("Extreme values should not throw");
        }

        [Test]
        public void Uninitialized_AverageShouldReturnNegativeOne()
        {
            Assert.AreEqual(-1m, sut.Average);
        }
    }
}