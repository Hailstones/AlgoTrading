using System;

namespace Algo.Common
{
    public static class Utilities
    {
        public static bool ApproximatelyEquals(double a, double b, double epsilon = 1e-7)
        {
            return Math.Abs(a - b) < epsilon;
        }
    }
}
