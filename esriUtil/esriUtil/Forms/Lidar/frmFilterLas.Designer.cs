namespace esriUtil.Forms.Lidar
{
    partial class frmFilterLas
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFilterLas));
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLasDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.nudCellSize = new System.Windows.Forms.NumericUpDown();
            this.btnOutDir = new System.Windows.Forms.Button();
            this.btnOpenLasDir = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudCellSize)).BeginInit();
            this.SuspendLayout();
            // 
            // txtOutDir
            // 
            this.txtOutDir.Location = new System.Drawing.Point(12, 82);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(215, 20);
            this.txtOutDir.TabIndex = 50;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 13);
            this.label2.TabIndex = 48;
            this.label2.Text = "Output Ground Las Directory";
            // 
            // txtLasDir
            // 
            this.txtLasDir.Location = new System.Drawing.Point(12, 34);
            this.txtLasDir.Name = "txtLasDir";
            this.txtLasDir.Size = new System.Drawing.Size(215, 20);
            this.txtLasDir.TabIndex = 47;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 45;
            this.label1.Text = "LiDAR .las Directory";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(186, 124);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(74, 27);
            this.btnExecute.TabIndex = 54;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 56;
            this.label4.Text = "Cell Size";
            // 
            // nudCellSize
            // 
            this.nudCellSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudCellSize.Location = new System.Drawing.Point(12, 129);
            this.nudCellSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudCellSize.Name = "nudCellSize";
            this.nudCellSize.Size = new System.Drawing.Size(80, 20);
            this.nudCellSize.TabIndex = 55;
            this.nudCellSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnOutDir
            // 
            this.btnOutDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutDir.Location = new System.Drawing.Point(233, 78);
            this.btnOutDir.Name = "btnOutDir";
            this.btnOutDir.Size = new System.Drawing.Size(27, 28);
            this.btnOutDir.TabIndex = 49;
            this.btnOutDir.UseVisualStyleBackColor = true;
            this.btnOutDir.Click += new System.EventHandler(this.btnDtm_Click);
            // 
            // btnOpenLasDir
            // 
            this.btnOpenLasDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenLasDir.Location = new System.Drawing.Point(233, 30);
            this.btnOpenLasDir.Name = "btnOpenLasDir";
            this.btnOpenLasDir.Size = new System.Drawing.Size(27, 28);
            this.btnOpenLasDir.TabIndex = 46;
            this.btnOpenLasDir.UseVisualStyleBackColor = true;
            this.btnOpenLasDir.Click += new System.EventHandler(this.btnOpenLasDir_Click);
            // 
            // frmFilterLas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 167);
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
            this.Name = "frmFilterLas";
            this.Text = "Filter Las Files";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudCellSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.Button btnOutDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLasDir;
        private System.Windows.Forms.Button btnOpenLasDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudCellSize;
    }
}