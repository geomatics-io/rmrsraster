namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmFocalSample
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFocalSample));
            this.btnExecute = new System.Windows.Forms.Button();
            this.cmbFocalStat = new System.Windows.Forms.ComboBox();
            this.txtOutName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbRaster = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnPlus = new System.Windows.Forms.Button();
            this.dgvAzDs = new System.Windows.Forms.DataGridView();
            this.clmAzimuth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDistance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAzDs)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(183, 306);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 50;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // cmbFocalStat
            // 
            this.cmbFocalStat.FormattingEnabled = true;
            this.cmbFocalStat.Location = new System.Drawing.Point(11, 78);
            this.cmbFocalStat.Name = "cmbFocalStat";
            this.cmbFocalStat.Size = new System.Drawing.Size(121, 21);
            this.cmbFocalStat.TabIndex = 43;
            // 
            // txtOutName
            // 
            this.txtOutName.Location = new System.Drawing.Point(11, 306);
            this.txtOutName.Name = "txtOutName";
            this.txtOutName.Size = new System.Drawing.Size(148, 20);
            this.txtOutName.TabIndex = 49;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 288);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 48;
            this.label2.Text = "Output Name";
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(237, 28);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(25, 27);
            this.btnOpenRaster.TabIndex = 47;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 44;
            this.label4.Text = "Focal Statistic";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "Input Raster";
            // 
            // cmbRaster
            // 
            this.cmbRaster.FormattingEnabled = true;
            this.cmbRaster.Location = new System.Drawing.Point(11, 31);
            this.cmbRaster.Name = "cmbRaster";
            this.cmbRaster.Size = new System.Drawing.Size(220, 21);
            this.cmbRaster.TabIndex = 45;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 54;
            this.label3.Text = "Offset from center";
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(222, 156);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(27, 23);
            this.btnMinus.TabIndex = 53;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnPlus
            // 
            this.btnPlus.Location = new System.Drawing.Point(222, 127);
            this.btnPlus.Name = "btnPlus";
            this.btnPlus.Size = new System.Drawing.Size(27, 23);
            this.btnPlus.TabIndex = 52;
            this.btnPlus.Text = "+";
            this.btnPlus.UseVisualStyleBackColor = true;
            this.btnPlus.Click += new System.EventHandler(this.btnPlus_Click);
            // 
            // dgvAzDs
            // 
            this.dgvAzDs.AllowUserToAddRows = false;
            this.dgvAzDs.AllowUserToDeleteRows = false;
            this.dgvAzDs.AllowUserToResizeColumns = false;
            this.dgvAzDs.AllowUserToResizeRows = false;
            this.dgvAzDs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAzDs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAzDs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAzDs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmAzimuth,
            this.clmDistance});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAzDs.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvAzDs.Location = new System.Drawing.Point(11, 127);
            this.dgvAzDs.Name = "dgvAzDs";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAzDs.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvAzDs.RowHeadersVisible = false;
            this.dgvAzDs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvAzDs.Size = new System.Drawing.Size(207, 150);
            this.dgvAzDs.TabIndex = 51;
            // 
            // clmAzimuth
            // 
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = null;
            this.clmAzimuth.DefaultCellStyle = dataGridViewCellStyle2;
            this.clmAzimuth.HeaderText = "Azimuth";
            this.clmAzimuth.Name = "clmAzimuth";
            // 
            // clmDistance
            // 
            dataGridViewCellStyle3.Format = "N0";
            dataGridViewCellStyle3.NullValue = null;
            this.clmDistance.DefaultCellStyle = dataGridViewCellStyle3;
            this.clmDistance.HeaderText = "Distance";
            this.clmDistance.Name = "clmDistance";
            // 
            // frmFocalSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 336);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.btnPlus);
            this.Controls.Add(this.dgvAzDs);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.cmbFocalStat);
            this.Controls.Add(this.txtOutName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOpenRaster);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbRaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFocalSample";
            this.Text = "Focal Sample Analysis";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dgvAzDs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.ComboBox cmbFocalStat;
        private System.Windows.Forms.TextBox txtOutName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRaster;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnPlus;
        private System.Windows.Forms.DataGridView dgvAzDs;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmAzimuth;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmDistance;
    }
}