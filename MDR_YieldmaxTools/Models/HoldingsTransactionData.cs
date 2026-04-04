using System;
using MDR_YieldmaxTools.Enums;

namespace MDR_YieldmaxTools.Models
{
    [Serializable]
    public class HoldingsTransactionData
    {
        public Symbols Symbol;
        public DateTime Timestamp;
        public double PricePerShare;
        public double Volume;
        public bool Drip;

        public HoldingsTransactionData()
        {
        }
    }
}