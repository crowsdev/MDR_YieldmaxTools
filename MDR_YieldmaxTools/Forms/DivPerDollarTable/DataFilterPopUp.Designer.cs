
namespace MDR_YieldmaxTools.Forms.DivPerDollarTable
{
    partial class DataFilterPopUp
    {
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
            this.visualStudio2022DarkTheme1 = new Telerik.WinControls.Themes.VisualStudio2022DarkTheme();
            this.radFilterView1 = new Telerik.WinControls.UI.RadFilterView();
            ((System.ComponentModel.ISupportInitialize)(this.radFilterView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radFilterView1
            // 
            this.radFilterView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radFilterView1.Location = new System.Drawing.Point(0, 0);
            this.radFilterView1.Name = "radFilterView1";
            this.radFilterView1.Size = new System.Drawing.Size(504, 550);
            this.radFilterView1.TabIndex = 0;
            this.radFilterView1.ThemeName = "VisualStudio2022Dark";
            // 
            // DataFilterPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 550);
            this.Controls.Add(this.radFilterView1);
            this.Name = "DataFilterPopUp";
            this.Text = "DataFilterPopUp";
            this.ThemeName = "VisualStudio2022Dark";
            this.Load += new System.EventHandler(this.DataFilterPopUp_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radFilterView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public Telerik.WinControls.Themes.VisualStudio2022DarkTheme visualStudio2022DarkTheme1;
        public Telerik.WinControls.UI.RadFilterView radFilterView1;
    }
}