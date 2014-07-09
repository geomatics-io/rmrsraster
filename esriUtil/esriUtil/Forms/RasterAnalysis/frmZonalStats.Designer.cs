namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmZonalStats
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmZonalStats));
            this.cmbZonalStat = new System.Windows.Forms.ComboBox();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbZoneRaster = new System.Windows.Forms.ComboBox();
            this.btnOpenValueRaster = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbValueRaster = new System.Windows.Forms.ComboBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.lsbStats = new System.Windows.Forms.ListBox();
            this.btnAll = new System.Windows.Forms.Button();
            this.cmbZoneField = new System.Windows.Forms.ComboBox();
            this.lblZone = new System.Windows.Forms.Label();
            this.txtTableName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chbClassCounts = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbZonalStat
            // 
            this.cmbZonalStat.FormattingEnabled = true;
            this.cmbZonalStat.Location = new System.Drawing.Point(12, 112);
            this.cmbZonalStat.Name = "cmbZonalStat";
            this.cmbZonalStat.Size = new System.Drawing.Size(213, 21);
            this.cmbZonalStat.TabIndex = 34;
            this.cmbZonalStat.SelectedIndexChanged += new System.EventHandler(this.cmbZonalStat_SelectedIndexChanged);
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(238, 31);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(25, 27);
            this.btnOpenRaster.TabIndex = 38;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Zonal Statistic";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Input Zonal Raster or Feature Class";
            // 
            // cmbZoneRaster
            // 
            this.cmbZoneRaster.FormattingEnabled = true;
            this.cmbZoneRaster.Location = new System.Drawing.Point(12, 34);
            this.cmbZoneRaster.Name = "cmbZoneRaster";
            this.cmbZoneRaster.Size = new System.Drawing.Size(220, 21);
            this.cmbZoneRaster.TabIndex = 36;
            this.cmbZoneRaster.SelectedIndexChanged += new System.EventHandler(this.cmbZoneRaster_SelectedIndexChanged);
            // 
            // btnOpenValueRaster
            // 
            this.btnOpenValueRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenValueRaster.Location = new System.Drawing.Point(236, 277);
            this.btnOpenValueRaster.Name = "btnOpenValueRaster";
            this.btnOpenValueRaster.Size = new System.Drawing.Size(25, 27);
            this.btnOpenValueRaster.TabIndex = 41;
            this.btnOpenValueRaster.UseVisualStyleBackColor = true;
            this.btnOpenValueRaster.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 262);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 40;
            this.label2.Text = "Input Value Raster";
            // 
            // cmbValueRaster
            // 
            this.cmbValueRaster.FormattingEnabled = true;
            this.cmbValueRaster.Location = new System.Drawing.Point(10, 280);
            this.cmbValueRaster.Name = "cmbValueRaster";
            this.cmbValueRaster.Size = new System.Drawing.Size(220, 21);
            this.cmbValueRaster.TabIndex = 39;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(186, 360);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 42;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(230, 167);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(27, 23);
            this.btnClear.TabIndex = 45;
            this.btnClear.Text = "!";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(230, 141);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(27, 23);
            this.btnMinus.TabIndex = 44;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // lsbStats
            // 
            this.lsbStats.FormattingEnabled = true;
            this.lsbStats.Location = new System.Drawing.Point(14, 140);
            this.lsbStats.Name = "lsbStats";
            this.lsbStats.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbStats.Size = new System.Drawing.Size(211, 108);
            this.lsbStats.TabIndex = 43;
            // 
            // btnAll
            // 
            this.btnAll.Location = new System.Drawing.Point(230, 196);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(27, 23);
            this.btnAll.TabIndex = 46;
            this.btnAll.Text = "%";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // cmbZoneField
            // 
            this.cmbZoneField.FormattingEnabled = true;
            this.cmbZoneField.Location = new System.Drawing.Point(71, 64);
            this.cmbZoneField.Name = "cmbZoneField";
            this.cmbZoneField.Size = new System.Drawing.Size(161, 21);
            this.cmbZoneField.TabIndex = 47;
            this.cmbZoneField.Visible = false;
            // 
            // lblZone
            // 
            this.lblZone.AutoSize = true;
            this.lblZone.Location = new System.Drawing.Point(11, 67);
            this.lblZone.Name = "lblZone";
            this.lblZone.Size = new System.Drawing.Size(54, 13);
            this.lblZone.TabIndex = 48;
            this.lblZone.Text = "Zone field";
            this.lblZone.Visible = false;
            // 
            // txtTableName
            // 
            this.txtTableName.Enabled = false;
            this.txtTableName.Location = new System.Drawing.Point(10, 330);
            this.txtTableName.Name = "txtTableName";
            this.txtTableName.Size = new System.Drawing.Size(222, 20);
            this.txtTableName.TabIndex = 49;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 313);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 50;
            this.label3.Text = "Output Table Path";
            // 
            // chbClassCounts
            // 
            this.chbClassCounts.AutoSize = true;
            this.chbClassCounts.Location = new System.Drawing.Point(10, 363);
            this.chbClassCounts.Name = "chbClassCounts";
            this.chbClassCounts.Size = new System.Drawing.Size(87, 17);
            this.chbClassCounts.TabIndex = 51;
            this.chbClassCounts.Text = "Class Counts";
            this.chbClassCounts.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button1.Location = new System.Drawing.Point(238, 327);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 27);
            this.button1.TabIndex = 52;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // frmZonalStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 395);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chbClassCounts);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTableName);
            this.Controls.Add(this.lblZone);
            this.Controls.Add(this.cmbZoneField);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.lsbStats);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnOpenValueRaster);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbValueRaster);
            this.Controls.Add(this.cmbZonalStat);
            this.Controls.Add(this.btnOpenRaster);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbZoneRaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmZonalStats";
            this.Text = "Zonal Stats";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbZonalStat;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbZoneRaster;
        private System.Windows.Forms.Button btnOpenValueRaster;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbValueRaster;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.ListBox lsbStats;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.ComboBox cmbZoneField;
        private System.Windows.Forms.Label lblZone;
        private System.Windows.Forms.TextBox txtTableName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chbClassCounts;
        private System.Windows.Forms.Button button1;
    }
}