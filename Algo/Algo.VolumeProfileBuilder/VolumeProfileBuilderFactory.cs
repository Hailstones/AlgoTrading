using System;
using Algo.VolumeProfileBuilder.Internals;

namespace Algo.VolumeProfileBuilder
{
    public class VolumeProfileBuilderFactory
    {
        public IVolumeProfileBuilder CreateVolumeProfileBuilder(string symbol, DateTime referenceDate, string outputFile = null)
        {
            outputFile = outputFile ?? VolumeProfileStorageConvention.GetVolumeProfileFileName(symbol, referenceDate);
            return new Internals.VolumeProfileBuilder(new VolumeProfileCsvReadWriter(outputFile));
        }

        public IVolumeProfileReader CreateVolumeProfileReader(string symbol, DateTime referenceDate, string csvFile = null)
        {
            csvFile = csvFile ?? VolumeProfileStorageConvention.GetVolumeProfileFileName(symbol, referenceDate);
            return new VolumeProfileCsvReadWriter(csvFile);
        }
    }
}
