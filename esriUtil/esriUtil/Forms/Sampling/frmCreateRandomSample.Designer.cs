namespace esriUtil.Forms.Sampling
{
    partial class frmCreateRandomSample
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCreateRandomSample));
            this.cmbRst = new System.Windows.Forms.ComboBox();
            this.txtOutWorkspace = new System.Windows.Forms.TextBox();
            this.nudSamples = new System.Windows.Forms.NumericUpDown();
            this.lblNS = new System.Windows.Forms.Label();
            this.btnCreateSamples = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSampleName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOpenWorkspace = new System.Windows.Forms.Button();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudSamples)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbRst
            // 
            this.cmbRst.FormattingEnabled = true;
            this.cmbRst.Location = new System.Drawing.Point(12, 25);
            this.cmbRst.Name = "cmbRst";
            this.cmbRst.Size = new System.Drawing.Size(226, 21);
            this.cmbRst.TabIndex = 1;
            // 
            // txtOutWorkspace
            // 
            this.txtOutWorkspace.Location = new System.Drawing.Point(12, 70);
            this.txtOutWorkspace.Name = "txtOutWorkspace";
            this.txtOutWorkspace.Size = new System.Drawing.Size(226, 20);
            this.txtOutWorkspace.TabIndex = 2;
            // 
            // nudSamples
            // 
            this.nudSamples.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudSamples.Location = new System.Drawing.Point(15, 164);
            this.nudSamples.Maximum = new decimal(new int[] {
            200000,
            0,
            0,
            0});
            this.nudSamples.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSamples.Name = "nudSamples";
            this.nudSamples.Size = new System.Drawing.Size(120, 20);
            this.nudSamples.TabIndex = 4;
            this.nudSamples.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblNS
            // 
            this.lblNS.AutoSize = true;
            this.lblNS.Location = new System.Drawing.Point(12, 147);
            this.lblNS.Name = "lblNS";
            this.lblNS.Size = new System.Drawing.Size(99, 13);
            this.lblNS.TabIndex = 5;
            this.lblNS.Text = "Number of Samples";
            // 
            // btnCreateSamples
            // 
            this.btnCreateSamples.Location = new System.Drawing.Point(162, 187);
            this.btnCreateSamples.Name = "btnCreateSamples";
            this.btnCreateSamples.Size = new System.Drawing.Size(105, 23);
            this.btnCreateSamples.TabIndex = 6;
            this.btnCreateSamples.Text = "Create Sample";
            this.btnCreateSamples.UseVisualStyleBackColor = true;
            this.btnCreateSamples.Click += new System.EventHandler(this.btnCreateSamples_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Raster to Sample";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Output Workspace";
            // 
            // txtSampleName
            // 
            this.txtSampleName.Location = new System.Drawing.Point(12, 116);
            this.txtSampleName.Name = "txtSampleName";
            this.txtSampleName.Size = new System.Drawing.Size(223, 20);
            this.txtSampleName.TabIndex = 10;
            this.txtSampleName.Text = "RasterSample";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Name of New Sample File";
            // 
            // btnOpenWorkspace
            // 
            this.btnOpenWorkspace.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenWorkspace.Location = new System.Drawing.Point(241, 67);
            this.btnOpenWorkspace.Name = "btnOpenWorkspace";
            this.btnOpenWorkspace.Size = new System.Drawing.Size(26, 25);
            this.btnOpenWorkspace.TabIndex = 7;
            this.btnOpenWorkspace.UseVisualStyleBackColor = true;
            this.btnOpenWorkspace.Click += new System.EventHandler(this.btnOpenWorkspace_Click);
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(241, 23);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(26, 25);
            this.btnOpenRaster.TabIndex = 0;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // frmCreateRandomSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 224);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSampleName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOpenWorkspace);
            this.Controls.Add(this.btnCreateSamples);
            this.Controls.Add(this.lblNS);
            this.Controls.Add(this.nudSamples);
            this.Controls.Add(this.txtOutWorkspace);
            this.Controls.Add(this.cmbRst);
            this.Controls.Add(this.btnOpenRaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCreateRandomSample";
            this.Text = "Create Random Sample";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudSamples)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.ComboBox cmbRst;
        private System.Windows.Forms.TextBox txtOutWorkspace;
        private System.Windows.Forms.NumericUpDown nudSamples;
        private System.Windows.Forms.Label lblNS;
        private System.Windows.Forms.Button btnCreateSamples;
        private System.Windows.Forms.Button btnOpenWorkspace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSampleName;
        private System.Windows.Forms.Label label4;
    }
}