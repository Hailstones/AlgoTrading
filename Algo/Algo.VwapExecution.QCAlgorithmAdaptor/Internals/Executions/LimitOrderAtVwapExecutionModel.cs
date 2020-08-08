using System.Collections.Generic;
using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Orders;

namespace Algo.VwapExecution.QCAlgorithmAdaptor.Internals.Executions
{
    internal class LimitOrderAtVwapExecutionModel : VwapExecutionModelBase
    {
        private readonly VwapCalculator _vwap;
        private readonly OpenLimitOrdersManager _openLimitOrdersManager;

        public LimitOrderAtVwapExecutionModel(int maxOpenOrders, VwapCalculator vwap)
        {
            _vwap = vwap;
            _openLimitOrdersManager = new OpenLimitOrdersManager(maxOpenOrders);
        }

        protected override List<OrderTicket> DoExecute(QCAlgorithm algorithm, Symbol symbol, decimal quantity)
        {
            var tick = algorithm.Securities[symbol];

            var average = _vwap.Average;
            decimal price;
            if (average <= 0)
            {
                price = quantity > 0 ? tick.BidPrice : tick.AskPrice;
            }
            else
            {
                price = (decimal) average;
            }

            var ticket = _openLimitOrdersManager.LimitOrder(algorithm, symbol, quantity, price);
            return ticket != null ? new List<OrderTicket> {ticket} : new List<OrderTicket>();
        }
    }
}
