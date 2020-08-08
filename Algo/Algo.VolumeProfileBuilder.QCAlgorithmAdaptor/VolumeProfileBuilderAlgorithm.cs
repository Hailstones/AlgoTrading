using System;
using System.Globalization;
using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Data;

namespace Algo.VolumeProfileBuilder.QCAlgorithmAdaptor
{
    public class VolumeProfileBuilderAlgorithm : QCAlgorithm
    {
        private  IVolumeProfileBuilder _volumeProfileBuilder;
        private string _symbol;

        public override void Initialize()
        {
            _symbol = GetParameter(Constants.SymbolKey);
            var outputPath = GetParameter(Constants.OutputFileKey);

            var date = DateTime.ParseExact(GetParameter(Constants.ReferenceDateKey), "O", CultureInfo.InvariantCulture);

            // Set the period from which the framework will feed historic data
            // to our algorithm
            SetStartDate(date);
            SetEndDate(date);

            // Subscribe the data (consolidated to a minute) for our symbol
            AddEquity(_symbol, Resolution.Minute);

            _volumeProfileBuilder = new VolumeProfileBuilderFactory().CreateVolumeProfileBuilder(_symbol, date, outputPath);
        }

        public override void OnData(Slice slice)
        {
            var tick = slice.Bars[_symbol];
            _volumeProfileBuilder.Tick(tick.Volume);
        }

        public override void OnEndOfAlgorithm()
        {
             _volumeProfileBuilder.Build(true);
        }
    }
}
