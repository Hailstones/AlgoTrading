using System;
using System.Collections.Generic;
using System.Linq;
using QuantConnect;

namespace Algo.VolumeProfileBuilder.Internals
{
    internal class VolumeProfileBuilder : IVolumeProfileBuilder
    {
        private readonly List<decimal> _volumes = new List<decimal>();
        private readonly IVolumeProfileWriter _volumeProfileCsvReadWriter;

        public VolumeProfileBuilder(IVolumeProfileWriter volumeProfileCsvReadWriter)
        {
            _volumeProfileCsvReadWriter = volumeProfileCsvReadWriter;
        }

        public void Tick(decimal volume)
        {
            _volumes.Add(volume);
        }

        public VolumeProfile Build(bool persist = false)
        {
            decimal sum = 0;
            var prefixSum = _volumes.Select(x => sum += x).ToList();
            var normalizedVolumes = prefixSum.Select(x => x / sum).ToList();

            var vp = new VolumeProfile(sum, normalizedVolumes);

            if (persist)
            {
                try
                {
                    _volumeProfileCsvReadWriter.Write(vp);
                }
                catch (Exception e)
                {
                    // TODO: Do some error reporting. But have to wire this class to the framework first.
                }
            }

            return vp;
        }
    }
}
