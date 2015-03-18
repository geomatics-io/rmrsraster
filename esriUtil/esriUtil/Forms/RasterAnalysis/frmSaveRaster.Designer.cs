namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmSaveRaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSaveRaster));
            this.btnSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbRaster = new System.Windows.Forms.ComboBox();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.txtWorkspace = new System.Windows.Forms.TextBox();
            this.txtOutName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnWorkspace = new System.Windows.Forms.Button();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNoDataVl = new System.Windows.Forms.TextBox();
            this.lblNoDataVl = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nudBS = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudBS)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(199, 199);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(63, 22);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Raster to Save";
            // 
            // cmbRaster
            // 
            this.cmbRaster.FormattingEnabled = true;
            this.cmbRaster.Location = new System.Drawing.Point(14, 38);
            this.cmbRaster.Name = "cmbRaster";
            this.cmbRaster.Size = new System.Drawing.Size(211, 21);
            this.cmbRaster.TabIndex = 8;
            this.cmbRaster.SelectedIndexChanged += new System.EventHandler(this.cmbRaster_SelectedIndexChanged);
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(235, 36);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(27, 27);
            this.btnOpenRaster.TabIndex = 7;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // txtWorkspace
            // 
            this.txtWorkspace.Location = new System.Drawing.Point(14, 83);
            this.txtWorkspace.Name = "txtWorkspace";
            this.txtWorkspace.Size = new System.Drawing.Size(211, 20);
            this.txtWorkspace.TabIndex = 11;
            this.txtWorkspace.TextChanged += new System.EventHandler(this.txtWorkspace_TextChanged);
            // 
            // txtOutName
            // 
            this.txtOutName.Location = new System.Drawing.Point(14, 130);
            this.txtOutName.Name = "txtOutName";
            this.txtOutName.Size = new System.Drawing.Size(119, 20);
            this.txtOutName.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Out Workspace";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Out Name";
            // 
            // btnWorkspace
            // 
            this.btnWorkspace.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnWorkspace.Location = new System.Drawing.Point(235, 79);
            this.btnWorkspace.Name = "btnWorkspace";
            this.btnWorkspace.Size = new System.Drawing.Size(27, 27);
            this.btnWorkspace.TabIndex = 15;
            this.btnWorkspace.UseVisualStyleBackColor = true;
            this.btnWorkspace.Click += new System.EventHandler(this.btnWorkspace_Click);
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(139, 130);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(86, 21);
            this.cmbType.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(136, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Out Type";
            // 
            // txtNoDataVl
            // 
            this.txtNoDataVl.Location = new System.Drawing.Point(139, 173);
            this.txtNoDataVl.Name = "txtNoDataVl";
            this.txtNoDataVl.Size = new System.Drawing.Size(86, 20);
            this.txtNoDataVl.TabIndex = 18;
            this.txtNoDataVl.Visible = false;
            // 
            // lblNoDataVl
            // 
            this.lblNoDataVl.AutoSize = true;
            this.lblNoDataVl.Location = new System.Drawing.Point(139, 155);
            this.lblNoDataVl.Name = "lblNoDataVl";
            this.lblNoDataVl.Size = new System.Drawing.Size(74, 13);
            this.lblNoDataVl.TabIndex = 19;
            this.lblNoDataVl.Text = "NoData Value";
            this.lblNoDataVl.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Block Size";
            // 
            // nudBS
            // 
            this.nudBS.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudBS.Location = new System.Drawing.Point(14, 174);
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
            this.nudBS.TabIndex = 22;
            this.nudBS.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // frmSaveRaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 230);
            this.Controls.Add(this.nudBS);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblNoDataVl);
            this.Controls.Add(this.txtNoDataVl);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.btnWorkspace);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutName);
            this.Controls.Add(this.txtWorkspace);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbRaster);
            this.Controls.Add(this.btnOpenRaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSaveRaster";
            this.Text = "Save Raster";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudBS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbRaster;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.TextBox txtWorkspace;
        private System.Windows.Forms.TextBox txtOutName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnWorkspace;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNoDataVl;
        private System.Windows.Forms.Label lblNoDataVl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudBS;
    }
}