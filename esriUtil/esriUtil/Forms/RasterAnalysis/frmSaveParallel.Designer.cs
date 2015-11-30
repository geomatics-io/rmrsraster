namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmSaveParallel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSaveParallel));
            this.label2 = new System.Windows.Forms.Label();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFtrCls = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOutWks = new System.Windows.Forms.Button();
            this.txtOutWks = new System.Windows.Forms.TextBox();
            this.nudBS = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.lblNoDataVl = new System.Windows.Forms.Label();
            this.txtNoDataVl = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtOutName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtRasterPath = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudBS)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Raster Path";
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(233, 29);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(27, 27);
            this.btnOpenRaster.TabIndex = 28;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Tiles (Feature Class)";
            // 
            // cmbFtrCls
            // 
            this.cmbFtrCls.FormattingEnabled = true;
            this.cmbFtrCls.Location = new System.Drawing.Point(12, 82);
            this.cmbFtrCls.Name = "cmbFtrCls";
            this.cmbFtrCls.Size = new System.Drawing.Size(211, 21);
            this.cmbFtrCls.TabIndex = 32;
            // 
            // button1
            // 
            this.button1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button1.Location = new System.Drawing.Point(233, 78);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 27);
            this.button1.TabIndex = 31;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 39;
            this.label4.Text = "Output Workspace";
            // 
            // btnOutWks
            // 
            this.btnOutWks.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutWks.Location = new System.Drawing.Point(233, 131);
            this.btnOutWks.Name = "btnOutWks";
            this.btnOutWks.Size = new System.Drawing.Size(27, 27);
            this.btnOutWks.TabIndex = 38;
            this.btnOutWks.UseVisualStyleBackColor = true;
            this.btnOutWks.Click += new System.EventHandler(this.btnOutWks_Click);
            // 
            // txtOutWks
            // 
            this.txtOutWks.Location = new System.Drawing.Point(12, 135);
            this.txtOutWks.Name = "txtOutWks";
            this.txtOutWks.Size = new System.Drawing.Size(211, 20);
            this.txtOutWks.TabIndex = 37;
            // 
            // nudBS
            // 
            this.nudBS.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudBS.Location = new System.Drawing.Point(12, 228);
            this.nudBS.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.nudBS.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudBS.Name = "nudBS";
            this.nudBS.Size = new System.Drawing.Size(119, 20);
            this.nudBS.TabIndex = 48;
            this.nudBS.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 210);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 47;
            this.label5.Text = "Block Size";
            // 
            // lblNoDataVl
            // 
            this.lblNoDataVl.AutoSize = true;
            this.lblNoDataVl.Location = new System.Drawing.Point(137, 209);
            this.lblNoDataVl.Name = "lblNoDataVl";
            this.lblNoDataVl.Size = new System.Drawing.Size(74, 13);
            this.lblNoDataVl.TabIndex = 46;
            this.lblNoDataVl.Text = "NoData Value";
            // 
            // txtNoDataVl
            // 
            this.txtNoDataVl.Location = new System.Drawing.Point(137, 227);
            this.txtNoDataVl.Name = "txtNoDataVl";
            this.txtNoDataVl.Size = new System.Drawing.Size(86, 20);
            this.txtNoDataVl.TabIndex = 45;
            this.txtNoDataVl.Text = "null";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(134, 165);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 44;
            this.label6.Text = "Out Type";
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(137, 184);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(86, 21);
            this.cmbType.TabIndex = 43;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 42;
            this.label7.Text = "Out Prefix";
            // 
            // txtOutName
            // 
            this.txtOutName.Location = new System.Drawing.Point(12, 184);
            this.txtOutName.Name = "txtOutName";
            this.txtOutName.Size = new System.Drawing.Size(119, 20);
            this.txtOutName.TabIndex = 41;
            this.txtOutName.Text = "Tile";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(197, 256);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(63, 22);
            this.btnSave.TabIndex = 40;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtRasterPath
            // 
            this.txtRasterPath.Location = new System.Drawing.Point(12, 33);
            this.txtRasterPath.Name = "txtRasterPath";
            this.txtRasterPath.Size = new System.Drawing.Size(211, 20);
            this.txtRasterPath.TabIndex = 49;
            // 
            // frmSaveParallel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 287);
            this.Controls.Add(this.txtRasterPath);
            this.Controls.Add(this.nudBS);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblNoDataVl);
            this.Controls.Add(this.txtNoDataVl);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtOutName);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnOutWks);
            this.Controls.Add(this.txtOutWks);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbFtrCls);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOpenRaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSaveParallel";
            this.Text = "Save Parallel";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudBS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbFtrCls;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOutWks;
        private System.Windows.Forms.TextBox txtOutWks;
        private System.Windows.Forms.NumericUpDown nudBS;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblNoDataVl;
        private System.Windows.Forms.TextBox txtNoDataVl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtOutName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtRasterPath;
    }
}