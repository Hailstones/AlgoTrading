using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Algo.VolumeProfileBuilder.Internals
{
    internal class VolumeProfileCsvReadWriter : IVolumeProfileReader, IVolumeProfileWriter
    {
        private readonly string _defaultFile;

        public VolumeProfileCsvReadWriter(string defaultFile)
        {
            _defaultFile = defaultFile ?? throw new ArgumentNullException(nameof(defaultFile));
        }

        public void Write(VolumeProfile volumeProfile, string outputFile)
        {
            outputFile = String.IsNullOrEmpty(outputFile) ? _defaultFile : outputFile;

            var sb = new StringBuilder();
            sb.AppendLine(volumeProfile.TotalVolume.ToString(CultureInfo.InvariantCulture));

            sb.Append(volumeProfile.NormalizedVolumes.First());

            foreach (var entry in volumeProfile.NormalizedVolumes.Skip(1))
            {
                sb.Append(",");
                sb.Append(entry);
            }

            File.WriteAllText(outputFile, sb.ToString());
        }

        public VolumeProfile Read(string csvFile)
        {
            csvFile = String.IsNullOrEmpty(csvFile) ? _defaultFile : csvFile;

            var lines = File.ReadAllLines(csvFile);

            var totalVolume = Decimal.Parse(lines[0], CultureInfo.InvariantCulture);
            var normalizedVolumes = lines[1].Split(',')
                .Select(Decimal.Parse)
                .ToList();

            return new VolumeProfile(totalVolume, normalizedVolumes);
        }
    }
}
