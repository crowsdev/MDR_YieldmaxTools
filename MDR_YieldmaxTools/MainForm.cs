using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.Statistics;
using MDR_YieldmaxTools.Enums;
using MDR_YieldmaxTools.Models;
using MDR_YieldmaxTools.Tabs.Correlation;
using MDR_YieldmaxTools.Tabs.CorrelationTrend;
using MDR_YieldmaxTools.Tabs.DivPerDollar;
using MDR_YieldmaxTools.Tabs.Holdings;
using MDR_YieldmaxTools.Tabs.Projection;
using MDR_YieldmaxTools.Utils;
using Telerik.Charting;
using Telerik.WinControls;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.HeatMap;
using Telerik.Windows.Diagrams.Core;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI.Export;
using AxisType = Telerik.Charting.AxisType;
using DataPoint = System.Windows.Forms.DataVisualization.Charting.DataPoint;

namespace MDR_YieldmaxTools
{
    public partial class MainForm : RadForm
    {
        #region Tabs.

        /*
         * Move stuff to here and organise for each tab
         */

        #region DivPerDollar Tab.

        public DivPerDollarHandler DpdHandler;
        public double ROIMinDivPerDollar = GlobalVars.ROIMinDivValues["6 months"];

        #endregion

        #region Holding Tab.

        public HoldingsHeirarchicalGrid GridHoldingsHeirarchical;

        #endregion

        #region Heatmap TAB.

        private CorrelationVectorsFactory CVF;
        private double[][] Vectors;
        public int TotalDaysCorrelated = 0;

        public Stack<SymbolPairPopup> SymPairPopups = new Stack<SymbolPairPopup>();


        #endregion

        #region TrendingCorrelation

        public BindingList<Symbols> TrendingCorrSymbolSelectorADataSource;
        public BindingList<Symbols> TrendingCorrSymbolSelectorBDataSource;

        #endregion

        #region Projection Tab

        public ProjectionHandler Projection;
        public bool bAutoSettingActive = false;
        public string SelectedDividendMode = "low";
        public double LowestWeeklyDivPerDollar;
        public double MinAverageDivPerDollar;
        public double SelectedDivPerDollarValue;

        #endregion

        #endregion

        // Async example.

        // private void button_Click(object sender, EventArgs e)
        // {
        //     var task = Task.Run<int>(
        //         () => this.TestSynchronous());
        // 
        //     task.Wait();
        // 
        //     MessageBox.Show(task.Result.ToString());
        // }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ThemeResolutionService.ApplicationThemeName = "Office2019Dark";
            this.Text = $"MDR Yieldmax Tools v{GlobalVars.Version}";

            Thread _t0 = new Thread(new ThreadStart(InitCustomDataSource));
            _t0.IsBackground = true;
            _t0.Start();

            InitHoldingsTab();

            // DoTest();
            Thread _t1 = new Thread(new ThreadStart(() =>
            {
                CVF = new CorrelationVectorsFactory(GlobalVars.AllSymbols);
            }));
            _t1.IsBackground = true;
            _t1.Start();
            DoRadHeatmap();

            this.InitTrendingCorrelationTab();

            this.InitProjectionTab();
        }

        #region DivPerDollarTable Tab.

        public void InitCustomDataSource()
        {
            DpdHandler = new DivPerDollarHandler();

            using (this.radGridView_divperdollartable.DeferRefresh())
            {
                foreach (DivPerDollarDataItem row in DpdHandler.Rows)
                {
                    this.radGridView_divperdollartable.Rows.Add(new object[]
                    {
                        row.Symbol.ToString(), row.Timestamp, row.LastClose, row.TypicalPrice, row.Dividend, row.DivPerDollar,
                        row.DivPerDol_4MA, row.DPD_Vs_4MA
                    });
                }
            }
            
        }


        #region ROIMinDivPerDollar.

        public void InitROIMinDpdFormatting()
        {
            this.radGridView_divperdollartable.Columns["DivPerDollar"].ConditionalFormattingObjectList.Clear();

            ConditionalFormattingObject obj = new ConditionalFormattingObject("roi", ConditionTypes.GreaterOrEqual,
                $"{this.ROIMinDivPerDollar}", "", true);
            obj.CellBackColor = Color.Lime;
            obj.CellForeColor = Color.Black;
            obj.TextAlignment = ContentAlignment.MiddleRight;

            this.radGridView_divperdollartable.Columns["DivPerDollar"].ConditionalFormattingObjectList.Add(obj);
            // this.radGridView_divperdollartable.Refresh();
        }

        private void radRadioButton_any_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            string selTxt = ((RadRadioButton)sender).Text;

            if (!GlobalVars.ROIMinDivValues.TryGetValue(selTxt, out double roiWeeksValue))
            {
                this.ROIMinDivPerDollar = GlobalVars.ROIMinDivValues["6 months"];
            }
            else
            {
                this.ROIMinDivPerDollar = roiWeeksValue;
            }

