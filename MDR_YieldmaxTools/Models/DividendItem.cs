using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MDR_YieldmaxTools.Enums;
using MDR_YieldmaxTools.Utils;

namespace MDR_YieldmaxTools.Models
{
    public class DividendItem
    {
        public Symbols Symbol;
        public DateTime Timestamp;
        public double SharePrice;
        public double TotalVolume;
        public double DividendPerShare;
        public double DividendReceived;
        public double AvgDivReceived;
        public double DivPerInvestedDollar;
        public string ParentID;

        public DividendItem(HistoricalDividendData _divData, HoldingsItem0 _holdingsItem0, double _sharePrice)
        {
            this.ParentID = _holdingsItem0.ID;
            this.Symbol = (Symbols)Enum.Parse(typeof(Symbols), _divData.symbol);
            this.Timestamp = _divData.timestamp;
            this.SharePrice = _sharePrice;
            this.DividendPerShare = _divData.dividend ?? 0;
            this.TotalVolume = _holdingsItem0.TotalVolume;
            this.DividendReceived = this.DividendPerShare * this.TotalVolume;
            this.DivPerInvestedDollar = (_divData.dividend ?? 0) / _holdingsItem0.InitialSharePrice;
        }
    }
}
