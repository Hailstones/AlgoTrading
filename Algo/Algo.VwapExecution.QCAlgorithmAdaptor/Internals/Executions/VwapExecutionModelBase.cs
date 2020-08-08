using System.Collections.Generic;
using System.Linq;
using Algo.Common;
using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Algorithm.Framework.Execution;
using QuantConnect.Algorithm.Framework.Portfolio;
using QuantConnect.Orders;

namespace Algo.VwapExecution.QCAlgorithmAdaptor.Internals.Executions
{
    internal abstract class VwapExecutionModelBase : ExecutionModel
    {
        public override void Execute(QCAlgorithm algorithm, IPortfolioTarget[] targets)
        {
            var orders = targets.GroupBy(t => t.Symbol)
                .Select(
                    g => new
                    {
                        Symbol = g.Key,
                        Quantity = g.Sum(x => OrderSizing.GetUnorderedQuantity(algorithm, x))
                    }
                )
                .Where(order =>
                    algorithm.Securities[order.Symbol].HasData
                    && order.Quantity != 0);

            foreach (var order in orders)
            {
                var tickets = DoExecute(algorithm, order.Symbol, order.Quantity);

                foreach (var ticket in tickets)
                {
                    ticket.OrderClosed
                        .WaitOneAsync()
                        .ContinueWith(
                            task =>
                            {
                                if (task.Result)
                                {
                                    algorithm.Debug($"{algorithm.Time} - ({ticket.OrderId}) {ticket.QuantityFilled} share of {ticket.Symbol} is filled @{ticket.AverageFillPrice}.");
                                }
                            }
                        );
                }
            }
        }

        protected abstract List<OrderTicket> DoExecute(QCAlgorithm algorithm, Symbol symbol, decimal quantity);
    }
}