            InitROIMinDpdFormatting();
        }

        #endregion

        #endregion

        #region Tab2 Holdings.

        private void InitHoldingsTab()
        {
            this.radDropDownList_symbol.DataSource = GlobalVars.AllSymbols;
            GridHoldingsHeirarchical = new HoldingsHeirarchicalGrid();
            InitHoldingsGrid();
        }

        private void InitHoldingsGrid()
        {
            this.radGridView_holdings.MasterTemplate.Reset();
            this.radGridView_holdings.AutoGenerateHierarchy = true;

            using (this.radGridView_holdings.DeferRefresh())
            {
                Holdings_SetupMasterTemplate();

                GridViewTemplate childTemplate = Holdings_SetupChildTemplate();
                childTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                // childTemplate.ReadOnly = true;
                //  childTemplate.AllowAddNewRow = false;
                //  childTemplate.AllowDragToGroup = false;
                //  childTemplate.AllowSearchRow = false;
                

                GridViewRelation relation = new GridViewRelation(this.radGridView_holdings.MasterTemplate);
                relation.ChildTemplate = childTemplate;
                relation.RelationName = "parentchild";
                relation.ParentColumnNames.Add("ID");
                relation.ChildColumnNames.Add("ParentID");
                this.radGridView_holdings.Relations.Add(relation);

                foreach (HoldingsItem0 h in GridHoldingsHeirarchical.DataSourceParent)
                {
                    // Parent rows. HoldingsItem0.
                    // Symbol, Timestamp, InitialInvestment, InitialVolume, InitialSharePrice, TotalInvestment, TotalVolume, AvgSharePrice, MarketSharePrice, CurrentValue, AvgDivValue, DividendCount, TotalDividends, SharePricePercChange, ProfitLoss, ProfitLossPerc, Drip, USDHoldings, ID
                    this.radGridView_holdings.MasterTemplate.Rows.Add(new object[]
                    {
                        h.Symbol.ToString(), h.Timestamp, h.InitialInvestment, h.InitialVolume, h.InitialSharePrice, h.TotalInvestment, h.TotalVolume, h.AvgSharePrice, h.MarketSharePrice, h.CurrentValue, h.AvgDivValue, h.DividendCount, h.TotalDividends, h.SharePricePercChange, h.ProfitLoss, h.ProfitLossPerc, h.Drip, h.USDHoldings, h.ID
                    });

                    foreach (DividendItem d in h.ChildItems)
                    {
                        this.radGridView_holdings.MasterTemplate.Templates[0].Rows.Add(new object[]
                        {
                            d.Symbol, d.Timestamp, d.SharePrice, d.TotalVolume, d.DividendPerShare, d.DividendReceived,
                            d.AvgDivReceived, d.DivPerInvestedDollar, d.ParentID
                        });
                    }
                }

                // foreach (DividendItem d in GridHoldingsHeirarchical.ChildItems)
                // {
                //     // Child rows. DividendItem.
                //     // Symbol, Timestamp, SharePrice, TotalVolume, DividendPerShare, DividendReceived, AvgDivReceived, DivPerInvestedDollar, ParentID
                //     childTemplate.Rows.Add(new object[]
                //     {
                //         d.Symbol, d.Timestamp, d.SharePrice, d.TotalVolume, d.DividendPerShare, d.DividendReceived, d.AvgDivReceived, d.DivPerInvestedDollar, d.ParentID
                //     });
                // }
            }
        }

        private void Holdings_SetupMasterTemplate()
        {
            GridViewTextBoxColumn SymbolCol = new GridViewTextBoxColumn("Symbol");
            this.radGridView_holdings.MasterTemplate.Columns.Add(SymbolCol);

            GridViewDateTimeColumn TimestampCol = new GridViewDateTimeColumn("Timestamp");
            TimestampCol.FormatString = "{0:MM/dd/yyyy}";
            this.radGridView_holdings.MasterTemplate.Columns.Add(TimestampCol);

            GridViewDecimalColumn InitialInvestmentCol = new GridViewDecimalColumn("InitialInvestment");
            InitialInvestmentCol.FormatString = "{0:c}";
            // InitialInvestmentCol.DecimalPlaces = 2;
            this.radGridView_holdings.MasterTemplate.Columns.Add(InitialInvestmentCol);

            GridViewDecimalColumn InitialVolumeCol = new GridViewDecimalColumn("InitialVolume");
            InitialVolumeCol.DecimalPlaces = 0;
            this.radGridView_holdings.MasterTemplate.Columns.Add(InitialVolumeCol);

            GridViewDecimalColumn InitialSharePriceCol = new GridViewDecimalColumn("InitialSharePrice");
            InitialSharePriceCol.FormatString = "{0:c}";
            // InitialSharePriceCol.DecimalPlaces = 4;
            this.radGridView_holdings.MasterTemplate.Columns.Add(InitialSharePriceCol);

            GridViewDecimalColumn TotalInvestmentCol = new GridViewDecimalColumn("TotalInvestment");
            TotalInvestmentCol.FormatString = "{0:c}";
            // TotalInvestmentCol.DecimalPlaces = 2;
            this.radGridView_holdings.MasterTemplate.Columns.Add(TotalInvestmentCol);

            GridViewDecimalColumn TotalVolumeCol = new GridViewDecimalColumn("TotalVolume");
            TotalVolumeCol.DecimalPlaces = 0;
            this.radGridView_holdings.MasterTemplate.Columns.Add(TotalVolumeCol);

            GridViewDecimalColumn AvgSharePriceCol = new GridViewDecimalColumn("AvgSharePrice");
            AvgSharePriceCol.FormatString = "{0:c}";
            // AvgSharePriceCol.DecimalPlaces = 4;
            AvgSharePriceCol.IsVisible = false;
            this.radGridView_holdings.MasterTemplate.Columns.Add(AvgSharePriceCol);

            GridViewDecimalColumn MarketSharePriceCol = new GridViewDecimalColumn("MarketSharePrice");
            MarketSharePriceCol.FormatString = "{0:c}";
            // MarketSharePriceCol.DecimalPlaces = 4;
            this.radGridView_holdings.MasterTemplate.Columns.Add(MarketSharePriceCol);

            GridViewDecimalColumn CurrentValueCol = new GridViewDecimalColumn("CurrentValue");
            CurrentValueCol.FormatString = "{0:c}";
            // CurrentValueCol.DecimalPlaces = 2;
            this.radGridView_holdings.MasterTemplate.Columns.Add(CurrentValueCol);

            GridViewDecimalColumn AvgDivValueCol = new GridViewDecimalColumn("AvgDivValue");
            AvgDivValueCol.FormatString = "{0:c}";
            // AvgDivValueCol.DecimalPlaces = 4;
            this.radGridView_holdings.MasterTemplate.Columns.Add(AvgDivValueCol);

            GridViewDecimalColumn DividendCountCol = new GridViewDecimalColumn("DividendCount");
            DividendCountCol.DecimalPlaces = 0;
            this.radGridView_holdings.MasterTemplate.Columns.Add(DividendCountCol);

            GridViewDecimalColumn TotalDividendsCol = new GridViewDecimalColumn("TotalDividends");
            TotalDividendsCol.FormatString = "{0:c}";
            // TotalDividendsCol.DecimalPlaces = 2;
            this.radGridView_holdings.MasterTemplate.Columns.Add(TotalDividendsCol);

            GridViewDecimalColumn SharePricePercChangeCol = new GridViewDecimalColumn("SharePricePercChange");
            SharePricePercChangeCol.DecimalPlaces = 2;
            this.radGridView_holdings.MasterTemplate.Columns.Add(SharePricePercChangeCol);

            GridViewDecimalColumn ProfitLossCol = new GridViewDecimalColumn("ProfitLoss");
            ProfitLossCol.FormatString = "{0:c}";
            // ProfitLossCol.DecimalPlaces = 2;
            this.radGridView_holdings.MasterTemplate.Columns.Add(ProfitLossCol);

            GridViewDecimalColumn ProfitLossPercCol = new GridViewDecimalColumn("ProfitLossPerc");
            ProfitLossPercCol.FormatString = "{0:c}";
            // ProfitLossPercCol.DecimalPlaces = 2;
            this.radGridView_holdings.MasterTemplate.Columns.Add(ProfitLossPercCol);

            GridViewCheckBoxColumn DripCol = new GridViewCheckBoxColumn("Drip");
            this.radGridView_holdings.MasterTemplate.Columns.Add(DripCol);

            GridViewDecimalColumn USDHoldingsCol = new GridViewDecimalColumn("USDHoldings");
            USDHoldingsCol.FormatString = "{0:c}";
            // USDHoldingsCol.DecimalPlaces = 2;
            this.radGridView_holdings.MasterTemplate.Columns.Add(USDHoldingsCol);

            GridViewTextBoxColumn IDCol = new GridViewTextBoxColumn("ID");
            IDCol.Name = "ID";
            IDCol.HeaderText = "ID";
            // IDCol.IsVisible = false;
            this.radGridView_holdings.MasterTemplate.Columns.Add(IDCol);

            this.radGridView_holdings.MasterTemplate.ReadOnly = true;
            this.radGridView_holdings.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
        }

        private GridViewTemplate Holdings_SetupChildTemplate()
        {
            GridViewTemplate result = new GridViewTemplate();

            GridViewTextBoxColumn symbolCol = new GridViewTextBoxColumn("Symbol");
            symbolCol.Name = "Symbol";
            result.Columns.Add(symbolCol);

            GridViewDateTimeColumn timestampCol = new GridViewDateTimeColumn("Timestamp");
            timestampCol.Name = "Timestamp";
            timestampCol.FormatString = "{0:MM/dd/yyyy}";
            result.Columns.Add(timestampCol);

            GridViewDecimalColumn sharepriceCol = new GridViewDecimalColumn("SharePrice");
            sharepriceCol.Name = "SharePrice";
            sharepriceCol.FormatString = "{0:c}";
            sharepriceCol.DecimalPlaces = 4;
            result.Columns.Add(sharepriceCol);

            GridViewDecimalColumn totalvolumeCol = new GridViewDecimalColumn("TotalVolume");
            totalvolumeCol.Name = "TotalVolume";
            totalvolumeCol.DecimalPlaces = 0;
            result.Columns.Add(totalvolumeCol);

            GridViewDecimalColumn dividendPerShareCol = new GridViewDecimalColumn("DividendPerShare");
            dividendPerShareCol.Name = "DividendPerShare";
            dividendPerShareCol.FormatString = "{0:c}";
            // dividendPerShareCol.DecimalPlaces = 4;
            result.Columns.Add(dividendPerShareCol);

            GridViewDecimalColumn dividendReceivedCol = new GridViewDecimalColumn("DividendReceived");
            dividendReceivedCol.Name = "DividendReceived";
            dividendReceivedCol.FormatString = "{0:c}";
            // dividendReceivedCol.DecimalPlaces = 2;
            result.Columns.Add(dividendReceivedCol);

            GridViewDecimalColumn avgDivReceivedCol = new GridViewDecimalColumn("AvgDivReceived");
            avgDivReceivedCol.Name = "AvgDivReceived";
            avgDivReceivedCol.FormatString = "{0:c}";
            // avgDivReceivedCol.DecimalPlaces = 2;
            result.Columns.Add(avgDivReceivedCol);

            GridViewDecimalColumn divPerInvestedDollarCol = new GridViewDecimalColumn("DivPerInvestedDollar");
            divPerInvestedDollarCol.Name = "DivPerInvestedDollar";
            divPerInvestedDollarCol.FormatString = "{0:c}";
            // divPerInvestedDollarCol.DecimalPlaces = 4;
            result.Columns.Add(divPerInvestedDollarCol);

            GridViewTextBoxColumn parentIDCol = new GridViewTextBoxColumn("ParentID");
            parentIDCol.Name = "ParentID";
            parentIDCol.HeaderText = "ParentID";
            // parentIDCol.IsVisible = true;
            result.Columns.Add(parentIDCol);

            this.radGridView_holdings.Templates.Add(result);

            return result;
        }

        #endregion

        private void radButton_add_Click(object sender, EventArgs e)
        {
            Symbols s = (Symbols)Enum.Parse(typeof(Symbols), radDropDownList_symbol.Items[this.radDropDownList_symbol.SelectedIndex].Value.ToString());
            DateTime d = this.radDateTimePicker_date.Value;
            double p = (double)this.numericUpDown_price.Value;
            double v = (double)this.numericUpDown_volume.Value;
            bool drp = checkBox_drip.Checked;

            using (radGridView_holdings.DeferRefresh())
            {
                GridHoldingsHeirarchical.AddHoldingsItem(s, d, p, v, drp);
                // radGridView_holdings.DataSource = null;
                // radGridView_holdings.Templates[0].DataSource = null;
                GridHoldingsHeirarchical.InitDataSources();
                InitHoldingsGrid();
                // radGridView_holdings.DataSource = GridHoldingsHeirarchical.DataSourceParent;
                // radGridView_holdings.Templates[0].DataSource = GridHoldingsHeirarchical.DataSourceChild;
                // radGridView_holdings.MasterTemplate.Refresh();
            }

            this.numericUpDown_price.Value = 0;
            this.numericUpDown_volume.Value = 0;
            this.radDropDownList_symbol.SelectedIndex = 0;
        }

        private void radButton_del_Click(object sender, EventArgs e)
        {
            HoldingsTransactionData htd = null;
            var timestamp = (DateTime) this.radGridView_holdings.SelectedRows[0].Cells["Timestamp"].Value;
            var symbol = (Symbols) Enum.Parse(typeof(Symbols), this.radGridView_holdings.SelectedRows[0].Cells["Symbol"].Value.ToString());
            double pps = double.Parse(this.radGridView_holdings.SelectedRows[0].Cells["InitialSharePrice"].Value.ToString());
            var vol = double.Parse(this.radGridView_holdings.SelectedRows[0].Cells["InitialVolume"].Value.ToString());
            if (HoldingsHandler.Instance.CurrentProfile.Items.Any(x => x.Timestamp == timestamp && x.Symbol == symbol && x.PricePerShare == pps && x.Volume == vol))
            {
                htd = HoldingsHandler.Instance.CurrentProfile.Items.Single(x => x.Timestamp == timestamp && x.Symbol == symbol && x.PricePerShare == pps && x.Volume == vol);
            }

            // Delete selected holdings.
            GridHoldingsHeirarchical.DeleteSelectedHoldings(htd);
            // radGridView_holdings.DataSource = null;
            // foreach (GridViewTemplate template in this.radGridView_holdings.Templates)
            // {
            //     template.DataSource = null;
            // }
            GridHoldingsHeirarchical.InitDataSources();
            InitHoldingsGrid();
            // radGridView_holdings.DataSource = GridHoldingsHeirarchical.DataSourceParent;
            // radGridView_holdings.Templates[0].DataSource = GridHoldingsHeirarchical.DataSourceChild;
            // radGridView_holdings.Refresh();


        }

        #region Correlation HeatMap

        private void DoRadHeatmap()
        {
            this.radHeatMap1.RowHeaderWidth = 55;

            CategoricalDefinition categoricalDefinition = new CategoricalDefinition();
            categoricalDefinition.RowGroupMember = nameof(CorrelationCoefficient.A);
            categoricalDefinition.ColumnGroupMember = nameof(CorrelationCoefficient.B);
            categoricalDefinition.ValueMember = nameof(CorrelationCoefficient.Value);
            // categoricalDefinition.DataSource = GetRadHeatmapDataSource();
            categoricalDefinition.DataSource = GetRadHeatmapDataSource(20);
            categoricalDefinition.Colorizer = new HeatMapRangeColorizer
            {
                IsAbsolute = true,
                Colors = new HeatMapRangeColorCollection()
                {
                    new HeatMapRangeColor() { From = -1, To = -0.66, Color = Color.Red },
                    new HeatMapRangeColor() { From = -0.66, To = -0.33, Color = Color.DarkSlateGray },
                    new HeatMapRangeColor() { From = -0.33, To = 0, Color = Color.DarkSlateGray },
                    new HeatMapRangeColor() { From = 0, To = 0.33, Color = Color.DarkSlateGray },
                    new HeatMapRangeColor() { From = 0.33, To = 0.66, Color = Color.DarkSlateGray },
                    new HeatMapRangeColor() { From = 0.66, To = 1, Color = Color.LimeGreen },
                }
            };
            this.radHeatMap1.Definition = categoricalDefinition;
            this.radHeatMap1.AllowSelection = true;
        }

        // Need to redo this method.
        private BindingList<CorrelationCoefficient> GetRadHeatmapDataSource()
        {
            BindingList<CorrelationCoefficient> result = new BindingList<CorrelationCoefficient>();

            #region Build DataSource

            int totalSymbols = GlobalVars.AllSymbols.Count;

            for (int i = 0; i < totalSymbols; i++)
            {

                Symbols symA = GlobalVars.AllSymbols[i];
                IEnumerable<double> vecA = this.CVF.SymbolDataMap[symA].Select(x => x.close ?? 0);
                var dataA = vecA.ToList();
                TotalDaysCorrelated = dataA.Count();

                for (int j = 0; j < totalSymbols; j++)
                {
                    Symbols symB = GlobalVars.AllSymbols[j];
                    IEnumerable<double> vecB = this.CVF.SymbolDataMap[symB].Select(x => x.close ?? 0);
                    var dataB = vecB.ToList();
                    int num0 = dataB.Count();
                    if (num0 != TotalDaysCorrelated)
                    {
                        RadMessageBox.Show(
                            $"{symB} as symbol B has {num0} days of data but {symA} as symbol A has {TotalDaysCorrelated}.");
                    }

                    double pco = Correlation.Pearson(dataA, dataB);
                    result.Add(new CorrelationCoefficient($"{symA}", $"{symB}", pco));
                }
            }

            #endregion

            return result;

        }

        private BindingList<CorrelationCoefficient> GetRadHeatmapDataSource(int numOfDays)
        {
            List<CorrelationCoefficient> result = new List<CorrelationCoefficient>();

            using (var db = new dbDataContext())
            {
                Symbols[] symbolsA = GlobalVars.AllSymbols.ToArray();
                Symbols[] symbolsB = GlobalVars.AllSymbols.ToArray();

                for (int i = 0; i < symbolsA.Length; i++)
                {
                    Symbols symA = symbolsA[i];

                    for (int j = 0; j < symbolsB.Length; j++)
                    {
                        Symbols symB = symbolsB[j];

                        List<HistoricalTickData1D> dbDataA = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{symA}").ToList();
                        List<HistoricalTickData1D> dbDataB = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{symB}").ToList();

                        var trimmedA = dbDataA.Skip(dbDataA.Count - numOfDays).OrderBy(x => x.timestamp).ToList();
                        var trimmedB = dbDataB.Skip(dbDataB.Count - numOfDays).OrderBy(x => x.timestamp).ToList();

                        CorrelationCoefficient cc = new CorrelationCoefficient(symA.ToString(), symB.ToString(), Correlation.Pearson(trimmedA.Select(x => x.close ?? 0), trimmedB.Select(x => x.close ?? 0)));
                        result.Add(cc);
                    }
                }
            }

            return new BindingList<CorrelationCoefficient>(result);
        }


        private void radHeatMap1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
                if (this.SymPairPopups.Count > 0)
                {
                    this.SymPairPopups.Pop().Close();
                }
            }
        }

        private void radHeatMap1_SelectedCellIndexChanged_1(object sender, HeatMapIndexChangedEventArgs e)
        {
            if (this.radHeatMap1.SelectedCellIndex.ColumnIndex < 0 || this.radHeatMap1.SelectedCellIndex.RowIndex < 0)
            {
                return;
            }

            SymbolPairPopup pp = new SymbolPairPopup();

            CorrelationCoefficient dataBndItm = this.radHeatMap1.SelectedDataItem.DataBoundItem as CorrelationCoefficient;
            pp.Show(this.CVF, (Symbols)Enum.Parse(typeof(Symbols), dataBndItm.A), (Symbols)Enum.Parse(typeof(Symbols), dataBndItm.B));
        }

        #endregion

        private void checkBox_drip_CheckedChanged(object sender, EventArgs e)
        {

        }

        #region Trending Correlation

        private void InitTrendingCorrelationTab()
        {
            this.TrendingCorrSymbolSelectorADataSource = new BindingList<Symbols>(GlobalVars.AllSymbols);
            this.TrendingCorrSymbolSelectorBDataSource = new BindingList<Symbols>(GlobalVars.AllSymbols);
            this.radDropDownList_ct_symbola.DataSource = this.TrendingCorrSymbolSelectorADataSource;
            this.radDropDownList_ct_symbolb.DataSource = this.TrendingCorrSymbolSelectorBDataSource;
        }


        private void button_update_trendingCorr_Click(object sender, EventArgs e)
        {
            var symA = (Symbols) this.radDropDownList_ct_symbola.SelectedItem.Value;
            var symB = (Symbols) this.radDropDownList_ct_symbolb.SelectedItem.Value;

            var ds = new CorrelationTrendHandler(symA.ToString(), symB.ToString());
            this.radGridView_ct.DataSource = null;
            this.radGridView_ct.DataSource = ds.DataSource;
            this.radGridView_ct.MasterTemplate.Refresh();

            this.UpdateChart(ds);

            // this.ExportTrendingCorrToCSV();
        }

        public void ExportTrendingCorrToCSV()
        {
            ExportToCSV exporter = new ExportToCSV(this.radGridView_ct);
            exporter.SummariesExportOption = SummariesOption.ExportAll; // Optional
            string fileName = "TrendingCorrelationExported.csv";
            exporter.RunExport(fileName);
        }

        public void UpdateChart(CorrelationTrendHandler _correlationTrendHandler)
        {
            this.chart_ct.Series.Clear();

            string seriesName = $"{_correlationTrendHandler.SymbolA}&{_correlationTrendHandler.SymbolB}";

            var series = this.chart_ct.Series.Add(seriesName);
            series.ChartType = SeriesChartType.Column;
            series.XValueType = ChartValueType.Date;
            series.XValueMember = nameof(CorrelationCoefficient.Timestamp);
            series.YValueType = ChartValueType.Double;
            series.YValueMembers = nameof(CorrelationCoefficient.Value);
            this.chart_ct.DataSource = null;
            this.chart_ct.DataSource = _correlationTrendHandler.DataSource;

            // series.DataPoints.Add(new CategoricalDataPoint(6, DateTime.Now));
            // foreach (var cc in _correlationTrendHandler.DataSource)
            // {
            //     series.Points.AddXY(cc.Timestamp, cc.Value);
            // }
        }

        #endregion

        #region Projection tab.

        public void InitProjectionTab()
        {
            #region Save-file dialog

            this.radSaveFileDialog_projection.SaveFileDialogForm.ExplorerControl.MainNavigationTreeView.ElementTree.EnableApplicationThemeName = false;
            this.radSaveFileDialog_projection.SaveFileDialogForm.ExplorerControl.FileBrowserListView.ElementTree.EnableApplicationThemeName = false;
            this.radSaveFileDialog_projection.SaveFileDialogForm.ElementTree.ThemeName = "Office2019Dark";
            this.radSaveFileDialog_projection.SaveFileDialogForm.ExplorerControl.MainNavigationTreeView.ElementTree.ThemeName = "Office2019Dark";
            this.radSaveFileDialog_projection.SaveFileDialogForm.ExplorerControl.FileBrowserListView.ElementTree.ThemeName = "Office2019Dark";

            #endregion

            #region DataGrid setup.

            GridViewDataColumn col0 = new GridViewDecimalColumn(typeof(int), "week", "week");
            this.radGridView_projection.Columns.Add(col0);

            GridViewDataColumn col1 = new GridViewDecimalColumn(typeof(double), "shareprice", "shareprice");
            this.radGridView_projection.Columns.Add(col1);

            GridViewDataColumn col2 = new GridViewDecimalColumn(typeof(double), "volume", "volume");
            this.radGridView_projection.Columns.Add(col2);

            GridViewDataColumn col3 = new GridViewDecimalColumn(typeof(double), "dividend", "dividend");
            this.radGridView_projection.Columns.Add(col3);

            GridViewDataColumn col4 = new GridViewDecimalColumn(typeof(double), "dripVolume", "dripVolume");
            this.radGridView_projection.Columns.Add(col4);

            GridViewDataColumn col5 = new GridViewDecimalColumn(typeof(double), "usd", "usd");
            this.radGridView_projection.Columns.Add(col5);

            GridViewDataColumn col6 = new GridViewDecimalColumn(typeof(double), "totalValue", "totalValue");
            this.radGridView_projection.Columns.Add(col6);

            #endregion

            #region Symbol selector

            this.radDropDownList_proj_symbol.DataSource = GlobalVars.AllSymbols;

            #endregion

        }

        #endregion

        private void radButton_proj_run_Click(object sender, EventArgs e)
        {
            // Clear the grid.
            this.radGridView_projection.Rows.Clear();
            this.Projection = ProjectionHandler.Create(Symbols.MARO, (int)this.numericUpDown_proj_duration.Value, (double)this.numericUpDown_proj_shareprice.Value, (double)numericUpDown_proj_volume.Value, (double)numericUpDown_proj_dividend.Value, (double)numericUpDown_proj_shareChange.Value, (double)numericUpDown_proj_shareChange.Value, (double) this.numericUpDown_proj_weeklyContribution.Value);
            var dataSource = this.Projection.RunProjection();

            for (int i = 0; i < this.Projection.ProjectionItems.Length; i++)
            {
                var pi = this.Projection.ProjectionItems[i];
                this.radGridView_projection.Rows.Add(new object[] { i, Math.Round(pi.SharePrice, 2), pi.Volume, Math.Round(pi.TotalDividend, 2), pi.DripAddVolume, Math.Round(pi.CashUSD, 2), Math.Round(pi.TotalValue, 2) });
            }
        }

        private void radButton_proj_append_Click(object sender, EventArgs e)
        {
            // Clear the grid.
            this.radGridView_projection.Rows.Clear();
            this.Projection.AppendProjection((int)this.numericUpDown_proj_duration.Value, (double)numericUpDown_proj_shareChange.Value, (double)numericUpDown_proj_shareChange.Value, (double)this.numericUpDown_proj_weeklyContribution.Value);

            for (int i = 0; i < this.Projection.ProjectionItems.Length; i++)
            {
                var pi = this.Projection.ProjectionItems[i];
                this.radGridView_projection.Rows.Add(new object[] { i, Math.Round(pi.SharePrice, 2), pi.Volume, Math.Round(pi.TotalDividend, 2), pi.DripAddVolume, Math.Round(pi.CashUSD, 2), Math.Round(pi.TotalValue, 2) });
            }
        }

        private void radButton_proj_export_Click(object sender, EventArgs e)
        {
            string fileName = $"projection_{numericUpDown_proj_duration.Value}Wks_{numericUpDown_proj_shareprice.Value}_{numericUpDown_proj_volume.Value}_{numericUpDown_proj_dividend.Value}_{numericUpDown_proj_shareChange.Value}.csv";
            this.radSaveFileDialog_projection.FileName = fileName;

            if (this.radSaveFileDialog_projection.ShowDialog() == DialogResult.OK)
            {
                ExportToCSV exporter = new ExportToCSV(this.radGridView_projection);
                exporter.SummariesExportOption = SummariesOption.ExportAll;

                // Choose a path for the file
                fileName = this.radSaveFileDialog_projection.FileName;
                exporter.RunExport(fileName);
            }


            
        }

        private void radGroupBox_highlightroi_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTip.AutoPopDelay = 15000;
            e.ToolTipText =
                "Selecting a timespan will highlight the divPerDollar values that are equal to or greater than the average required to achieve selected ROI.\n" +
                "This is intended to show current strength of a symbol.";

        }

        private void radLabel2_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            // holdings tab, DATE
            e.ToolTip.AutoPopDelay = 15000;
            e.ToolTipText = "When were the shares purchased ?";
        }

        private void radLabel3_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTip.AutoPopDelay = 15000;
            e.ToolTipText = "Price per share.";
        }

        private void radLabel4_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTip.AutoPopDelay = 15000;
            e.ToolTipText = "How many shares purchased ?";
        }

        private void radLabel5_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTip.AutoPopDelay = 15000;
            e.ToolTipText = "Automatically use weekly dividends to buy more shares.";
        }

        private void radGroupBox_ct_selectPair_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTip.AutoPopDelay = 15000;
            e.ToolTipText = "Shows the weekly correlation values for 2 symbols.";
        }

        private void radGroupBox1_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTip.AutoPopDelay = 15000;
            e.ToolTipText = "You can add your own actual holdings and also any 'what-if' \n" +
                            "scenarios you want to see played out up to the latest dividend date.\n";
        }

        private void radGroupBox_proj_weeklyContribution_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTip.AutoPopDelay = 15000;
            e.ToolTipText = "Invest more money every week.";
        }

        private void radButton_proj_append_ToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            e.ToolTip.AutoPopDelay = 15000;
            e.ToolTipText = "Continue with current table and add another number of weeks with a different share percent change.\n" +
                            " Will project using the last share price, volume and dividend from existing table.";
        }

        private void radDropDownList_proj_symbol_Click(object sender, EventArgs e)
        {

        }

        private void radDropDownList_proj_symbol_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            // sharePrice, volume, dividend will be set when selected symbol changes.
            string sym = radDropDownList_proj_symbol.Items[this.radDropDownList_proj_symbol.SelectedIndex].Value.ToString();
            DateTime startWeeklyDividends = new DateTime(2025, 10, 16);
            Dictionary<HistoricalDividendData, HistoricalTickData1D> dataPairs = new Dictionary<HistoricalDividendData, HistoricalTickData1D>();

            double sharePrice = -1;
            int volume = -1;
            double avgDividend = -1;
            double lowDividend = -1;

            using (var db = new dbDataContext())
            {
                List<HistoricalDividendData> dbDivData = db.HistoricalDividendDatas.Where(x => x.symbol.Equals(sym) && x.timestamp > startWeeklyDividends).ToList();
                foreach (HistoricalDividendData divData in dbDivData)
                {
                    if (db.HistoricalTickData1Ds.Any(x => x.symbol.Equals(sym) && x.timestamp.Date == divData.timestamp.Date))
                    {
                        HistoricalTickData1D tickData = db.HistoricalTickData1Ds.Single(x => x.symbol.Equals(sym) && x.timestamp.Date == divData.timestamp.Date);
                        dataPairs.Add(divData, tickData);
                    }
                }

                HistoricalTickData1D[] valueArray = dataPairs.Values.ToArray();

                int lastIdx = valueArray.Length - 1;
                sharePrice = valueArray[lastIdx].close ?? 0;
            }

            Dictionary<DateTime, double> DividendPerDollarDict = new Dictionary<DateTime, double>();

            foreach (var kvp0 in dataPairs.OrderBy(x => x.Key.timestamp))
            {
                var d = kvp0.Key;
                var t = kvp0.Value;

                double divValue = d.dividend ?? 0;
                if (divValue == 0)
                {
                    MessageBox.Show($"ERROR: HistoricalDividendData.dividend value is zero.", "FUCK!", MessageBoxButtons.OK);
                    return;
                }

                double closeValue = t.close ?? 0;
                if (divValue == 0)
                {
                    MessageBox.Show($"ERROR: HistoricalTickData1D.close value is zero.", "FUCK!", MessageBoxButtons.OK);
                    return;
                }

                double dpd = divValue / closeValue;
                DividendPerDollarDict.Add(d.timestamp.Date, dpd);
            }

            // Add controls for user to select dividend mode. lowestEver or average.
            this.LowestWeeklyDivPerDollar = DividendPerDollarDict.Values.Min();
            double overallAverageDpd = DividendPerDollarDict.Values.Average();
            int skipIndexRecentAvgDpd = DividendPerDollarDict.Values.Count - 4;
            double recentAverageDpd = DividendPerDollarDict.Values.Skip(skipIndexRecentAvgDpd).Average();
            this.MinAverageDivPerDollar = Math.Min(overallAverageDpd, recentAverageDpd);
            this.HandleSelectedDividendMode((decimal)sharePrice);

            this.bAutoSettingActive = true;
            this.numericUpDown_proj_dividend.Value = (decimal)(this.SelectedDivPerDollarValue * sharePrice);
            this.numericUpDown_proj_shareprice.Value = (decimal) sharePrice;
            this.numericUpDown_proj_volume.Value = (int)Math.Floor(this.numericUpDown_proj_initialInvestment.Value / (decimal)sharePrice);
            this.bAutoSettingActive = false;
        }

        private void HandleSelectedDividendMode(decimal _sharePrice)
        {
            switch (this.SelectedDividendMode)
            {
                case "low":
                {
                    this.SelectedDivPerDollarValue = this.LowestWeeklyDivPerDollar;
                    break;
                }
                case "avg":
                {
                    this.SelectedDivPerDollarValue = this.MinAverageDivPerDollar;
                    break;
                }
            }

            this.numericUpDown_proj_dividend.Value = (decimal)this.SelectedDivPerDollarValue * _sharePrice;
        }

        private void numericUpDown_proj_initialInvestment_ValueChanged(object sender, EventArgs e)
        {
            if (this.bAutoSettingActive) return;
            this.bAutoSettingActive = true;
            int initialInvestmentValue = (int) Math.Floor(numericUpDown_proj_initialInvestment.Value);
            this.numericUpDown_proj_volume.Value = (int)Math.Floor(initialInvestmentValue / this.numericUpDown_proj_shareprice.Value);
            this.bAutoSettingActive = false;
        }

        private void numericUpDown_proj_shareChange_ValueChanged(object sender, EventArgs e)
        {
            if (this.bAutoSettingActive) return;
            this.bAutoSettingActive = true;
            decimal sharePriceValue = numericUpDown_proj_shareprice.Value;
            this.numericUpDown_proj_volume.Value = Math.Floor(this.numericUpDown_proj_initialInvestment.Value / sharePriceValue);
            this.bAutoSettingActive = false;
        }

        #region Grid debugging.

        public ProjectionDebugForm ProjectionDebug;
        public bool ProjectionDebugVisible = false;
        private void radButton_proj_debug_Click(object sender, EventArgs e)
        {
            ProjectionDebugVisible = !ProjectionDebugVisible;
            if (ProjectionDebug == null)
            {
                ProjectionDebug = new ProjectionDebugForm();
                ProjectionDebug.propertyGrid_projDebug.SelectedObject = this.radGridView_holdings;
            }
            ProjectionDebug.Visible = ProjectionDebugVisible;
        }

        #endregion

        private void radRadioButton_proj_dividend_low_avg_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            RadRadioButton rrButton = (RadRadioButton)sender;
            if (rrButton == null) return;
            this.SelectedDividendMode = rrButton.Tag.ToString();
            this.HandleSelectedDividendMode((decimal)this.numericUpDown_proj_shareprice.Value);
        }
    }
}