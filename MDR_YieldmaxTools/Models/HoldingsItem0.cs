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
    public class HoldingsItem0 : INotifyPropertyChanged
    {
        public HoldingsTransactionData Transaction;

        private Symbols _symbol;
        public Symbols Symbol
        {
            get
            {
                return _symbol;
            }
            set
            {
                if (_symbol != value)
                {
                    _symbol = value;
                    OnPropertyChanged("Symbol");
                }
            }
        }
        private DateTime timestamp;
        public DateTime Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                if (timestamp != value)
                {
                    timestamp = value;
                    OnPropertyChanged("Timestamp");
                }
            }
        }
        private double initialInvestment;
        public double InitialInvestment
        {
            get
            {
                return initialInvestment;
            }
            set
            {
                if (initialInvestment != value)
                {
                    initialInvestment = value;
                    OnPropertyChanged("InitialInvestment");
                }
            }
        }
        private double initialVolume;
        public double InitialVolume
        {
            get
            {
                return initialVolume;
            }
            set
            {
                if (initialVolume != value)
                {
                    initialVolume = value;
                    OnPropertyChanged("InitialVolume");
                }
            }
        }
        private double initialSharePrice;
        public double InitialSharePrice
        {
            get
            {
                return initialSharePrice;
            }
            set
            {
                if (initialSharePrice != value)
                {
                    initialSharePrice = value;
                    OnPropertyChanged("InitialSharePrice");
                }
            }
        }
        private double totalInvestment;
        public double TotalInvestment
        {
            get
            {
                return totalInvestment;
            }
            set
            {
                if (totalInvestment != value)
                {
                    totalInvestment = value;
                    OnPropertyChanged("TotalInvestment");
                }
            }
        }
        private double totalVolume;
        public double TotalVolume
        {
            get
            {
                return totalVolume;
            }
            set
            {
                if (totalVolume != value)
                {
                    totalVolume = value;
                    OnPropertyChanged("TotalVolume");
                }
            }
        }
        private double avgSharePrice;
        [Browsable(false)]
        public double AvgSharePrice
        {
            get
            {
                return avgSharePrice;
            }
            set
            {
                if (avgSharePrice != value)
                {
                    avgSharePrice = value;
                    OnPropertyChanged("AvgSharePrice");
                }
            }
        }
        private double marketSharePrice;
        public double MarketSharePrice
        {
            get
            {
                return marketSharePrice;
            }
            set
            {
                if (marketSharePrice != value)
                {
                    marketSharePrice = value;
                    OnPropertyChanged("MarketSharePrice");
                }
            }
        }
        private double currentValue;
        public double CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                if (currentValue != value)
                {
                    currentValue = value;
                    OnPropertyChanged("CurrentValue");
                }
            }
        }
        private double avgDivValue;
        public double AvgDivValue
        {
            get
            {
                return avgDivValue;
            }
            set
            {
                if (avgDivValue != value)
                {
                    avgDivValue = value;
                    OnPropertyChanged("AvgDivValue");
                }
            }
        }

        private int dividendCount;
        public int DividendCount
        {
            get
            {
                return dividendCount;
            }
            set
            {
                if (dividendCount != value)
                {
                    dividendCount = value;
                    OnPropertyChanged("DividendCount");
                }
            }
        }
        private double totalDividends;
        public double TotalDividends
        {
            get
            {
                return totalDividends;
            }
            set
            {
                if (totalDividends != value)
                {
                    totalDividends = value;
                    OnPropertyChanged("TotalDividends");
                }
            }
        }
        private double sharePricePercChange;
        public double SharePricePercChange
        {
            get
            {
                return sharePricePercChange;
            }
            set
            {
                if (sharePricePercChange != value)
                {
                    sharePricePercChange = value;
                    OnPropertyChanged("SharePricePercChange");
                }
            }
        }
        private double profitLoss;
        public double ProfitLoss
        {
            get
            {
                return profitLoss;
            }
            set
            {
                if (profitLoss != value)
                {
                    profitLoss = value;
                    OnPropertyChanged("ProfitLoss");
                }
            }
        }
        private double profitLossPerc;
        public double ProfitLossPerc
        {
            get
            {
                return profitLossPerc;
            }
            set
            {
                if (profitLossPerc != value)
                {
                    profitLossPerc = value;
                    OnPropertyChanged("ProfitLossPerc");
                }
            }
        }
        private bool drip;
        public bool Drip
        {
            get
            {
                return drip;
            }
            set
            {
                if (drip != value)
                {
                    drip = value;
                    OnPropertyChanged("Drip");
                }
            }
        }
        private double uSDHoldings;
        public double USDHoldings
        {
            get
            {
                return uSDHoldings;
            }
            set
            {
                if (uSDHoldings != value)
                {
                    uSDHoldings = value;
                    OnPropertyChanged("USDHoldings");
                }
            }
        }

        private string iD;
        public string ID
        {
            get
            {
                return iD;
            }
            set
            {
                if (iD != value)
                {
                    iD = value;
                    OnPropertyChanged("ID");
                }
            }
        }


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

                        this.ChildItems.Add(new DividendItem(dd, this, lastClose));
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
                ProfitLossPerc = Math.Round(SussMath.GetPercentChange(InitialInvestment, CurrentValue), 2);
                SharePricePercChange = Math.Round(SussMath.GetPercentChange(InitialSharePrice, MarketSharePrice), 2);
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

        #region Notify for binding.

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
