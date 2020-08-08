using System.Collections.Generic;
using QuantConnect.Algorithm;
using QuantConnect;
using QuantConnect.Orders;

namespace Algo.VwapExecution.QCAlgorithmAdaptor.Internals.Executions
{
    internal class ImmediateMarketOrderExecutionModel : VwapExecutionModelBase
    {
        protected override List<OrderTicket> DoExecute(QCAlgorithm algorithm, Symbol symbol, decimal quantity)
        {
            var ticket = algorithm.MarketOrder(symbol, quantity, true);
            algorithm.Debug($"{algorithm.Time} - ({ticket.OrderId}) Placing Market Order for {quantity} share of {symbol}");
            return new List<OrderTicket> {ticket};
        }
    }
}
