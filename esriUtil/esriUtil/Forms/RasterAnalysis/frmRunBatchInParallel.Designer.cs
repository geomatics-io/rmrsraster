namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmRunBatchInParallel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRunBatchInParallel));
            this.txtRasterPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOutWks = new System.Windows.Forms.Button();
            this.txtOutWks = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFtrCls = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtRasterPath
            // 
            this.txtRasterPath.Location = new System.Drawing.Point(12, 33);
            this.txtRasterPath.Name = "txtRasterPath";
            this.txtRasterPath.Size = new System.Drawing.Size(211, 20);
            this.txtRasterPath.TabIndex = 58;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 57;
            this.label4.Text = "Output Workspace";
            // 
            // btnOutWks
            // 
            this.btnOutWks.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutWks.Location = new System.Drawing.Point(233, 131);
            this.btnOutWks.Name = "btnOutWks";
            this.btnOutWks.Size = new System.Drawing.Size(27, 27);
            this.btnOutWks.TabIndex = 56;
            this.btnOutWks.UseVisualStyleBackColor = true;
            // 
            // txtOutWks
            // 
            this.txtOutWks.Location = new System.Drawing.Point(12, 135);
            this.txtOutWks.Name = "txtOutWks";
            this.txtOutWks.Size = new System.Drawing.Size(211, 20);
            this.txtOutWks.TabIndex = 55;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 54;
            this.label1.Text = "Tiles (Feature Class)";
            // 
            // cmbFtrCls
            // 
            this.cmbFtrCls.FormattingEnabled = true;
            this.cmbFtrCls.Location = new System.Drawing.Point(12, 82);
            this.cmbFtrCls.Name = "cmbFtrCls";
            this.cmbFtrCls.Size = new System.Drawing.Size(211, 21);
            this.cmbFtrCls.TabIndex = 53;
            // 
            // button1
            // 
            this.button1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button1.Location = new System.Drawing.Point(233, 78);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 27);
            this.button1.TabIndex = 52;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 51;
            this.label2.Text = "Input Directory Path";
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(233, 29);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(27, 27);
            this.btnOpenRaster.TabIndex = 50;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            // 
            // frmRunBatchInParallel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 203);
            this.Controls.Add(this.txtRasterPath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnOutWks);
            this.Controls.Add(this.txtOutWks);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbFtrCls);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOpenRaster);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRunBatchInParallel";
            this.Text = "Run Batch In Parrallel";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRasterPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOutWks;
        private System.Windows.Forms.TextBox txtOutWks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbFtrCls;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOpenRaster;
    }
}