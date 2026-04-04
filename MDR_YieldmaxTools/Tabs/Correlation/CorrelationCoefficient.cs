using System;

namespace MDR_YieldmaxTools.Tabs.Correlation
{
    public class CorrelationCoefficient
    {
        public string A { get; set; }
        public string B { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }

        public CorrelationCoefficient(string a, string b, double value)
        {
            A = a;
            B = b;
            Value = value;
        }

        public CorrelationCoefficient(string a, string b, double value, DateTime timestamp)
        {
            A = a;
            B = b;
            Value = value;
            Timestamp = timestamp;
        }
    }
}
