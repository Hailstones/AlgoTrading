using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Algo.VolumeProfileBuilder;
using Algo.VwapExecution.QCAlgorithmAdaptor.Internals;
using Algo.VwapExecution.QCAlgorithmAdaptor.Internals.Executions;
using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Algorithm.Framework.Alphas;
using QuantConnect.Algorithm.Framework.Execution;
using QuantConnect.Data;

namespace Algo.VwapExecution.QCAlgorithmAdaptor
{
    public class VwapExecutionAlgorithm : QCAlgorithm
    {
        private string _symbol;
        private decimal _targetQuantity;
        private VolumeProfile _volumeProfile;

        private IReadOnlyDictionary<ExecutionStrategy, ExecutionModel> _strategyModelMap;

        private readonly VwapCalculator _vwapCalculator = new VwapCalculator();

        public override void Initialize()
        {
            DateTime ParseDateTimeParameter(string key)
            {
                var dateStr = GetParameter(key);
                return DateTime.ParseExact(dateStr, "O", CultureInfo.InvariantCulture);
            }

            _symbol = GetParameter(Constants.SymbolKey);

            var orderDate = ParseDateTimeParameter(Constants.OrderDateKey);

            // Set the period from which the framework will feed historic data
            // to our algorithm
            SetStartDate(orderDate);
            SetEndDate(orderDate);

            // We only care about the vwap execution algorithm
            // Assume we always have enough cash to execute the order
            Portfolio.SetCash(100000000000000);

            _targetQuantity = Decimal.Parse(GetParameter(Constants.QuantityKey));

            // Subscribe the tick data for our symbol
            AddEquity(_symbol, Resolution.Tick);

            // Abuse the insight a bit to pass the order parameters to the framework
            //   InsightDirection.Up -> Buy Order
            //   InsightDirection.Down -> Sell Order
            //   Magnitude -> Order Quantity
            var symbol = Securities[_symbol].Symbol;
            var insight = new Insight(
                symbol,
                TimeSpan.FromDays(1),
                InsightType.Price,
                _targetQuantity > 0 ? InsightDirection.Up : InsightDirection.Down,
                (double) _targetQuantity,
                null
            );
            SetAlpha(new EchoAlphaModel(new[] {insight}));

            var profileFile = GetParameter(Constants.VolumeProfileFileKey);
            var volumeProfileDate = ParseDateTimeParameter(Constants.VolumeProfileDateKey);
            var vpReader = new VolumeProfileBuilderFactory().CreateVolumeProfileReader(_symbol, volumeProfileDate, profileFile);
            _volumeProfile = vpReader.Read();
            var portfolioConstruction = new VwapPortfolioConstructionModel(_volumeProfile);
            SetPortfolioConstruction(portfolioConstruction);

            _strategyModelMap = new Dictionary<ExecutionStrategy, ExecutionModel>
            {
                [ExecutionStrategy.Aggressive3] = new ImmediateMarketOrderExecutionModel(),
                [ExecutionStrategy.Aggressive2] = new ImmediateFillLimitOrderExecutionModel(3),
                [ExecutionStrategy.Aggressive1] = new BestPriceLimitOrderExecutionModel(3),
                [ExecutionStrategy.Standard] = new LimitOrderAtVwapExecutionModel(3, _vwapCalculator),
            };

            var executionStrategy = (ExecutionStrategy) Enum.Parse(typeof(ExecutionStrategy)
                , GetParameter(Constants.ExecutionStrategyKey)
                , true);
            var executionModel = _strategyModelMap[executionStrategy];
            SetExecution(executionModel);
        }

        public override void OnData(Slice slice)
        {
            var symbol = Securities[_symbol].Symbol;
            if (slice.HasData && slice.TryGetValue(symbol, out var ticks))
            {
                foreach (var tick in ticks)
                {
                    _vwapCalculator.Tick(tick.Price, tick.Quantity);
                }
            }
        }

        public override void OnEndOfAlgorithm()
        {
            var s = Transactions.GetOrderTickets().Select(x => x.AverageFillPrice * x.QuantityFilled).Sum();
            var q = Transactions.GetOrderTickets().Select(x => x.QuantityFilled).Sum();

            Debug($"Target Quantity={_targetQuantity}. Quantity Filled={q}. Average Price={s / q}. Vwap={_vwapCalculator.Average}");
        }
    }
}
