using System;
using System.Linq;
using Algo.VolumeProfileBuilder.Internals;
using NUnit.Framework;

namespace Algo.VolumeProfileBuilder.Tests
{
    public class VolumeProfileReaderTests
    {
        private IVolumeProfileReader sut;

        [SetUp]
        public void Setup()
        {
            var symbol = "SPY";
            var date = new DateTime(2013, 10, 4);

            var file = VolumeProfileStorageConvention.GetVolumeProfileFileName(symbol, date);

            sut = new VolumeProfileBuilderFactory().CreateVolumeProfileReader(symbol, date, "../" + file);
        }

        [Test]
        public void Read_NoThrow()
        {
            var vp = sut.Read();

            Assert.IsNotNull(vp);
            Assert.IsTrue(vp.NormalizedVolumes.Any());
        }
    }
}
