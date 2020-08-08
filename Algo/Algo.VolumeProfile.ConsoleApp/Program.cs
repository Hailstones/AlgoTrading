using System;
using System.Collections.Generic;
using System.Globalization;
using Algo.LeanEngineLauncher;
using Algo.VolumeProfileBuilder;
using CommandLine;

namespace Algo.VolumeProfile.ConsoleApp
{
    class Program
    {
        public class Options
        {
            [Option('s', "symbol", Required = true, HelpText = "The symbol for the volume profile")]
            public string Symbol { get; set; }

            [Option('d', "reference-date", Required = true, HelpText = "yyyy-MM-dd. Reference Date used to generate the volume profile")]
            public DateTime ReferenceDate { get; set; }

            [Option('o', "output-file", Required=false, HelpText = "The output file path relative to the binary")]
            public string OutputFile { get; set; }
        }

        static void Main(string[] args)
        {
            var parameters = new Dictionary<string, string>();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    var date = options.ReferenceDate;
                    var dateStr = date.ToString("O", CultureInfo.InvariantCulture);
                    parameters[Constants.ReferenceDateKey] = dateStr;
                    parameters[Constants.SymbolKey] = options.Symbol;
                    parameters[Constants.OutputFileKey] = options.OutputFile;
                });

            var launcher = new Launcher();
            launcher.Run(parameters);
        }
    }
}
