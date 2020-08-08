namespace Algo.VwapExecution
{
    public class VwapCalculator
    {
        public void Tick(decimal indicativePrice, decimal volume)
        {
            _initialized = true;
            _totalValue += (double) (indicativePrice * volume);
            _totalVolume += volume;
        }

        public double Average
        {
            get
            {
                if (!_initialized)
                {
                    return -1;
                }

                return _totalValue / (double) _totalVolume;
            }
        }

        private bool _initialized;

        // We don't need the precision of decimal
        // Using a double prevents overflow
        private double _totalValue;
        private decimal _totalVolume;
    }
}
