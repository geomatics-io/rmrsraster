namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmLinearTransform
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLinearTransform));
            this.btnClear = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbInRaster1 = new System.Windows.Forms.ComboBox();
            this.dgvRasterSlopes = new System.Windows.Forms.DataGridView();
            this.clmRasterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSlopeValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nudIntercept = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOutNm = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRasterSlopes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntercept)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(255, 127);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(27, 23);
            this.btnClear.TabIndex = 53;
            this.btnClear.Text = "!";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(255, 101);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(27, 23);
            this.btnMinus.TabIndex = 52;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 50;
            this.label2.Text = "Select Slope Rasters";
            // 
            // cmbInRaster1
            // 
            this.cmbInRaster1.FormattingEnabled = true;
            this.cmbInRaster1.Location = new System.Drawing.Point(10, 74);
            this.cmbInRaster1.Name = "cmbInRaster1";
            this.cmbInRaster1.Size = new System.Drawing.Size(239, 21);
            this.cmbInRaster1.TabIndex = 49;
            this.cmbInRaster1.SelectedIndexChanged += new System.EventHandler(this.cmbInRaster1_SelectedIndexChanged);
            // 
            // dgvRasterSlopes
            // 
            this.dgvRasterSlopes.AllowUserToAddRows = false;
            this.dgvRasterSlopes.AllowUserToDeleteRows = false;
            this.dgvRasterSlopes.AllowUserToResizeColumns = false;
            this.dgvRasterSlopes.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRasterSlopes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRasterSlopes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRasterSlopes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmRasterName,
            this.clmSlopeValue});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.Format = "N2";
            dataGridViewCellStyle3.NullValue = "0";
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRasterSlopes.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvRasterSlopes.Location = new System.Drawing.Point(10, 101);
            this.dgvRasterSlopes.Name = "dgvRasterSlopes";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRasterSlopes.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvRasterSlopes.RowHeadersVisible = false;
            this.dgvRasterSlopes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvRasterSlopes.Size = new System.Drawing.Size(239, 171);
            this.dgvRasterSlopes.TabIndex = 55;
            // 
            // clmRasterName
            // 
            this.clmRasterName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmRasterName.HeaderText = "Raster";
            this.clmRasterName.Name = "clmRasterName";
            this.clmRasterName.ReadOnly = true;
            this.clmRasterName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clmRasterName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmRasterName.Width = 44;
            // 
            // clmSlopeValue
            // 
            this.clmSlopeValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Format = "N5";
            dataGridViewCellStyle2.NullValue = null;
            this.clmSlopeValue.DefaultCellStyle = dataGridViewCellStyle2;
            this.clmSlopeValue.HeaderText = "Slope";
            this.clmSlopeValue.Name = "clmSlopeValue";
            // 
            // nudIntercept
            // 
            this.nudIntercept.Location = new System.Drawing.Point(10, 28);
            this.nudIntercept.Maximum = new decimal(new int[] {
            276447232,
            23283,
            0,
            0});
            this.nudIntercept.Minimum = new decimal(new int[] {
            -727379968,
            232,
            0,
            -2147483648});
            this.nudIntercept.Name = "nudIntercept";
            this.nudIntercept.Size = new System.Drawing.Size(239, 20);
            this.nudIntercept.TabIndex = 56;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 57;
            this.label1.Text = "Intercept";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 280);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 60;
            this.label3.Text = "Out Raster Name";
            // 
            // txtOutNm
            // 
            this.txtOutNm.Location = new System.Drawing.Point(10, 298);
            this.txtOutNm.Name = "txtOutNm";
            this.txtOutNm.Size = new System.Drawing.Size(210, 20);
            this.txtOutNm.TabIndex = 59;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(226, 296);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(56, 23);
            this.btnExecute.TabIndex = 58;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(255, 72);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(27, 27);
            this.btnOpenRaster.TabIndex = 48;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // frmLinearTransform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 329);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtOutNm);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudIntercept);
            this.Controls.Add(this.dgvRasterSlopes);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbInRaster1);
            this.Controls.Add(this.btnOpenRaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLinearTransform";
            this.Text = "Linear Transform";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dgvRasterSlopes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntercept)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbInRaster1;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.DataGridView dgvRasterSlopes;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmRasterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSlopeValue;
        private System.Windows.Forms.NumericUpDown nudIntercept;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOutNm;
        private System.Windows.Forms.Button btnExecute;
    }
}