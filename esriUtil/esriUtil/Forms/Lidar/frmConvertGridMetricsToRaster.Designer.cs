namespace esriUtil.Forms.Lidar
{
    partial class frmConvertGridMetricsToRaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConvertGridMetricsToRaster));
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMetricsDir = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOutDir = new System.Windows.Forms.Button();
            this.btnOpenMetricsDir = new System.Windows.Forms.Button();
            this.gpSelection = new System.Windows.Forms.GroupBox();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbMetrics = new System.Windows.Forms.ComboBox();
            this.lstMetrics = new System.Windows.Forms.ListBox();
            this.gpSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtOutDir
            // 
            this.txtOutDir.Location = new System.Drawing.Point(12, 79);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(223, 20);
            this.txtOutDir.TabIndex = 26;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Output Directory";
            // 
            // txtMetricsDir
            // 
            this.txtMetricsDir.Location = new System.Drawing.Point(12, 29);
            this.txtMetricsDir.Name = "txtMetricsDir";
            this.txtMetricsDir.Size = new System.Drawing.Size(224, 20);
            this.txtMetricsDir.TabIndex = 23;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(195, 355);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(74, 27);
            this.btnExecute.TabIndex = 22;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Grid Metrics Directory";
            // 
            // btnOutDir
            // 
            this.btnOutDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutDir.Location = new System.Drawing.Point(242, 75);
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.Size = new System.Drawing.Size(27, 28);
            this.btnOutDir.TabIndex = 25;
            this.btnOutDir.UseVisualStyleBackColor = true;
            this.btnOutDir.Click += new System.EventHandler(this.btnOutDir_Click);
            // 
            // btnOpenMetricsDir
            // 
            this.btnOpenMetricsDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenMetricsDir.Location = new System.Drawing.Point(242, 24);
            this.btnOpenMetricsDir.Name = "btnOpenMetricsDir";
            this.btnOpenMetricsDir.Size = new System.Drawing.Size(27, 28);
            this.btnOpenMetricsDir.TabIndex = 21;
            this.btnOpenMetricsDir.UseVisualStyleBackColor = true;
            this.btnOpenMetricsDir.Click += new System.EventHandler(this.btnOpenMetricsDir_Click);
            // 
            // gpSelection
            // 
            this.gpSelection.Controls.Add(this.btnRemoveAll);
            this.gpSelection.Controls.Add(this.btnMinus);
            this.gpSelection.Controls.Add(this.btnAddAll);
            this.gpSelection.Controls.Add(this.label3);
            this.gpSelection.Controls.Add(this.cmbMetrics);
            this.gpSelection.Controls.Add(this.lstMetrics);
            this.gpSelection.Location = new System.Drawing.Point(12, 109);
            this.gpSelection.Name = "gpSelection";
            this.gpSelection.Size = new System.Drawing.Size(257, 240);
            this.gpSelection.TabIndex = 39;
            this.gpSelection.TabStop = false;
            this.gpSelection.Text = "Metrics";
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(227, 123);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveAll.TabIndex = 30;
            this.btnRemoveAll.Text = "!";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(227, 65);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(25, 23);
            this.btnMinus.TabIndex = 29;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(227, 94);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(25, 23);
            this.btnAddAll.TabIndex = 28;
            this.btnAddAll.Text = "%";
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Convert to Raster";
            // 
            // cmbMetrics
            // 
            this.cmbMetrics.FormattingEnabled = true;
            this.cmbMetrics.Location = new System.Drawing.Point(3, 39);
            this.cmbMetrics.Name = "cmbMetrics";
            this.cmbMetrics.Size = new System.Drawing.Size(220, 21);
            this.cmbMetrics.TabIndex = 18;
            this.cmbMetrics.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // lstMetrics
            // 
            this.lstMetrics.FormattingEnabled = true;
            this.lstMetrics.Location = new System.Drawing.Point(3, 62);
            this.lstMetrics.Name = "lstMetrics";
            this.lstMetrics.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstMetrics.Size = new System.Drawing.Size(220, 160);
            this.lstMetrics.TabIndex = 17;
            // 
            // frmConvertGridMetricsToRaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 394);
            this.Controls.Add(this.gpSelection);
            this.Controls.Add(this.txtOutDir);
            this.Controls.Add(this.btnOutDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMetricsDir);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnOpenMetricsDir);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmConvertGridMetricsToRaster";
            this.Text = "Convert Grid Metrics To Raster";
            this.TopMost = true;
            this.gpSelection.ResumeLayout(false);
            this.gpSelection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Button btnOutDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMetricsDir;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnOpenMetricsDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gpSelection;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbMetrics;
        private System.Windows.Forms.ListBox lstMetrics;
    }
}