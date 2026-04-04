using System;

namespace MDR_YieldmaxTools.Utils
{
    public class SussMath
    {
        public static double GetPercentChange(double _n1, double _n2)
        {
            return ((_n2 - _n1) / _n1) * 100;
        }
    }
}