using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using MDR_YieldmaxTools.Enums;

namespace MDR_YieldmaxTools.Tabs.Projection
{
    public class ProjectionHandler
    {
        public Symbols Symbol;
        public int DurationWeeks;
        public double InitialSharePrice;
        public double InitialVolume;
        public double InitialInvestment;
        public double DividendPerShare;
        public double SharePricePercentChange;
        public double DividendPerSharePercentChange;

        public ProjectionItem[] ProjectionItems;

        public ProjectionHandler()
        {
        }

        public static ProjectionHandler Create(Symbols _symbol, int _duration, double _sharePrice, double _volume, double _dividend, double _shareChange, double _dividendChange)
        {
            ProjectionHandler result = new ProjectionHandler
            {
                Symbol = _symbol,
                DurationWeeks = _duration,
                InitialSharePrice = _sharePrice,
                InitialVolume = _volume,
                InitialInvestment = _sharePrice * _volume,
                DividendPerShare = _dividend,
                SharePricePercentChange = _shareChange,
                DividendPerSharePercentChange = _dividendChange,
                ProjectionItems = new ProjectionItem[_duration]
            };

            return result;
        }

        public List<ProjectionItem> RunProjection()
        {
            double usd = 0;
            double volume = InitialVolume;
            double sharePrice = InitialSharePrice;
            double summation = sharePrice * volume;
            double dividend = DividendPerShare;

            double sharePriceChange = (InitialSharePrice * this.SharePricePercentChange) / DurationWeeks;
            double dividendChange = (DividendPerShare * this.DividendPerSharePercentChange) / DurationWeeks;

            for (int i = 0; i < DurationWeeks; i++)
            {
                ProjectionItem pi = new ProjectionItem(Symbol, volume, sharePrice, dividend, usd);
                pi.AverageSharePrice = summation / volume;
                ProjectionItems[i] = pi;
                usd = pi.CashUSD;
                volume += pi.DripAddVolume;
                summation += pi.DripAddVolume * sharePrice;
                sharePrice += sharePriceChange;
                dividend += dividendChange;
            }

            return this.ProjectionItems.ToList();
        }
    }
}