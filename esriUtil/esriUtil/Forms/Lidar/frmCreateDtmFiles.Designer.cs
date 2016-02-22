namespace esriUtil.Forms.Lidar
{
    partial class frmCreateDtmFiles
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCreateDtmFiles));
            this.label4 = new System.Windows.Forms.Label();
            this.nudCellSize = new System.Windows.Forms.NumericUpDown();
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLasDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOutDir = new System.Windows.Forms.Button();
            this.btnOpenLasDir = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudCellSize)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 65;
            this.label4.Text = "Cell Size";
            // 
            // nudCellSize
            // 
            this.nudCellSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudCellSize.Location = new System.Drawing.Point(12, 127);
            this.nudCellSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudCellSize.Name = "nudCellSize";
            this.nudCellSize.Size = new System.Drawing.Size(80, 20);
            this.nudCellSize.TabIndex = 64;
            this.nudCellSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(186, 122);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(74, 27);
            this.btnExecute.TabIndex = 63;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txtOutDir
            // 
            this.txtOutDir.Location = new System.Drawing.Point(12, 80);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(215, 20);
            this.txtOutDir.TabIndex = 62;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(205, 13);
            this.label2.TabIndex = 60;
            this.label2.Text = "Output Digital Terrain Model .dtm directory";
            // 
            // txtLasDir
            // 
            this.txtLasDir.Location = new System.Drawing.Point(12, 32);
            this.txtLasDir.Name = "txtLasDir";
            this.txtLasDir.Size = new System.Drawing.Size(215, 20);
            this.txtLasDir.TabIndex = 59;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 57;
            this.label1.Text = "Filtered LiDAR .las Directory";
            // 
            // btnOutDir
            // 
            this.btnOutDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutDir.Location = new System.Drawing.Point(233, 76);
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.Size = new System.Drawing.Size(27, 28);
            this.btnOutDir.TabIndex = 61;
            this.btnOutDir.UseVisualStyleBackColor = true;
            this.btnOutDir.Click += new System.EventHandler(this.btnDtm_Click);
            // 
            // btnOpenLasDir
            // 
            this.btnOpenLasDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenLasDir.Location = new System.Drawing.Point(233, 28);
            this.btnOpenLasDir.Name = "btnOpenLasDir";
            this.btnOpenLasDir.Size = new System.Drawing.Size(27, 28);
            this.btnOpenLasDir.TabIndex = 58;
            this.btnOpenLasDir.UseVisualStyleBackColor = true;
            this.btnOpenLasDir.Click += new System.EventHandler(this.btnOpenLasDir_Click);
            // 
            // frmCreateDtmFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 168);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nudCellSize);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtOutDir);
            this.Controls.Add(this.btnOutDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLasDir);
            this.Controls.Add(this.btnOpenLasDir);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCreateDtmFiles";
            this.Text = "Create DTM Files";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudCellSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudCellSize;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Button btnOutDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLasDir;
        private System.Windows.Forms.Button btnOpenLasDir;
        private System.Windows.Forms.Label label1;
    }
}