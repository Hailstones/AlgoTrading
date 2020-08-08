using System;
using System.Collections.Generic;
using System.Linq;
using QuantConnect.Securities;

namespace Algo.VwapExecution
{
    public class MarketHoursToVolumeProfileIndexHelper
    {
        public int ToIndex(IEnumerable<MarketHoursSegment> segments, DateTime time)
        {
            bool isSet = false;

            TimeSpan elapsedMarketTime = default;
            foreach (var segment in segments.Where(s => s.State == MarketHoursState.Market))
            {
                var elapsedTime = time - time.Date;
                if (segment.Contains(elapsedTime))
                {
                    isSet = true;
                    elapsedMarketTime += elapsedTime - segment.Start;
                }
                else if (elapsedTime >= segment.End)
                {
                    isSet = true;
                    elapsedMarketTime += segment.End - segment.Start;
                }
            }

            return isSet ? (int) Math.Floor(elapsedMarketTime.TotalMinutes) : -1;
        }
    }
}
