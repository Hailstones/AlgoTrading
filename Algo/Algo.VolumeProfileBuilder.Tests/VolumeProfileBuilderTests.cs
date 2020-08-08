using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Algo.VolumeProfileBuilder.Internals;
using Moq;
using NUnit.Framework;

namespace Algo.VolumeProfileBuilder.Tests
{
    public class VolumeProfileBuilderTests
    {
        private Internals.VolumeProfileBuilder sut;
        private Mock<IVolumeProfileWriter> _csvWriterMock;

        [SetUp]
        public void Setup()
        {
            _csvWriterMock = new Mock<IVolumeProfileWriter>();
            sut = new Internals.VolumeProfileBuilder(_csvWriterMock.Object);
        }

        [Test, TestCaseSource(nameof(WithTicks_OnBuild_ShouldReturnEmptyVolumeListSource))]
        public void WithTicks_OnBuild_ShouldReturnProperVolumeProfile(decimal[] volumes, VolumeProfile expectedVolumeProfile)
        {
            foreach (var v in volumes)
            {
                sut.Tick(v);
            }

            var actual = sut.Build();

            Assert.AreEqual(expectedVolumeProfile.TotalVolume, actual.TotalVolume);
            CollectionAssert.AreEqual(expectedVolumeProfile.NormalizedVolumes, actual.NormalizedVolumes);
        }

        public static IEnumerable<TestCaseData> WithTicks_OnBuild_ShouldReturnEmptyVolumeListSource()
        {
            yield return new TestCaseData(
                new decimal[] {},
                new VolumeProfile(0, new List<decimal>())
            ).SetName("No ticks");

            const decimal tick = 123m;
            yield return new TestCaseData(
                new[] {tick},
                new VolumeProfile(123m, new List<decimal> {1})
            ).SetName("Single tick should normalize to 1");

            yield return new TestCaseData(
                new[] {1m, 2m, 3m, 4m},
                new VolumeProfile(10m, new List<decimal> {0.1m, 0.3m, 0.6m, 1m})
            ).SetName("Sunny case #1");

            const int count = 10_000_000;
            const decimal value = 3m;
            yield return new TestCaseData(
                Enumerable.Repeat(value, count).ToArray(),
                new VolumeProfile(value * count
                    , Enumerable.Range(1, count).Select(x => (decimal) x / count).ToList())
            ).SetName("Sunny case #2");
        }

        [Test]
        public void RandomData_OnBuild_NormalizedVolumeShouldAlwaysConvergeToOneExactly()
        {
            var rng = new Random(123);

            const int iteration = 1_000_000;

            for (var i = 0; i < iteration; i++)
            {
                sut.Tick(rng.Next());
            }

            var vp = sut.Build();
            Assert.AreEqual(1m, vp.NormalizedVolumes.Last());
        }

        [Test]
        public void OnBuildAndPersist_ShouldInvokeCsvWriterWriteMethod()
        {
            // Arrange
            int count = 0;

            _csvWriterMock.Setup(x => x.Write(It.IsAny<VolumeProfile>(), It.IsAny<string>()))
                .Callback(() => Interlocked.Increment(ref count));

            // Act
            sut.Build(true);

            // Assert
            Assert.AreEqual(1, count);
        }
    }
}