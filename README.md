# AlgoTrading

# How to Build & Run
You can open and build the project by opening Algo/Algo.sln in Visual Studio.
It consists of a submodule QuantConnect/Lean - https://github.com/QuantConnect/Lean which is the primary framework we use for backtesting and reading historic data.
You may need to run `git pull --recurse-submodules` instead of the normal `git pull`.
See https://stackoverflow.com/questions/1030169/easy-way-to-pull-latest-of-all-git-submodules if you need further help.

There are 2 ConsoleApp projects that you can run directly.

Algo.VolumeProfile.ConsoleApp - Used to generate a volume profile for the vwap execution algorithm to run.
e.g. .\Algo.VolumeProfile.ConsoleApp.exe -s SPY -d 2013-10-04

Algo.VwapExecution.ConsoleApp - The actual algorithm
.\Algo.VwapExecution.ConsoleApp.exe -s SPY --volume-profile-date 2013-10-04 --date 2013-10-07 --side Buy -q 1000000 --execution-strategy Aggressive3

You may want to look for the something like "Debug: Target Quantity=1000000. Quantity Filled=1000000. Average Price=166.019518238580536. Vwap=153.458267637951" at the end of the execution.

# Troubleshoot
- The example command are supposed to be run in the same directory of the binary, e.g. "{project}/bin/Debug". You need to pass the path parameters otherwise.
- You must run the VolumeProfile app to generate the profile first before running the algorithm.
- Some of the framework projects depend on some bundled binaries, if you see errors like missing CSharpAPI / Python.Runtime, you may need to manually add the reference in   Visual Studio. You can find the files in Lean/packages.

# Vwap Execution Algorithm Strategies
  A couple of execution strategy can be configured, but only one
  of them is working properly at the moment.

  Aggressive3 - Emit Market Order according to the volume profile

  WIP strategies
  Aggressive2 - Emit Limit Order at Best Ask for buy orders.
  (WIP)         Or at Best Bid for sell orders

                A maximum number of orders can be set. Orders at
                less favorable price are cancelled and the quantity
                is added to the next limit order.

  Aggressive1 - Emit Limit Order at Best Bid for buy orders.
  (WIP)         Or at Best Ask for sell orders.

  Standard    - Emit Limit Order at Vwap price.
  (WIP)

# Implementation Overview
Algo.LeanEngineLauncher - A wrapper of the Lean framework. Exposes a single class Launcher.
For each of the 2 console apps, there are 4 related projects,
1. Console app project, simple console app to pass user parameters and launch the framework
2. QCAlgorithmAdaptor project, adapts the library project to the framework
3. Library project, the main implementation of the algorithm
4. Library test project, tests for the library project

All QuantConnect* projects are from the Lean submodule.

The console app uses a config.json to specify the algorithm and corresponding dll to be launched by the framework.
See "algorithm-type-name", and "algorithm-location".

We have to abuse the the framework a bit as it is not primarily designed for an execution algorithm. Instead we just use it to feed data to our algorithm.

The flow of an algorithm in the framework (See http://quantconnect.com/docs/algorithm-framework/overview#Overview-Important-Terminology):
Market Data --> Alpha Model --> Portfolio Construction Model --> Execution Model

  1. Market Data is fed to the Alpha Model, which generate Insights

  2. Insights are passed to the Portfolio Construction Model, which generates PortfolioTargets.

  3. PortfolioTargets are passed to the ExecutionModel which sends out actual orders.

For the volume profile generation algorithm, we use empty models for everything, and just stream the data to our VolumeProfileBuilder.
For the vwap execution algorithm, we use a dummy Alpha Model which emits preconfigured Insights to represent the input vwap order, and implements the PortfolioConstructionModel and ExecutionModel correspondingly.
