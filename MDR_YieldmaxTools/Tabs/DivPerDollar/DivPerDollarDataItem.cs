using System;
using MDR_YieldmaxTools.Enums;

namespace MDR_YieldmaxTools.Tabs.DivPerDollar
{
    public class DivPerDollarDataItem
    {
        public Symbols Symbol;
        public DateTime Timestamp;
        public decimal LastClose;
        public decimal TypicalPrice;
        public decimal Dividend;
        public decimal DivPerDollar;
        public decimal DivPerDol_4MA;
        public int DPD_Vs_4MA;

        public DivPerDollarDataItem()
        {
        }
    }
}