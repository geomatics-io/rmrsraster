namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmSetNull
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetNull));
            this.label3 = new System.Windows.Forms.Label();
            this.txtOutNm = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.dgvRanges = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbInRaster1 = new System.Windows.Forms.ComboBox();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.Band = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRanges)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 79;
            this.label3.Text = "Out Raster Name";
            // 
            // txtOutNm
            // 
            this.txtOutNm.Location = new System.Drawing.Point(8, 259);
            this.txtOutNm.Name = "txtOutNm";
            this.txtOutNm.Size = new System.Drawing.Size(210, 20);
            this.txtOutNm.TabIndex = 78;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(224, 257);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(56, 23);
            this.btnExecute.TabIndex = 77;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // dgvRanges
            // 
            this.dgvRanges.AllowUserToAddRows = false;
            this.dgvRanges.AllowUserToDeleteRows = false;
            this.dgvRanges.AllowUserToResizeColumns = false;
            this.dgvRanges.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.NullValue = "0";
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRanges.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRanges.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRanges.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Band,
            this.clmMin,
            this.clmMax});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.Format = "N2";
            dataGridViewCellStyle5.NullValue = "0";
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRanges.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvRanges.Location = new System.Drawing.Point(8, 62);
            this.dgvRanges.Name = "dgvRanges";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRanges.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvRanges.RowHeadersVisible = false;
            this.dgvRanges.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvRanges.Size = new System.Drawing.Size(239, 171);
            this.dgvRanges.TabIndex = 76;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 73;
            this.label2.Text = "In Raster";
            // 
            // cmbInRaster1
            // 
            this.cmbInRaster1.FormattingEnabled = true;
            this.cmbInRaster1.Location = new System.Drawing.Point(8, 24);
            this.cmbInRaster1.Name = "cmbInRaster1";
            this.cmbInRaster1.Size = new System.Drawing.Size(239, 21);
            this.cmbInRaster1.TabIndex = 72;
            this.cmbInRaster1.SelectedIndexChanged += new System.EventHandler(this.cmbInRaster1_SelectedIndexChanged);
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(253, 22);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(27, 27);
            this.btnOpenRaster.TabIndex = 71;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // Band
            // 
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = "0";
            this.Band.DefaultCellStyle = dataGridViewCellStyle2;
            this.Band.HeaderText = "Band";
            this.Band.Name = "Band";
            this.Band.ReadOnly = true;
            // 
            // clmMin
            // 
            this.clmMin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Format = "N0";
            this.clmMin.DefaultCellStyle = dataGridViewCellStyle3;
            this.clmMin.HeaderText = ">= Min";
            this.clmMin.Name = "clmMin";
            this.clmMin.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmMin.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clmMax
            // 
            this.clmMax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Format = "N0";
            this.clmMax.DefaultCellStyle = dataGridViewCellStyle4;
            this.clmMax.HeaderText = "< Max";
            this.clmMax.Name = "clmMax";
            // 
            // frmSetNull
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 294);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtOutNm);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.dgvRanges);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbInRaster1);
            this.Controls.Add(this.btnOpenRaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSetNull";
            this.Text = "Set Null";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dgvRanges)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOutNm;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.DataGridView dgvRanges;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbInRaster1;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.DataGridViewTextBoxColumn Band;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmMax;
    }
}