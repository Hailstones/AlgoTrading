using System;
using System.Globalization;

namespace Algo.VolumeProfileBuilder.Internals
{
    internal static class VolumeProfileStorageConvention
    {
        public static string GetVolumeProfileFileName(string symbol, DateTime date)
        {
            return $"../../../VolumeProfile/{symbol}_{date.ToString("yyyy_MMM_dd", CultureInfo.InvariantCulture)}_volume_profile.csv";
        }
    }
}
