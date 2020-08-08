using System.Collections.Generic;
using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Orders;

namespace Algo.VwapExecution.QCAlgorithmAdaptor.Internals.Executions
{
    internal class ImmediateFillLimitOrderExecutionModel : VwapExecutionModelBase
    {
        private readonly OpenLimitOrdersManager _openLimitOrdersManager;

        public ImmediateFillLimitOrderExecutionModel(int maxOpenOrders)
        {
            _openLimitOrdersManager = new OpenLimitOrdersManager(maxOpenOrders);
        }

        protected override List<OrderTicket> DoExecute(QCAlgorithm algorithm, Symbol symbol, decimal quantity)
        {
            var tick = algorithm.Securities[symbol];
            var price = quantity > 0 ? tick.AskPrice : tick.BidPrice;
            var ticket = _openLimitOrdersManager.LimitOrder(algorithm, symbol, quantity, price);
            return ticket != null ? new List<OrderTicket> {ticket} : new List<OrderTicket>();
        }
    }
}
