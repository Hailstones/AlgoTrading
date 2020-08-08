using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using QuantConnect;
using QuantConnect.Configuration;
using QuantConnect.Lean.Engine;
using QuantConnect.Logging;
using QuantConnect.Util;

namespace Algo.LeanEngineLauncher
{
    // Copied and modified based on QuantConnect.Lean.Launcher.Program.cs
    public class Launcher
    {
        public void Run(Dictionary<string, string> parameters)
        {
            const string mode = "DEBUG";

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var liveMode = Config.GetBool("live-mode");
            Log.DebuggingEnabled = Config.GetBool("debug-mode");
            Log.FilePath = Path.Combine(Config.Get("results-destination-folder"), "log.txt");
            Log.LogHandler = Composer.Instance.GetExportedValueByTypeName<ILogHandler>(Config.Get("log-handler", "CompositeLogHandler"));

            Log.Trace($"Engine.Main(): LEAN ALGORITHMIC TRADING ENGINE v{Globals.Version} Mode: {mode} ({(Environment.Is64BitProcess ? "64" : "32")}bit)");
            Log.Trace($"Engine.Main(): Started {DateTime.Now.ToShortTimeString()}");

            var systemHandler = LeanEngineSystemHandlers.FromConfiguration(Composer.Instance);
            systemHandler.Initialize();

            var job = systemHandler.JobQueue.NextJob(out var assemblyPath);

            foreach (var kvp in parameters)
            {
                job.Parameters[kvp.Key] = kvp.Value;
            }

            var algorithmHandler = LeanEngineAlgorithmHandlers.FromConfiguration(Composer.Instance);

            try
            {
                var algorithmManager = new AlgorithmManager(liveMode, job);

                systemHandler.LeanManager.Initialize(systemHandler, algorithmHandler, job, algorithmManager);

                var engine = new Engine(systemHandler, algorithmHandler, liveMode);
                engine.Run(job, algorithmManager, assemblyPath, WorkerThread.Instance);
            }
            finally
            {
                Console.WriteLine("Engine.Main(): Analysis Complete. Press any key to continue.");
                Console.ReadKey(true);

                systemHandler.Dispose();
                algorithmHandler.Dispose();
                Log.LogHandler.Dispose();

                Log.Trace("Program.Main(): Exiting Lean...");

                Thread.Sleep(2000);
                Environment.Exit(0);
            }
        }
    }
}
