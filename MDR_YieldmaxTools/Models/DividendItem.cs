using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MDR_YieldmaxTools.Enums;

namespace MDR_YieldmaxTools.Models
{
    public class DividendItem : INotifyPropertyChanged
    {
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

        private double sharePrice;
        public double SharePrice
        {
            get
            {
                return sharePrice;
            }
            set
            {
                if (sharePrice != value)
                {
                    sharePrice = value;
                    OnPropertyChanged("SharePrice");
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

        private double dividendPerShare;
        public double DividendPerShare
        {
            get
            {
                return dividendPerShare;
            }
            set
            {
                if (dividendPerShare != value)
                {
                    dividendPerShare = value;
                    OnPropertyChanged("DividendPerShare");
                }
            }
        }

        private double dividendReceived;
        public double DividendReceived
        {
            get
            {
                return dividendReceived;
            }
            set
            {
                if (dividendReceived != value)
                {
                    dividendReceived = value;
                    OnPropertyChanged("DividendReceived");
                }
            }
        }

        private double divPerInvestedDollar;
        public double DivPerInvestedDollar
        {
            get
            {
                return divPerInvestedDollar;
            }
            set
            {
                if (divPerInvestedDollar != value)
                {
                    divPerInvestedDollar = value;
                    OnPropertyChanged("DivPerInvestedDollar");
                }
            }
        }
        // private string parentID;
        // public string ParentID
        // {
        //     get
        //     {
        //         return parentID;
        //     }
        //     set
        //     {
        //         if (parentID != value)
        //         {
        //             parentID = value;
        //             OnPropertyChanged("ParentID");
        //         }
        //     }
        // }

        public string ParentID { get; set; }

        public DividendItem(HistoricalDividendData _divData, HoldingsItem0 _holdingsItem0, double _sharePrice)
        {
            this.ParentID = _holdingsItem0.ID;
            this.Symbol = (Symbols)Enum.Parse(typeof(Symbols), _divData.symbol);
            this.timestamp = _divData.timestamp;
            this.SharePrice = _sharePrice;
            this.DividendPerShare = _divData.dividend ?? 0;
            this.TotalVolume = _holdingsItem0.TotalVolume;
            this.DividendReceived = this.DividendPerShare * this.TotalVolume;
            this.DivPerInvestedDollar = (_divData.dividend ?? 0) / _holdingsItem0.InitialSharePrice;


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
