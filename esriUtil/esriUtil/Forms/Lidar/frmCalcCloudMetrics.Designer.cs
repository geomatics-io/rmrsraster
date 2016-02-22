namespace esriUtil.Forms.Lidar
{
    partial class frmCalcCloudMetrics
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCalcCloudMetrics));
            this.label4 = new System.Windows.Forms.Label();
            this.nudRadius = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLasDir = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSample = new System.Windows.Forms.Button();
            this.btnOpenLasDir = new System.Windows.Forms.Button();
            this.cmbSample = new System.Windows.Forms.ComboBox();
            this.cmbShape = new System.Windows.Forms.ComboBox();
            this.nudCuttOffBelow = new System.Windows.Forms.NumericUpDown();
            this.nudCuttOffAbove = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDTM = new System.Windows.Forms.TextBox();
            this.btnDtm = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCuttOffBelow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCuttOffAbove)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Plot Radius";
            // 
            // nudRadius
            // 
            this.nudRadius.DecimalPlaces = 1;
            this.nudRadius.Location = new System.Drawing.Point(12, 183);
            this.nudRadius.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudRadius.Name = "nudRadius";
            this.nudRadius.Size = new System.Drawing.Size(74, 20);
            this.nudRadius.TabIndex = 29;
            this.nudRadius.Value = new decimal(new int[] {
            115,
            0,
            0,
            65536});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Sample Locations";
            // 
            // txtLasDir
            // 
            this.txtLasDir.Location = new System.Drawing.Point(12, 75);
            this.txtLasDir.Name = "txtLasDir";
            this.txtLasDir.Size = new System.Drawing.Size(215, 20);
            this.txtLasDir.TabIndex = 23;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(186, 230);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(74, 27);
            this.btnExecute.TabIndex = 22;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "LiDAR .las Directory";
            // 
            // btnSample
            // 
            this.btnSample.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnSample.Location = new System.Drawing.Point(233, 21);
            this.btnSample.Name = "btnSample";
            this.btnSample.Size = new System.Drawing.Size(27, 28);
            this.btnSample.TabIndex = 25;
            this.btnSample.UseVisualStyleBackColor = true;
            this.btnSample.Click += new System.EventHandler(this.btnSample_Click);
            // 
            // btnOpenLasDir
            // 
            this.btnOpenLasDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenLasDir.Location = new System.Drawing.Point(233, 71);
            this.btnOpenLasDir.Name = "btnOpenLasDir";
            this.btnOpenLasDir.Size = new System.Drawing.Size(27, 28);
            this.btnOpenLasDir.TabIndex = 21;
            this.btnOpenLasDir.UseVisualStyleBackColor = true;
            this.btnOpenLasDir.Click += new System.EventHandler(this.btnOpenLasDir_Click);
            // 
            // cmbSample
            // 
            this.cmbSample.FormattingEnabled = true;
            this.cmbSample.Location = new System.Drawing.Point(13, 25);
            this.cmbSample.Name = "cmbSample";
            this.cmbSample.Size = new System.Drawing.Size(214, 21);
            this.cmbSample.TabIndex = 31;
            // 
            // cmbShape
            // 
            this.cmbShape.FormattingEnabled = true;
            this.cmbShape.Items.AddRange(new object[] {
            "Rectangle",
            "Circle"});
            this.cmbShape.Location = new System.Drawing.Point(12, 230);
            this.cmbShape.Name = "cmbShape";
            this.cmbShape.Size = new System.Drawing.Size(80, 21);
            this.cmbShape.TabIndex = 32;
            this.cmbShape.Text = "Rectangle";
            // 
            // nudCuttOffBelow
            // 
            this.nudCuttOffBelow.Location = new System.Drawing.Point(92, 183);
            this.nudCuttOffBelow.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudCuttOffBelow.Name = "nudCuttOffBelow";
            this.nudCuttOffBelow.Size = new System.Drawing.Size(76, 20);
            this.nudCuttOffBelow.TabIndex = 33;
            // 
            // nudCuttOffAbove
            // 
            this.nudCuttOffAbove.Location = new System.Drawing.Point(177, 183);
            this.nudCuttOffAbove.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudCuttOffAbove.Name = "nudCuttOffAbove";
            this.nudCuttOffAbove.Size = new System.Drawing.Size(78, 20);
            this.nudCuttOffAbove.TabIndex = 34;
            this.nudCuttOffAbove.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "Shape";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(89, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Remove Below";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(174, 164);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 37;
            this.label6.Text = "Remove Above";
            // 
            // txtDTM
            // 
            this.txtDTM.Location = new System.Drawing.Point(14, 125);
            this.txtDTM.Name = "txtDTM";
            this.txtDTM.Size = new System.Drawing.Size(215, 20);
            this.txtDTM.TabIndex = 40;
            // 
            // btnDtm
            // 
            this.btnDtm.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnDtm.Location = new System.Drawing.Point(235, 121);
            this.btnDtm.Name = "btnDtm";
            this.btnDtm.Size = new System.Drawing.Size(27, 28);
            this.btnDtm.TabIndex = 39;
            this.btnDtm.UseVisualStyleBackColor = true;
            this.btnDtm.Click += new System.EventHandler(this.dtm_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 13);
            this.label7.TabIndex = 38;
            this.label7.Text = "DTM .dtm Directory";
            // 
            // frmCalcCloudMetrics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 275);
            this.Controls.Add(this.txtDTM);
            this.Controls.Add(this.btnDtm);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudCuttOffAbove);
            this.Controls.Add(this.nudCuttOffBelow);
            this.Controls.Add(this.cmbShape);
            this.Controls.Add(this.cmbSample);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nudRadius);
            this.Controls.Add(this.btnSample);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLasDir);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnOpenLasDir);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCalcCloudMetrics";
            this.Text = "Calc Cloud Metrics";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCuttOffBelow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCuttOffAbove)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudRadius;
        private System.Windows.Forms.Button btnSample;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLasDir;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnOpenLasDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSample;
        private System.Windows.Forms.ComboBox cmbShape;
        private System.Windows.Forms.NumericUpDown nudCuttOffBelow;
        private System.Windows.Forms.NumericUpDown nudCuttOffAbove;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDTM;
        private System.Windows.Forms.Button btnDtm;
        private System.Windows.Forms.Label label7;
    }
}