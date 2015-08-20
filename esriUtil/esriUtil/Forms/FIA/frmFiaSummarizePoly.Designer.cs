namespace esriUtil.Forms.FIA
{
    partial class frmFiaSummarizePoly
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFiaSummarizePoly));
            this.btnOpenStands = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAll = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.lsbFields = new System.Windows.Forms.ListBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnOpenValueRaster = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbStrata = new System.Windows.Forms.ComboBox();
            this.cmbFields = new System.Windows.Forms.ComboBox();
            this.btnOpenPlots = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPlots = new System.Windows.Forms.ComboBox();
            this.cmbStands = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOpenStands
            // 
            this.btnOpenStands.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenStands.Location = new System.Drawing.Point(244, 304);
            this.btnOpenStands.Name = "btnOpenStands";
            this.btnOpenStands.Size = new System.Drawing.Size(25, 27);
            this.btnOpenStands.TabIndex = 71;
            this.btnOpenStands.UseVisualStyleBackColor = true;
            this.btnOpenStands.Click += new System.EventHandler(this.btnOpenStands_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 290);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 69;
            this.label3.Text = "Stands (Polygon)";
            // 
            // btnAll
            // 
            this.btnAll.Location = new System.Drawing.Point(236, 172);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(27, 23);
            this.btnAll.TabIndex = 65;
            this.btnAll.Text = "%";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(236, 143);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(27, 23);
            this.btnClear.TabIndex = 64;
            this.btnClear.Text = "!";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(236, 117);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(27, 23);
            this.btnMinus.TabIndex = 63;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // lsbFields
            // 
            this.lsbFields.FormattingEnabled = true;
            this.lsbFields.Location = new System.Drawing.Point(20, 116);
            this.lsbFields.Name = "lsbFields";
            this.lsbFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbFields.Size = new System.Drawing.Size(211, 108);
            this.lsbFields.TabIndex = 62;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(192, 337);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 61;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnOpenValueRaster
            // 
            this.btnOpenValueRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenValueRaster.Location = new System.Drawing.Point(242, 255);
            this.btnOpenValueRaster.Name = "btnOpenValueRaster";
            this.btnOpenValueRaster.Size = new System.Drawing.Size(25, 27);
            this.btnOpenValueRaster.TabIndex = 60;
            this.btnOpenValueRaster.UseVisualStyleBackColor = true;
            this.btnOpenValueRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 240);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(198, 13);
            this.label2.TabIndex = 59;
            this.label2.Text = "Stratification Dataset (Polygon or Raster)";
            // 
            // cmbStrata
            // 
            this.cmbStrata.FormattingEnabled = true;
            this.cmbStrata.Location = new System.Drawing.Point(16, 258);
            this.cmbStrata.Name = "cmbStrata";
            this.cmbStrata.Size = new System.Drawing.Size(220, 21);
            this.cmbStrata.TabIndex = 58;
            // 
            // cmbFields
            // 
            this.cmbFields.FormattingEnabled = true;
            this.cmbFields.Location = new System.Drawing.Point(18, 88);
            this.cmbFields.Name = "cmbFields";
            this.cmbFields.Size = new System.Drawing.Size(213, 21);
            this.cmbFields.TabIndex = 53;
            this.cmbFields.SelectedIndexChanged += new System.EventHandler(this.cmbZonalStat_SelectedIndexChanged);
            // 
            // btnOpenPlots
            // 
            this.btnOpenPlots.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenPlots.Location = new System.Drawing.Point(244, 34);
            this.btnOpenPlots.Name = "btnOpenPlots";
            this.btnOpenPlots.Size = new System.Drawing.Size(25, 27);
            this.btnOpenPlots.TabIndex = 57;
            this.btnOpenPlots.UseVisualStyleBackColor = true;
            this.btnOpenPlots.Click += new System.EventHandler(this.btnOpenPlots_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 54;
            this.label4.Text = "Fields To Summarize";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 56;
            this.label1.Text = "Plots (Points)";
            // 
            // cmbPlots
            // 
            this.cmbPlots.FormattingEnabled = true;
            this.cmbPlots.Location = new System.Drawing.Point(18, 37);
            this.cmbPlots.Name = "cmbPlots";
            this.cmbPlots.Size = new System.Drawing.Size(220, 21);
            this.cmbPlots.TabIndex = 55;
            this.cmbPlots.SelectedIndexChanged += new System.EventHandler(this.cmbZoneRaster_SelectedIndexChanged);
            // 
            // cmbStands
            // 
            this.cmbStands.FormattingEnabled = true;
            this.cmbStands.Location = new System.Drawing.Point(16, 310);
            this.cmbStands.Name = "cmbStands";
            this.cmbStands.Size = new System.Drawing.Size(220, 21);
            this.cmbStands.TabIndex = 72;
            // 
            // frmFiaSummarizePoly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 368);
            this.Controls.Add(this.cmbStands);
            this.Controls.Add(this.btnOpenStands);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.lsbFields);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnOpenValueRaster);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbStrata);
            this.Controls.Add(this.cmbFields);
            this.Controls.Add(this.btnOpenPlots);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbPlots);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFiaSummarizePoly";
            this.Text = "Summarize Plots by Polygon";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenStands;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.ListBox lsbFields;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnOpenValueRaster;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbStrata;
        private System.Windows.Forms.ComboBox cmbFields;
        private System.Windows.Forms.Button btnOpenPlots;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbPlots;
        private System.Windows.Forms.ComboBox cmbStands;
    }
}