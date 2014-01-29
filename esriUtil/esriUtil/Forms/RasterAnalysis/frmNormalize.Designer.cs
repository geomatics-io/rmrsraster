namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmNormalize
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNormalize));
            this.btnExecute = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOutName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbInRaster2 = new System.Windows.Forms.ComboBox();
            this.btnInRaster2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbInRaster1 = new System.Windows.Forms.ComboBox();
            this.btnInRaster1 = new System.Windows.Forms.Button();
            this.trbPercent = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trbPercent)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(221, 206);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(54, 24);
            this.btnExecute.TabIndex = 32;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 189);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Out Name";
            // 
            // txtOutName
            // 
            this.txtOutName.Location = new System.Drawing.Point(10, 206);
            this.txtOutName.Name = "txtOutName";
            this.txtOutName.Size = new System.Drawing.Size(205, 20);
            this.txtOutName.TabIndex = 30;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Raster To Transform";
            // 
            // cmbInRaster2
            // 
            this.cmbInRaster2.FormattingEnabled = true;
            this.cmbInRaster2.Location = new System.Drawing.Point(10, 79);
            this.cmbInRaster2.Name = "cmbInRaster2";
            this.cmbInRaster2.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster2.TabIndex = 28;
            // 
            // btnInRaster2
            // 
            this.btnInRaster2.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnInRaster2.Location = new System.Drawing.Point(231, 77);
            this.btnInRaster2.Name = "btnInRaster2";
            this.btnInRaster2.Size = new System.Drawing.Size(27, 27);
            this.btnInRaster2.TabIndex = 27;
            this.btnInRaster2.UseVisualStyleBackColor = true;
            this.btnInRaster2.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Reference Raster";
            // 
            // cmbInRaster1
            // 
            this.cmbInRaster1.FormattingEnabled = true;
            this.cmbInRaster1.Location = new System.Drawing.Point(10, 26);
            this.cmbInRaster1.Name = "cmbInRaster1";
            this.cmbInRaster1.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster1.TabIndex = 25;
            // 
            // btnInRaster1
            // 
            this.btnInRaster1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnInRaster1.Location = new System.Drawing.Point(231, 24);
            this.btnInRaster1.Name = "btnInRaster1";
            this.btnInRaster1.Size = new System.Drawing.Size(27, 27);
            this.btnInRaster1.TabIndex = 24;
            this.btnInRaster1.UseVisualStyleBackColor = true;
            this.btnInRaster1.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // trbPercent
            // 
            this.trbPercent.LargeChange = 10;
            this.trbPercent.Location = new System.Drawing.Point(12, 133);
            this.trbPercent.Maximum = 100;
            this.trbPercent.Minimum = 10;
            this.trbPercent.Name = "trbPercent";
            this.trbPercent.Size = new System.Drawing.Size(209, 45);
            this.trbPercent.SmallChange = 10;
            this.trbPercent.TabIndex = 34;
            this.trbPercent.TickFrequency = 10;
            this.trbPercent.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trbPercent.Value = 20;
            this.trbPercent.Scroll += new System.EventHandler(this.trbPercent_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Percent Change";
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(116, 114);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(27, 13);
            this.lblPercent.TabIndex = 36;
            this.lblPercent.Text = "20%";
            // 
            // frmNormalize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 256);
            this.Controls.Add(this.lblPercent);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trbPercent);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOutName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbInRaster2);
            this.Controls.Add(this.btnInRaster2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbInRaster1);
            this.Controls.Add(this.btnInRaster1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmNormalize";
            this.Text = "NormalizeRaster";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.trbPercent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOutName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbInRaster2;
        private System.Windows.Forms.Button btnInRaster2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbInRaster1;
        private System.Windows.Forms.Button btnInRaster1;
        private System.Windows.Forms.TrackBar trbPercent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPercent;
    }
}