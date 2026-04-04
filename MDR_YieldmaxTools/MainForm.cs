using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        #endregion

        #endregion

        public MainForm()
        {
            InitializeComponent();
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

        private void InitTab2()
        {
            this.radDropDownList_symbol.DataSource = GlobalVars.AllSymbols;
            GridHoldingsHeirarchical = new HoldingsHeirarchicalGrid();
            InitHoldingsGrid();
        }

        private void InitHoldingsGrid()
        {
            this.radGridView_holdings.MasterTemplate.Reset();
            this.radGridView_holdings.DataSource = GridHoldingsHeirarchical.DataSourceParent;
            this.radGridView_holdings.AutoGenerateHierarchy = true;
            GridViewTemplate firstChildTemplate = new GridViewTemplate();
            firstChildTemplate.DataSource = GridHoldingsHeirarchical.DataSourceChild;
            firstChildTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            this.radGridView_holdings.MasterTemplate.Templates.Add(firstChildTemplate);
            firstChildTemplate.DataSource = GridHoldingsHeirarchical.DataSourceChild;

            GridViewRelation relation = new GridViewRelation(radGridView_holdings.MasterTemplate);
            relation.ChildTemplate = firstChildTemplate;

            relation.RelationName = "parentchild";
            relation.ParentColumnNames.Add(nameof(HoldingsItem0.ID));
            relation.ChildColumnNames.Add(nameof(DividendItem.ParentID));
            radGridView_holdings.Relations.Add(relation);
        }

        #endregion


        private void MainForm_Load(object sender, EventArgs e)
        {
            ThemeResolutionService.ApplicationThemeName = "Office2019Dark";
            this.Text = $"MDR Yieldmax Tools v{GlobalVars.Version}";

            InitCustomDataSource();

            InitTab2();

            // DoTest();
            CVF = new CorrelationVectorsFactory(GlobalVars.AllSymbols);
            DoRadHeatmap();

            this.InitTrendingCorrelationTab();

            this.InitProjectionTab();
        }

        private void radButton_add_Click(object sender, EventArgs e)
        {
            Symbols s = (Symbols)Enum.Parse(typeof(Symbols), radDropDownList_symbol.Items[this.radDropDownList_symbol.SelectedIndex].Value.ToString());
            DateTime d = this.radDateTimePicker_date.Value;
            double p = (double)this.numericUpDown_price.Value;
            double v = (double)this.numericUpDown_volume.Value;
            bool drp = checkBox_drip.Checked;

            GridHoldingsHeirarchical.AddHoldingsItem(s, d, p, v, drp);
            // radGridView_holdings.DataSource = null;
            // radGridView_holdings.Templates[0].DataSource = null;
            GridHoldingsHeirarchical.InitDataSources();
            InitHoldingsGrid();
            // radGridView_holdings.DataSource = GridHoldingsHeirarchical.DataSourceParent;
            // radGridView_holdings.Templates[0].DataSource = GridHoldingsHeirarchical.DataSourceChild;
            radGridView_holdings.MasterTemplate.Refresh();

            this.numericUpDown_price.Value = 0;
            this.numericUpDown_volume.Value = 0;
            this.radDropDownList_symbol.SelectedIndex = 0;
        }

        private void radButton_del_Click(object sender, EventArgs e)
        {
            // Delete selected holdings.
            GridHoldingsHeirarchical.DeleteSelectedHoldings(this.radGridView_holdings.SelectedRows.ToList());
            // radGridView_holdings.DataSource = null;
            // foreach (GridViewTemplate template in this.radGridView_holdings.Templates)
            // {
            //     template.DataSource = null;
            // }
            GridHoldingsHeirarchical.InitDataSources();
            InitHoldingsGrid();
            // radGridView_holdings.DataSource = GridHoldingsHeirarchical.DataSourceParent;
            // radGridView_holdings.Templates[0].DataSource = GridHoldingsHeirarchical.DataSourceChild;
            radGridView_holdings.Refresh();
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
            this.radSaveFileDialog_projection.SaveFileDialogForm.ExplorerControl.MainNavigationTreeView.ElementTree.EnableApplicationThemeName = false;
            this.radSaveFileDialog_projection.SaveFileDialogForm.ExplorerControl.FileBrowserListView.ElementTree.EnableApplicationThemeName = false;
            this.radSaveFileDialog_projection.SaveFileDialogForm.ElementTree.ThemeName = "Office2019Dark";
            this.radSaveFileDialog_projection.SaveFileDialogForm.ExplorerControl.MainNavigationTreeView.ElementTree.ThemeName = "Office2019Dark";
            this.radSaveFileDialog_projection.SaveFileDialogForm.ExplorerControl.FileBrowserListView.ElementTree.ThemeName = "Office2019Dark";


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

        }

        #endregion

        private void radButton_proj_run_Click(object sender, EventArgs e)
        {
            // Clear the grid.
            this.radGridView_projection.Rows.Clear();
            this.Projection = ProjectionHandler.Create(Symbols.MARO, (int)this.numericUpDown_proj_duration.Value, (double)this.numericUpDown_proj_shareprice.Value, (double)numericUpDown_proj_volume.Value, (double)numericUpDown_proj_dividend.Value, (double)numericUpDown_proj_shareChange.Value, this.CalculateDividendPercentChange((double)numericUpDown_proj_shareChange.Value));
            var dataSource = this.Projection.RunProjection();

            for (int i = 0; i < this.Projection.ProjectionItems.Length; i++)
            {
                var pi = this.Projection.ProjectionItems[i];
                this.radGridView_projection.Rows.Add(new object[] { i, Math.Round(pi.SharePrice, 2), pi.Volume, Math.Round(pi.TotalDividend, 2), pi.DripAddVolume, Math.Round(pi.CashUSD, 2), Math.Round(pi.TotalValue, 2) });
            }
        }

        private double CalculateDividendPercentChange(double _sharePricePercentChange)
        {
            return Math.Round(_sharePricePercentChange * 0.75, 2);
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
    }
}