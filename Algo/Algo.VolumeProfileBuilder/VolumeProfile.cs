using System.Collections.Generic;

namespace Algo.VolumeProfileBuilder
{
    // Each entry represents 1 minute
    public class VolumeProfile
    {
        public VolumeProfile(decimal totalVolume, List<decimal> normalizedVolumes)
        {
            TotalVolume = totalVolume;
            NormalizedVolumes = normalizedVolumes;
        }

        public decimal TotalVolume { get; }
        public List<decimal> NormalizedVolumes { get; }
    }
}
