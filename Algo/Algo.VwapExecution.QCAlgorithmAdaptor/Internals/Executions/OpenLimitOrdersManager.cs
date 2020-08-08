using System.Collections.Generic;
using System.Linq;
using Algo.Common;
using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Orders;

namespace Algo.VwapExecution.QCAlgorithmAdaptor.Internals.Executions
{
    internal class OpenLimitOrdersManager
    {
        private readonly int _maxOpenOrders;
        private readonly object _lock = new object();
        private decimal _pendingQuantity;

        public OpenLimitOrdersManager(int maxOpenOrders)
        {
            _maxOpenOrders = maxOpenOrders;
        }

        public OrderTicket LimitOrder(QCAlgorithm algorithm, Symbol symbol, decimal quantity, decimal price)
        {
            var openOrders = algorithm.Transactions.GetOpenOrders(symbol);

            void CancelOpenOrder(Order order)
            {
                var orderTicket = order.ToOrderTicket(algorithm.Transactions);
                try
                {
                    orderTicket.Cancel();
                    orderTicket
                        .OrderClosed
                        .WaitOneAsync()
                        .ContinueWith(
                            task =>
                            {
                                if (task.Result)
                                {
                                    lock (_lock)
                                    {
                                        _pendingQuantity += orderTicket.Quantity - orderTicket.QuantityFilled;
                                    }
                                }
                            });
                }
                catch
                {
                    // ignored
                }
            }

            List<Order> sortedOrders;
            Order orderToAmend = null;
            if (quantity > 0)
            {
                sortedOrders = openOrders.OrderBy(o => o.Price).ToList();

                if (sortedOrders.Any())
                {
                    var bestPriceOrder = sortedOrders.Last();
                    if (bestPriceOrder.Price > price)
                    {
                        price = bestPriceOrder.Price;
                        orderToAmend = bestPriceOrder;
                    }
                }
            }
            else
            {
                sortedOrders = openOrders.OrderByDescending(o => o.Price).ToList();

                if (sortedOrders.Any())
                {
                    var bestPriceOrder = sortedOrders.Last();
                    if (bestPriceOrder.Price < price)
                    {
                        price = bestPriceOrder.Price;
                        orderToAmend = bestPriceOrder;
                    }
                }
            }

            foreach (var order in sortedOrders.Take(_maxOpenOrders))
            {
                CancelOpenOrder(order);
            }

            lock (_lock)
            {
                quantity += _pendingQuantity;
                _pendingQuantity = 0;
            }

            if (orderToAmend != null)
            {
                var ticket = orderToAmend.ToOrderTicket(algorithm.Transactions);
                var originalQuantity = ticket.Quantity;

                try
                {
                    ticket.UpdateQuantity(originalQuantity + quantity);
                    algorithm.Debug(
                        $"{algorithm.Time} - ({ticket.OrderId}) Update Limit Order of {symbol} from {originalQuantity}@{price} to {quantity}@{price}");
                    return ticket;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                var ticket = algorithm.LimitOrder(symbol, quantity, price);
                algorithm.Debug($"{algorithm.Time} - ({ticket.OrderId}) Placing Limit Order of {symbol} {quantity}@{price}");
                return ticket;
            }
        }
    }
}
