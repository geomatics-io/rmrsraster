namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmShiftRaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmShiftRaster));
            this.txtValueX = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOutName = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbInRaster1 = new System.Windows.Forms.ComboBox();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.txtValueY = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtValueX
            // 
            this.txtValueX.Location = new System.Drawing.Point(12, 81);
            this.txtValueX.Name = "txtValueX";
            this.txtValueX.Size = new System.Drawing.Size(61, 20);
            this.txtValueX.TabIndex = 73;
            this.txtValueX.Validated += new System.EventHandler(this.txtValue_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 72;
            this.label3.Text = "XCells";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // txtOutName
            // 
            this.txtOutName.Location = new System.Drawing.Point(11, 132);
            this.txtOutName.Name = "txtOutName";
            this.txtOutName.Size = new System.Drawing.Size(159, 20);
            this.txtOutName.TabIndex = 71;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(205, 132);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(54, 24);
            this.btnExecute.TabIndex = 70;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnClip_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 69;
            this.label1.Text = "Out Raster Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 68;
            this.label2.Text = "Input Raster";
            // 
            // cmbInRaster1
            // 
            this.cmbInRaster1.FormattingEnabled = true;
            this.cmbInRaster1.Location = new System.Drawing.Point(11, 31);
            this.cmbInRaster1.Name = "cmbInRaster1";
            this.cmbInRaster1.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster1.TabIndex = 67;
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(232, 29);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(27, 27);
            this.btnOpenRaster.TabIndex = 66;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // txtValueY
            // 
            this.txtValueY.Location = new System.Drawing.Point(95, 82);
            this.txtValueY.Name = "txtValueY";
            this.txtValueY.Size = new System.Drawing.Size(57, 20);
            this.txtValueY.TabIndex = 75;
            this.txtValueY.Validated += new System.EventHandler(this.txtValue_Validated);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(99, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 74;
            this.label4.Text = "YCells";
            // 
            // frmShiftRaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 181);
            this.Controls.Add(this.txtValueY);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtValueX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtOutName);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbInRaster1);
            this.Controls.Add(this.btnOpenRaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmShiftRaster";
            this.Text = "Shift Raster";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtValueX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOutName;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbInRaster1;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.TextBox txtValueY;
        private System.Windows.Forms.Label label4;
    }
}