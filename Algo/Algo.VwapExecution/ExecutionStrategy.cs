namespace Algo.VwapExecution
{
    // Ordered by descending level of aggressiveness
    public enum ExecutionStrategy
    {
        Aggressive3,
        Aggressive2,
        Aggressive1,
        Standard,
        BestPrice,
    }
}
