namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmSetStatistics
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetStatistics));
            this.dgvStats = new System.Windows.Forms.DataGridView();
            this.clmBand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMean = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmStd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStats)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvStats
            // 
            this.dgvStats.AllowUserToAddRows = false;
            this.dgvStats.AllowUserToDeleteRows = false;
            this.dgvStats.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvStats.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvStats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStats.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmBand,
            this.clmMin,
            this.clmMax,
            this.clmMean,
            this.clmStd});
            this.dgvStats.Location = new System.Drawing.Point(12, 12);
            this.dgvStats.Name = "dgvStats";
            this.dgvStats.Size = new System.Drawing.Size(329, 150);
            this.dgvStats.TabIndex = 0;
            // 
            // clmBand
            // 
            this.clmBand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmBand.HeaderText = "Band";
            this.clmBand.Name = "clmBand";
            this.clmBand.ReadOnly = true;
            this.clmBand.Width = 57;
            // 
            // clmMin
            // 
            this.clmMin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmMin.HeaderText = "Min";
            this.clmMin.Name = "clmMin";
            this.clmMin.Width = 49;
            // 
            // clmMax
            // 
            this.clmMax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmMax.HeaderText = "Max";
            this.clmMax.Name = "clmMax";
            this.clmMax.Width = 52;
            // 
            // clmMean
            // 
            this.clmMean.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmMean.HeaderText = "Mean";
            this.clmMean.Name = "clmMean";
            this.clmMean.Width = 59;
            // 
            // clmStd
            // 
            this.clmStd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmStd.HeaderText = "StdDev";
            this.clmStd.Name = "clmStd";
            this.clmStd.Width = 68;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(266, 169);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmSetStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 200);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dgvStats);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSetStatistics";
            this.Text = "Set Statistics";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dgvStats)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvStats;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmBand;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMean;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmStd;
        private System.Windows.Forms.Button btnClose;

    }
}