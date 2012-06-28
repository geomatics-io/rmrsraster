namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmClipRaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmClipRaster));
            this.btnClip = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbRaster = new System.Windows.Forms.ComboBox();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.chbErase = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOutRasterName = new System.Windows.Forms.TextBox();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClip
            // 
            this.btnClip.Location = new System.Drawing.Point(215, 161);
            this.btnClip.Name = "btnClip";
            this.btnClip.Size = new System.Drawing.Size(45, 22);
            this.btnClip.TabIndex = 13;
            this.btnClip.Text = "Clip";
            this.btnClip.UseVisualStyleBackColor = true;
            this.btnClip.Click += new System.EventHandler(this.btnClip_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Raster to Clip";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Clipping Polygon";
            // 
            // cmbRaster
            // 
            this.cmbRaster.FormattingEnabled = true;
            this.cmbRaster.Location = new System.Drawing.Point(12, 77);
            this.cmbRaster.Name = "cmbRaster";
            this.cmbRaster.Size = new System.Drawing.Size(211, 21);
            this.cmbRaster.TabIndex = 10;
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(12, 28);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(210, 21);
            this.cmbSampleFeatureClass.TabIndex = 9;
            // 
            // chbErase
            // 
            this.chbErase.AutoSize = true;
            this.chbErase.Location = new System.Drawing.Point(14, 161);
            this.chbErase.Name = "chbErase";
            this.chbErase.Size = new System.Drawing.Size(53, 17);
            this.chbErase.TabIndex = 15;
            this.chbErase.Text = "Erase";
            this.chbErase.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Out Raster Name";
            // 
            // txtOutRasterName
            // 
            this.txtOutRasterName.Location = new System.Drawing.Point(12, 126);
            this.txtOutRasterName.Name = "txtOutRasterName";
            this.txtOutRasterName.Size = new System.Drawing.Size(208, 20);
            this.txtOutRasterName.TabIndex = 18;
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(233, 24);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(27, 28);
            this.btnOpenFeatureClass.TabIndex = 14;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeatureClass_Click);
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(233, 75);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(27, 27);
            this.btnOpenRaster.TabIndex = 8;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // frmClipRaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 187);
            this.Controls.Add(this.txtOutRasterName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chbErase);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.btnClip);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbRaster);
            this.Controls.Add(this.cmbSampleFeatureClass);
            this.Controls.Add(this.btnOpenRaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmClipRaster";
            this.Text = "Clip Raster";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Button btnClip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRaster;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.CheckBox chbErase;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOutRasterName;
    }
}