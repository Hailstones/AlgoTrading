using System;
using System.Collections.Generic;
using QuantConnect.Algorithm;
using QuantConnect.Algorithm.Framework.Alphas;
using QuantConnect.Data;

namespace Algo.VwapExecution.QCAlgorithmAdaptor.Internals
{
    internal class EchoAlphaModel : AlphaModel
    {
        private readonly Insight[] _insights;

        public EchoAlphaModel(Insight[] insightsToEcho)
        {
            _insights = insightsToEcho ?? throw new ArgumentNullException(nameof(insightsToEcho));
        }

        public override IEnumerable<Insight> Update(QCAlgorithm algorithm, Slice data)
        {
            return _insights;
        }
    }
}
