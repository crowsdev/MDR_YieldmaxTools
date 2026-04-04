using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MDR_YieldmaxTools.Enums;
using MDR_YieldmaxTools.Models;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI;
using Telerik.Windows.Diagrams.Core;

namespace MDR_YieldmaxTools.Tabs.Holdings
{
    public class HoldingsHeirarchicalGrid
    {
        public BindingList<HoldingsItem0> DataSourceParent;
        public BindingList<DividendItem> DataSourceChild;
        private List<DividendItem> ChildItems;

        public HoldingsHeirarchicalGrid()
        {
            InitTab();
        }

        public void InitTab()
        {
            this.InitDataSources();
        }

        public List<HoldingsItem0> GetDataSource()
        {
            List<HoldingsItem0> result = new List<HoldingsItem0>();

            foreach (var htd in HoldingsHandler.Instance.CurrentProfile.Items)
            {
                HoldingsItem0 hi0 = new HoldingsItem0(htd);
                result.Add(hi0);
                // this.AddToDataSourceChild(hi0.ChildItems);
                this.ChildItems.AddRange(hi0.ChildItems);
            }

            return result;
        }

        public void AddToDataSourceChild(List<DividendItem> _divItems)
        {
            this.DataSourceChild.AddRange(_divItems);
        }
        
        public void InitDataSources()
        {
            ChildItems = new List<DividendItem>();
            // this.DataSourceChild = new BindingList<DividendItem>();
            // this.DataSourceChild.AllowEdit = true;
            // this.DataSourceChild.AllowNew = true;
            // this.DataSourceChild.AllowRemove = true;
            // this.DataSourceChild.RaiseListChangedEvents = true;
            this.DataSourceParent = new BindingList<HoldingsItem0>(GetDataSource());
            this.DataSourceChild = new BindingList<DividendItem>(this.ChildItems);
        }

        public void AddHoldingsItem(Symbols _symbol, DateTime _date, double _pricePerUnit, double _volume, bool _drip)
        {
            HoldingsTransactionData htd = new HoldingsTransactionData
            {
                Symbol = _symbol,
                Timestamp = _date,
                PricePerShare = _pricePerUnit,
                Volume = _volume,
                Drip = _drip
            };

            HoldingsHandler.Instance.CurrentProfile.AddItem(htd);
        }

        public void DeleteSelectedHoldings(List<GridViewRowInfo> _selectedRows)
        {
            if (_selectedRows.Count == 0) return;
            
            foreach (GridViewRowInfo row in _selectedRows)
            {
                if (row.DataBoundItem is not HoldingsItem0 boundObject)
                {
                    throw new Exception("ERROR. Failed to get HoldingsItem from row.");
                }

                List<GridViewRowInfo> childRowsToRemove = new List<GridViewRowInfo>();

                childRowsToRemove.Add(row);

                foreach (GridViewRowInfo childRowToRemove in childRowsToRemove)
                {
                    HoldingsItem0 hi = childRowToRemove.DataBoundItem as HoldingsItem0;
                    HoldingsHandler.Instance.CurrentProfile.Items.Remove(hi.Transaction);
                }
            }
        }
    }
}