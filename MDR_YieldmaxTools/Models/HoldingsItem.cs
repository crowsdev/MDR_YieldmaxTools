using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MDR_YieldmaxTools.Enums;
using MDR_YieldmaxTools.Utils;

namespace MDR_YieldmaxTools.Models
{
    [Serializable]
    public class HoldingsItem : INotifyPropertyChanged
    {
        #region Columns

        private Symbols symbol;
        public Symbols Symbol
        {
            get
            {
                return symbol;
            }
            set
            {
                if (symbol != value)
                {
                    symbol = value;
                    OnPropertyChanged("Symbol");
                }
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                if (date != value)
                {
                    date = value;
                    OnPropertyChanged("Date");
                }
            }
        }

        private double volume;
        public double Volume
        {
            get
            {
                return volume;
            }
            set
            {
                if (volume != value)
                {
                    volume = value;
                    OnPropertyChanged("Volume");
                }
            }
        }

        private double pricePerUnit;
        public double PricePerUnit
        {
            get
            {
                return pricePerUnit;
            }
            set
            {
                if (pricePerUnit != value)
                {
                    pricePerUnit = value;
                    OnPropertyChanged("PricePerUnit");
                }
            }
        }

        private double totalCost;
        public double TotalCost {
            get
            {
                return totalCost;
            }
            set
            {
                if (totalCost != value)
                {
                    totalCost = value;
                    OnPropertyChanged("TotalCost");
                }
            } 
        }

        private double marketPrice;
        public double MarketPrice 
        {
            get
            {
                return marketPrice;
            }
            set
            {
                if (marketPrice  != value)
                {
                    marketPrice = value;
                    OnPropertyChanged("MarketPrice");
                }
            }
        }

        private double totalValue;
        public double TotalValue 
        {
            get
            {
                return totalValue;
            }
            set
            {
                if (totalValue != value)
                {
                    totalValue = value;
                    OnPropertyChanged("TotalValue");
                }
            }
        }

        public double nAV_PL_percent;
        public double NAV_PL_percent 
        {
            get
            {
                return nAV_PL_percent;
            }
            set
            {
                if (nAV_PL_percent != value)
                {
                    nAV_PL_percent = value;
                    OnPropertyChanged("NAV_PL_percent");
                }
            }
        }
        
