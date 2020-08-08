using System;
using System.Collections.Generic;
using System.Globalization;
using Algo.LeanEngineLauncher;
using CommandLine;

namespace Algo.VwapExecution.ConsoleApp
{
    class Program
    {
        public class Options
        {
            [Option('s', "symbol", Required = true, HelpText = "The symbol for the volume profile")]
            public string Symbol { get; set; }

            [Option("volume-profile-date", Required = true,
                HelpText = "yyyy-MM-dd. The date of which the volume profile will be used")]
            public DateTime VolumeProfileDate { get; set; }

            [Option("date", Required = false,
                HelpText = "yyyy-MM-dd. The date of which the algorithm is run (for backtesting)",
                Default = null)]
            public DateTime? OrderDate { get; set; }

            [Option('f', "volume-profile-file", Required = false, HelpText = "Path of the volume profile path relative to the binary", Default = null)]
            public string VolumeProfileFile { get; set; }

            [Option("side", Required = false, HelpText = "\"Buy\"\\\"Sell\"", Default = OrderSide.Buy)]
            public OrderSide OrderSide { get; set; }

            [Option('q', "quantity", Required = true, HelpText = "The quantity of the vwap order")]
            public decimal Quantity { get; set; }

            [Option("execution-strategy", Required = false, HelpText = "The chosen execution strategy", Default = ExecutionStrategy.Standard)]
            public ExecutionStrategy ExecutionStrategy { get; set; }
        }

        static void Main(string[] args)
        {
            var parameters = new Dictionary<string, string>();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    parameters[Constants.SymbolKey] = options.Symbol;

                    var volumeProfileDate = options.VolumeProfileDate;
                    var vpdStr = volumeProfileDate.ToString("O", CultureInfo.InvariantCulture);
                    parameters[Constants.VolumeProfileDateKey] = vpdStr;

                    var orderDate = options.OrderDate ?? DateTime.Now.Date;
                    var dateStr = orderDate.ToString("O", CultureInfo.InvariantCulture);
                    parameters[Constants.OrderDateKey] = dateStr;

                    var quantity = options.Quantity;
                    if (options.OrderSide == OrderSide.Sell)
                    {
                        quantity = -Math.Abs(quantity);
                    }

                    parameters[Constants.QuantityKey] = quantity.ToString(CultureInfo.InvariantCulture);
                    parameters[Constants.VolumeProfileFileKey] = options.VolumeProfileFile;
                    parameters[Constants.ExecutionStrategyKey] = options.ExecutionStrategy.ToString();
                });

            var launcher = new Launcher();
            launcher.Run(parameters);
        }
    }
}
