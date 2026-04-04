using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDR_YieldmaxTools.Enums;

namespace MDR_YieldmaxTools.Tabs.Projection
{
    public class ProjectionItem
    {
        public Symbols Symbol;
        public double Volume;
        public double SharePrice;
        public double TotalValue;
        public double Dividend;
        public double TotalDividend;
        public double DripAddVolume;
        public double CashUSD;
        public double AverageSharePrice;


        public ProjectionItem(Symbols _symbol, double _volume, double _sharePrice, double _dividend, double _cashUsd)
        {
            Symbol = _symbol;
            Volume = _volume;
            SharePrice = _sharePrice;
            TotalValue = _sharePrice * _volume;
            Dividend = _dividend;
            TotalDividend = _dividend * _volume;
            double cashOnHand = TotalDividend + _cashUsd;
            DripAddVolume = Math.Floor(cashOnHand / SharePrice);
            CashUSD = cashOnHand - (DripAddVolume * SharePrice);
        }
    }
}