        private int dividendCount;
        public int DividendCount
        {
            get
            {
                return  dividendCount;
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

        private double overallPosition;
        public double OverallPosition 
        {
            get
            {
                return overallPosition;
            }
            set
            {
                if (overallPosition != value)
                {
                    overallPosition = value;
                    OnPropertyChanged("OverallPosition");
                }
            }
        }

        private double profitFactor;

        public double ProfitFactor
        {
            get
            {
                return profitFactor;
            }
            set
            {
                if (profitFactor != value)
                {
                    profitFactor = value;
                    OnPropertyChanged("ProfitFactor");
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

        #endregion

        #region Account Cash balance.

        public double USDHoldings = 0;

        #endregion


        public HoldingsItem()
        {
        }

        public static HoldingsItem Create(HoldingsItem _holdingsItem)
        {
            HoldingsItem result = new HoldingsItem
            {
                Symbol = _holdingsItem.Symbol,
                Date = _holdingsItem.Date,
                PricePerUnit = 0,
                Volume = 0,
                TotalCost = 0,
                Drip = _holdingsItem.Drip,
            };

            result.AddHoldingsItem(_holdingsItem);
            return result;
        }

        public static HoldingsItem Create(Symbols _symbol, DateTime _date, double _pricePerUnit, double _volume, bool _drip)
        {
            HoldingsItem inputItem = new HoldingsItem
            {
                Symbol = _symbol,
                Date = _date,
                Volume = _volume,
                PricePerUnit = _pricePerUnit,
                TotalCost = _volume * _pricePerUnit,
                Drip = _drip,
                InitialInvestment = _volume * _pricePerUnit,
            };
            
            return HoldingsItem.Create(inputItem);
        }

        public void AddHoldingsItem(HoldingsItem _holdingsItem)
        {
            InitialInvestment += _holdingsItem.InitialInvestment;
            if (Drip)
            {
                RunMarketSimulation(_holdingsItem);
                return;
            }

            double addPricePerUnit = _holdingsItem.PricePerUnit;
            double addVolume = _holdingsItem.Volume;
            DateTime addDate = _holdingsItem.Date;

            this.RecalculatePricePerUnit(addPricePerUnit, addVolume);
            this.RecalculateTotalDividends(addDate, addVolume);
            this.CalculateNAVPL();
            this.FinalCalculations();
        }

        private void RecalculatePricePerUnit(double _addPricePerUnit, double _addVolume)
        {
            double addTotalCost = _addVolume * _addPricePerUnit;
            TotalCost += addTotalCost;
            Volume += _addVolume;
            if (Volume == 0) return;
            PricePerUnit = TotalCost / Volume;
        }

        // Try to calculate for drip.
        private void RunMarketSimulation(HoldingsItem _holdingsItem)
        {
            this.Symbol = _holdingsItem.Symbol;
            this.Date = _holdingsItem.Date;
            this.PricePerUnit = _holdingsItem.PricePerUnit;
            this.Volume = _holdingsItem.Volume;
            this.TotalCost = PricePerUnit * Volume;
            this.USDHoldings = 0.00;
            

            using (var db = new dbDataContext())
            {
                List<HistoricalTickData1D> dailyData = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{this.Symbol}" && x.timestamp >= this.Date).OrderBy(y => y.timestamp).ToList();
                List<HistoricalDividendData> divData = db.HistoricalDividendDatas.Where(x => x.symbol == $"{this.Symbol}" && x.timestamp >= this.Date).OrderBy(y => y.timestamp).ToList();
                List<HistoricalStockSplit> splitData = db.HistoricalStockSplits.Where(x => x.symbol == $"{this.Symbol}" && x.timestamp >= this.Date).OrderBy(y => y.timestamp).ToList();

                foreach (HistoricalTickData1D data1D in dailyData)
                {
                    var cl = data1D.close ?? 0;
                    if (cl == 0) return;

                    if (divData.Any(x => x.timestamp.Date == data1D.timestamp.Date))
                    {
                        this.DividendCount++;
                        var dd = divData.Single(x => x.timestamp.Date == data1D.timestamp.Date);
                        var div = dd.dividend ?? 0;
                        if (div == 0)
                        {
                            return;
                        }

                        double divPaid = (Volume * div);
                        this.TotalDividends += divPaid;
                        USDHoldings += divPaid;
                        if (USDHoldings >= cl)
                        {
                            int v = (int)Math.Floor(USDHoldings / cl);
                            USDHoldings -= v * cl;
                            RecalculatePricePerUnit(cl, v);
                        }
                    }

                    // Do splits after div ??
                    // if (splitData.Any(x => x.timestamp.Date == data1D.timestamp.Date))
                    // {
                    //     var split = splitData.Single(x => x.timestamp.Date == data1D.timestamp.Date);
                    //     var num = split.numerator;
                    //     var denom = split.denominator;
                    //     Volume = (Volume / denom) * num;
                    // }
                    /*
                     * API data is backwards adjusted so splits are already factored in.
                     */

                }
            }

            CalculateNAVPL();
            FinalCalculations();
        }


        private void RecalculateTotalDividends(DateTime _addDate, double _addVolume)
        {
            HistoricalDividendData[] dbData = Array.Empty<HistoricalDividendData>();

            using (var db = new dbDataContext())
            {
                dbData = db.HistoricalDividendDatas.Where(x => x.symbol == $"{this.Symbol}" && x.timestamp >= _addDate).ToArray();

                double cumulativeDiv = 0;

                for (int i = 0; i < dbData.Length; i++)
                {
                    double divValue = dbData[i].dividend ?? -1;
                    if (divValue == -1)
                    {
                        throw new Exception("Failed to get value from HistoricalDividendData db item.");
                    }
                    cumulativeDiv += divValue;
                    DividendCount++;
                }

                double addTotalDiv = cumulativeDiv * _addVolume;
                TotalDividends += addTotalDiv;
            }
        }

        private void CalculateNAVPL()
        {
            using (var db = new dbDataContext())
            {
                var latest = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{Symbol}").OrderByDescending(y => y.timestamp).First();
                MarketPrice = latest.close ?? 0;
            }

            TotalValue = MarketPrice * Volume;
            double num0 = SussMath.GetPercentChange(PricePerUnit, MarketPrice);
            // NAVPL is percentage so round to 2 decimal places.
            NAV_PL_percent = Math.Round(num0, 2);
        }

        private void FinalCalculations()
        {
            OverallPosition = TotalValue - TotalCost + TotalDividends;
            ProfitFactor = Math.Round((OverallPosition / TotalCost), 2);
        }

        public void Clear()
        {
            Volume = 0;
            PricePerUnit = 0;
            TotalCost = 0;
            MarketPrice = 0;
            TotalValue = 0;
            NAV_PL_percent = 0;
            DividendCount = 0;
            TotalDividends = 0;
            OverallPosition = 0;
        }

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
    }
}