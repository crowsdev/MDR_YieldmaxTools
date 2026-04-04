using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using MDR_YieldmaxTools.Enums;
using MDR_YieldmaxTools.Utils;
using Telerik.WinControls;

namespace MDR_YieldmaxTools.Models
{
    public class TickData1DTo1WK
    {
        public int TotalDays = 0;
        public double FinalClosePrice;
        public double PreviousFinalClosePrice;
        public double Highest = 0;
        public double Lowest = 999999999999;

        public double GetTypicalPrice()
        {
            double summation = this.Highest + this.Lowest + this.FinalClosePrice;
            return summation / 3.0;
        }

        public double NAVChangePercent()
        {
            double num0 = this.PreviousFinalClosePrice;
            double num1 = this.FinalClosePrice;

            return SussMath.GetPercentChange(num0, num1);
        }

        public void Reset()
        {
            TotalDays = 0;
            this.PreviousFinalClosePrice = this.FinalClosePrice;
            this.FinalClosePrice = 0;
            this.Highest = 0;
            this.Lowest = 999999999999;
        }

        public void AddDay(HistoricalTickData1D _day)
        {
            TotalDays++;

            this.FinalClosePrice = _day.close ?? 0;
            double h = _day.high ?? 0;
            double l = _day.low ?? 0;
            if (h > this.Highest) this.Highest = h;
            if (l < this.Lowest) this.Lowest = l;
        }
    }
}