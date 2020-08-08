using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Algo.VolumeProfileBuilder;
using QuantConnect.Algorithm;
using QuantConnect.Algorithm.Framework.Alphas;
using QuantConnect.Algorithm.Framework.Portfolio;
using QuantConnect.Util;

namespace Algo.VwapExecution.QCAlgorithmAdaptor.Internals
{
    internal class VwapPortfolioConstructionModel : PortfolioConstructionModel
    {
        private readonly VolumeProfile _volumeProfile;
        private readonly MarketHoursToVolumeProfileIndexHelper _marketHoursHelper = new MarketHoursToVolumeProfileIndexHelper();

        public VwapPortfolioConstructionModel(VolumeProfile volumeProfile)
        {
            _volumeProfile = volumeProfile;
        }

        public override IEnumerable<IPortfolioTarget> CreateTargets(QCAlgorithm algorithm, Insight[] insights)
        {
            // Hack: The framework makes the Algorithm property setter to be private which should really
            // be protected....
            // Alternatively we could branch the framework.

            // ReSharper disable PossibleNullReferenceException
            GetType().BaseType
                .GetProperty("Algorithm", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetSetMethod(true)
                .Invoke(this, new object[] {algorithm});
            // ReSharper restore PossibleNullReferenceException

            // Use the volume profile to determine the targetQuantity.
            var percents = DetermineTargetPercent(insights.ToList());
            foreach (var insight in insights)
            {
                if (percents.TryGetValue(insight, out var percent)
                    && insight.Magnitude.HasValue)
                {
                    var symbol = insight.Symbol;
                    var targetQuantity = insight.Magnitude.Value;
                    yield return new PortfolioTarget(symbol, (decimal) (percent * targetQuantity));
                }
            }
        }

        protected override Dictionary<Insight, double> DetermineTargetPercent(List<Insight> activeInsights)
        {
            var ret = new Dictionary<Insight, double>();

            if (activeInsights.IsNullOrEmpty())
            {
                return ret;
            }

            // We are abusing the framework, so we should only have a single insight
            var insight = activeInsights.Single();

            var time = Algorithm.Time;
            var hours = Algorithm.Securities[insight.Symbol].Exchange.Hours;

            if (!hours.IsOpen(time, false))
            {
                return ret;
            }

            var segments = hours.MarketHours[time.DayOfWeek].Segments;
            var index = _marketHoursHelper.ToIndex(segments, Algorithm.Time);

            ret[insight] = (double) _volumeProfile.NormalizedVolumes[index];

            return ret;
        }
    }
}
