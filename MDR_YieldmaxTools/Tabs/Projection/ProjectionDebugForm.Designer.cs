
namespace MDR_YieldmaxTools.Tabs.Projection
{
    partial class ProjectionDebugForm
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
            this.propertyGrid_projDebug = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // propertyGrid_projDebug
            // 
            this.propertyGrid_projDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid_projDebug.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid_projDebug.Name = "propertyGrid_projDebug";
            this.propertyGrid_projDebug.Size = new System.Drawing.Size(562, 809);
            this.propertyGrid_projDebug.TabIndex = 1;
            // 
            // ProjectionDebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 809);
            this.Controls.Add(this.propertyGrid_projDebug);
            this.Name = "ProjectionDebugForm";
            this.Text = "ProjectionDebugForm";
            this.Load += new System.EventHandler(this.ProjectionDebugForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PropertyGrid propertyGrid_projDebug;
    }
}