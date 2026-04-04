
using System;
using System.Linq;
using MDR_YieldmaxTools.Enums;
using Telerik.WinControls.UI;

namespace MDR_YieldmaxTools.Tabs.Correlation
{
    public class SymbolPairPopup : RadForm
    {
        private Symbols a { get; set; }
        private Symbols b { get; set; }
        private CorrelationVectorsFactory cvf { get; set; }

        public SymbolPairPopup()
        {
            InitializeComponent();
        }

        public void Show(CorrelationVectorsFactory _cvf, Symbols _symA, Symbols _symB)
        {
            cvf = _cvf;
            a = _symA;
            b = _symB;

            this.radLabel_a.Text = $"{a}";
            this.radLabel_b.Text = $"{b}";

            BuildSparkLines();

            base.Show();
        }

        public void BuildSparkLines()
        {
            var lineSeriesA = new SparkLineSeries();
            var lineSeriesB = new SparkLineSeries();

            var dataA = this.cvf.SymbolDataMap[a].Select<HistoricalTickData1D, double>(x => x.adjustedClose ?? 0).ToList<double>();
            var dataB = this.cvf.SymbolDataMap[b].Select<HistoricalTickData1D, double>(x => x.adjustedClose ?? 0).ToList<double>();

            lineSeriesA.DataPoints.AddRange(dataA.Select(x => new CategoricalSparkDataPoint(x)));
            // lineSeriesA.ShowHighPointIndicator = true;
            // lineSeriesA.ShowLowPointIndicator = true;
            // lineSeriesA.HighPointShape = new StarShape(5, 3);
            this.radSparkline_a.Series = lineSeriesA;

            lineSeriesB.DataPoints.AddRange(dataB.Select(x => new CategoricalSparkDataPoint(x)));
            // lineSeriesB.ShowHighPointIndicator = true;
            // lineSeriesB.ShowLowPointIndicator = true;
            // lineSeriesB.HighPointShape = new StarShape(5, 3);
            this.radSparkline_b.Series = lineSeriesB;
        }

        private void SymbolPairPopup_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.office2019DarkTheme1 = new Telerik.WinControls.Themes.Office2019DarkTheme();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.radSparkline_b = new Telerik.WinControls.UI.RadSparkline();
            this.radLabel_b = new Telerik.WinControls.UI.RadLabel();
            this.radLabel_symbolpair = new Telerik.WinControls.UI.RadLabel();
            this.radLabel_a = new Telerik.WinControls.UI.RadLabel();
            this.radSparkline_a = new Telerik.WinControls.UI.RadSparkline();
            this.aquaTheme1 = new Telerik.WinControls.Themes.AquaTheme();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radSparkline_b)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel_b)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel_symbolpair)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel_a)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSparkline_a)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.radSparkline_b, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.radLabel_b, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.radLabel_symbolpair, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radLabel_a, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.radSparkline_a, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(441, 336);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // radSparkline_b
            // 
            this.radSparkline_b.AxisDrawMode = Telerik.WinControls.UI.AxisDrawMode.BelowSeries;
            this.radSparkline_b.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel1.SetColumnSpan(this.radSparkline_b, 2);
            this.radSparkline_b.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radSparkline_b.ForeColor = System.Drawing.Color.Red;
            this.radSparkline_b.Location = new System.Drawing.Point(156, 193);
            this.radSparkline_b.Margin = new System.Windows.Forms.Padding(9);
            this.radSparkline_b.Name = "radSparkline_b";
            this.radSparkline_b.ShowFirstPointIndicator = true;
            this.radSparkline_b.ShowLastPointIndicator = true;
            this.radSparkline_b.Size = new System.Drawing.Size(276, 134);
            this.radSparkline_b.TabIndex = 4;
            this.radSparkline_b.Text = "radSparkline2";
            this.radSparkline_b.ThemeName = "Fluent";
            // 
            // radLabel_b
            // 
            this.radLabel_b.AutoSize = false;
            this.radLabel_b.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radLabel_b.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel_b.Location = new System.Drawing.Point(3, 187);
            this.radLabel_b.Name = "radLabel_b";
            this.radLabel_b.Size = new System.Drawing.Size(141, 146);
            this.radLabel_b.TabIndex = 3;
            this.radLabel_b.Text = "BBBB";
            this.radLabel_b.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radLabel_b.ThemeName = "Office2019Dark";
            // 
            // radLabel_symbolpair
            // 
            this.radLabel_symbolpair.AutoSize = false;
            this.tableLayoutPanel1.SetColumnSpan(this.radLabel_symbolpair, 3);
            this.radLabel_symbolpair.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radLabel_symbolpair.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel_symbolpair.Location = new System.Drawing.Point(3, 3);
            this.radLabel_symbolpair.Name = "radLabel_symbolpair";
            this.radLabel_symbolpair.Size = new System.Drawing.Size(435, 27);
            this.radLabel_symbolpair.TabIndex = 0;
            this.radLabel_symbolpair.Text = "Symbol Pair Info";
            this.radLabel_symbolpair.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radLabel_symbolpair.ThemeName = "Office2019Dark";
            // 
            // radLabel_a
            // 
            this.radLabel_a.AutoSize = false;
            this.radLabel_a.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radLabel_a.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel_a.Location = new System.Drawing.Point(3, 36);
            this.radLabel_a.Name = "radLabel_a";
            this.radLabel_a.Size = new System.Drawing.Size(141, 145);
            this.radLabel_a.TabIndex = 1;
            this.radLabel_a.Text = "AAAA";
            this.radLabel_a.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radLabel_a.ThemeName = "Office2019Dark";
            // 
            // radSparkline_a
            // 
            this.radSparkline_a.AxisDrawMode = Telerik.WinControls.UI.AxisDrawMode.BelowSeries;
            this.radSparkline_a.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel1.SetColumnSpan(this.radSparkline_a, 2);
            this.radSparkline_a.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radSparkline_a.ForeColor = System.Drawing.Color.Lime;
            this.radSparkline_a.Location = new System.Drawing.Point(156, 42);
            this.radSparkline_a.Margin = new System.Windows.Forms.Padding(9);
            this.radSparkline_a.Name = "radSparkline_a";
            this.radSparkline_a.ShowFirstPointIndicator = true;
            this.radSparkline_a.ShowLastPointIndicator = true;
            this.radSparkline_a.Size = new System.Drawing.Size(276, 133);
            this.radSparkline_a.TabIndex = 2;
            this.radSparkline_a.ThemeName = "Aqua";
            // 
            // SymbolPairPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 336);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SymbolPairPopup";
            this.Text = "SymbolPairPopup";
            this.ThemeName = "Office2019Dark";
            this.Load += new System.EventHandler(this.SymbolPairPopup_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radSparkline_b)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel_b)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel_symbolpair)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel_a)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSparkline_a)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public Telerik.WinControls.Themes.Office2019DarkTheme office2019DarkTheme1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public Telerik.WinControls.UI.RadLabel radLabel_symbolpair;
        public Telerik.WinControls.UI.RadLabel radLabel_a;
        public Telerik.WinControls.UI.RadLabel radLabel_b;
        public Telerik.WinControls.UI.RadSparkline radSparkline_b;
        public Telerik.WinControls.UI.RadSparkline radSparkline_a;
        public Telerik.WinControls.Themes.AquaTheme aquaTheme1;
    }
}