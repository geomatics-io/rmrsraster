namespace esriUtil.Forms.Lidar
{
    partial class frmCalcGridMetricsFusion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCalcGridMetricsFusion));
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpenLasDir = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtLasDir = new System.Windows.Forms.TextBox();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.btnOutDir = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nudCellSize = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nudCuttOffAbove = new System.Windows.Forms.NumericUpDown();
            this.nudCuttOffBelow = new System.Windows.Forms.NumericUpDown();
            this.txtDTM = new System.Windows.Forms.TextBox();
            this.btnDtm = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudCellSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCuttOffAbove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCuttOffBelow)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "LiDAR .las Directory";
            // 
            // btnOpenLasDir
            // 
            this.btnOpenLasDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenLasDir.Location = new System.Drawing.Point(233, 28);
            this.btnOpenLasDir.Name = "btnOpenLasDir";
            this.btnOpenLasDir.Size = new System.Drawing.Size(27, 28);
            this.btnOpenLasDir.TabIndex = 10;
            this.btnOpenLasDir.UseVisualStyleBackColor = true;
            this.btnOpenLasDir.Click += new System.EventHandler(this.btnOpenLasDir_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(186, 225);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(74, 27);
            this.btnExecute.TabIndex = 11;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txtLasDir
            // 
            this.txtLasDir.Location = new System.Drawing.Point(12, 32);
            this.txtLasDir.Name = "txtLasDir";
            this.txtLasDir.Size = new System.Drawing.Size(215, 20);
            this.txtLasDir.TabIndex = 12;
            // 
            // txtOutDir
            // 
            this.txtOutDir.Location = new System.Drawing.Point(12, 82);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(215, 20);
            this.txtOutDir.TabIndex = 15;
            // 
            // btnOutDir
            // 
            this.btnOutDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutDir.Location = new System.Drawing.Point(233, 78);
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.Size = new System.Drawing.Size(27, 28);
            this.btnOutDir.TabIndex = 14;
            this.btnOutDir.UseVisualStyleBackColor = true;
            this.btnOutDir.Click += new System.EventHandler(this.btnOutDir_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Output Directory";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Cell Size";
            // 
            // nudCellSize
            // 
            this.nudCellSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudCellSize.Location = new System.Drawing.Point(12, 181);
            this.nudCellSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudCellSize.Name = "nudCellSize";
            this.nudCellSize.Size = new System.Drawing.Size(80, 20);
            this.nudCellSize.TabIndex = 18;
            this.nudCellSize.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(183, 162);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 41;
            this.label6.Text = "Remove Above";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(98, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 40;
            this.label5.Text = "Remove Below";
            // 
            // nudCuttOffAbove
            // 
            this.nudCuttOffAbove.Location = new System.Drawing.Point(186, 181);
            this.nudCuttOffAbove.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudCuttOffAbove.Name = "nudCuttOffAbove";
            this.nudCuttOffAbove.Size = new System.Drawing.Size(78, 20);
            this.nudCuttOffAbove.TabIndex = 39;
            this.nudCuttOffAbove.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            // 
            // nudCuttOffBelow
            // 
            this.nudCuttOffBelow.Location = new System.Drawing.Point(101, 181);
            this.nudCuttOffBelow.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudCuttOffBelow.Name = "nudCuttOffBelow";
            this.nudCuttOffBelow.Size = new System.Drawing.Size(76, 20);
            this.nudCuttOffBelow.TabIndex = 38;
            // 
            // txtDTM
            // 
            this.txtDTM.Location = new System.Drawing.Point(12, 129);
            this.txtDTM.Name = "txtDTM";
            this.txtDTM.Size = new System.Drawing.Size(215, 20);
            this.txtDTM.TabIndex = 44;
            // 
            // btnDtm
            // 
            this.btnDtm.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnDtm.Location = new System.Drawing.Point(233, 125);
            this.btnDtm.Name = "btnDtm";
            this.btnDtm.Size = new System.Drawing.Size(27, 28);
            this.btnDtm.TabIndex = 43;
            this.btnDtm.UseVisualStyleBackColor = true;
            this.btnDtm.Click += new System.EventHandler(this.btnDtm_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 112);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 13);
            this.label7.TabIndex = 42;
            this.label7.Text = "DTM .dtm Directory";
            // 
            // frmCalcGridMetricsFusion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 264);
            this.Controls.Add(this.txtDTM);
            this.Controls.Add(this.btnDtm);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudCuttOffAbove);
            this.Controls.Add(this.nudCuttOffBelow);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nudCellSize);
            this.Controls.Add(this.txtOutDir);
            this.Controls.Add(this.btnOutDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLasDir);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnOpenLasDir);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCalcGridMetricsFusion";
            this.Text = "Calculate Grid Metrics";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudCellSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCuttOffAbove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCuttOffBelow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenLasDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtLasDir;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Button btnOutDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudCellSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudCuttOffAbove;
        private System.Windows.Forms.NumericUpDown nudCuttOffBelow;
        private System.Windows.Forms.TextBox txtDTM;
        private System.Windows.Forms.Button btnDtm;
        private System.Windows.Forms.Label label7;
    }
}