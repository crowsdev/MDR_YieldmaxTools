using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using MDR_YieldmaxTools.Enums;
using MDR_YieldmaxTools.Tabs.Holdings;
using MDR_YieldmaxTools.Utils;

namespace MDR_YieldmaxTools.Models
{
    public class HoldingsItem0
    {
        public HoldingsTransactionData Transaction;

        public Symbols Symbol;
        
        public DateTime Timestamp;
        
        public double InitialInvestment;
        
        public double InitialVolume;
        
        public double InitialSharePrice;
        
        public double TotalInvestment;
        
        public double TotalVolume;
        
        public double AvgSharePrice;

        public double MarketSharePrice;

        public double CurrentValue;
        
        public double AvgDivValue;

        public int DividendCount;

        public double TotalDividends;

        public double SharePricePercChange;

        public double ProfitLoss;

        public double ProfitLossPerc;

        public bool Drip;

        public double USDHoldings;

        public string ID;

        public List<DividendItem> ChildItems;

        public HoldingsItem0()
        {
        }

        public HoldingsItem0(HoldingsTransactionData transaction)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Transaction = transaction;
            this.Symbol = transaction.Symbol;
            this.Timestamp = transaction.Timestamp;
            this.InitialSharePrice = this.AvgSharePrice = transaction.PricePerShare;
            this.InitialVolume = this.TotalVolume = transaction.Volume;
            this.InitialInvestment = this.TotalInvestment = InitialVolume * InitialSharePrice;
            this.Drip = transaction.Drip;
            this.ChildItems = new List<DividendItem>();

            RunMarketSimulation();
        }

        public void RunMarketSimulation()
        {
            this.USDHoldings = 0.00;

            using (var db = new dbDataContext())
            {
                List<HistoricalTickData1D> dailyData = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{this.Symbol}" && x.timestamp >= this.Timestamp).OrderBy(y => y.timestamp).ToList();
                List<HistoricalDividendData> divData = db.HistoricalDividendDatas.Where(x => x.symbol == $"{this.Symbol}" && x.timestamp >= this.Timestamp).OrderBy(y => y.timestamp).ToList();
                // List<HistoricalStockSplit> splitData = db.HistoricalStockSplits.Where(x => x.symbol == $"{this.Symbol}" && x.timestamp >= this.Timestamp).OrderBy(y => y.timestamp).ToList();

                double lastClose = 0;
                List<double> divValues = new List<double>();

                foreach (HistoricalTickData1D data1D in dailyData)
                {
                    lastClose = data1D.close ?? 0;
                    if (lastClose == 0) return;

                    if (divData.Any(x => x.timestamp.Date == data1D.timestamp.Date))
                    {
                        this.DividendCount++;
                        var dd = divData.Single(x => x.timestamp.Date == data1D.timestamp.Date);
                        var div = dd.dividend ?? 0;
                        if (div == 0)
                        {
                            MessageBox.Show("ERROR.", $"{dd.symbol} :: {dd.timestamp} :: {dd.dividend} == 0. Bad value.", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        divValues.Add(div);
                        this.AvgDivValue = divValues.Average();
                        double divPaid = (TotalVolume * div);
                        this.TotalDividends += divPaid;
                        USDHoldings += divPaid;

                        DividendItem di = new DividendItem(dd, this, lastClose);
                        di.AvgDivReceived = di.DividendReceived;
                        if (this.ChildItems.Count > 0)
                        {
                            di.AvgDivReceived = this.GetAverageDividendReceived();
                        }
                        this.ChildItems.Add(di);
                    }

                    if (this.Drip)
                    {
                        if (USDHoldings >= lastClose)
                        {
                            int v = (int)Math.Floor(USDHoldings / lastClose);
                            TotalVolume += v;
                            TotalInvestment += v * lastClose;
                            USDHoldings -= v * lastClose;
                            // Calculate the avgSharePrice at the end.
                        }
                    }
                }

                // Finalise the results.
                AvgSharePrice = TotalInvestment / TotalVolume;
                MarketSharePrice = lastClose;
                CurrentValue = TotalVolume * MarketSharePrice;
                ProfitLoss = CurrentValue - InitialInvestment;
                ProfitLossPerc = Math.Round(SussMath.GetPercentChange(InitialInvestment, CurrentValue), 0);
                SharePricePercChange = Math.Round(SussMath.GetPercentChange(InitialSharePrice, MarketSharePrice), 0);
            }
        }

        private void RecalculatePricePerUnit(double _addPricePerUnit, double _addVolume)
        {
            double addTotalCost = _addVolume * _addPricePerUnit;
            TotalInvestment += addTotalCost;
            TotalVolume += _addVolume;
            if (TotalVolume == 0) return;
            AvgSharePrice = TotalInvestment / TotalVolume;
        }

        public double GetAverageDividendReceived()
        {
            return ChildItems.Select(x => x.DividendReceived).Average();
        }
    }
}
