namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmPhotoAnalysis
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPhotoAnalysis));
            this.btnDir = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnROI = new System.Windows.Forms.Button();
            this.cmbROI = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOut = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.txtInDir = new System.Windows.Forms.TextBox();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.cmbExt = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnDir
            // 
            this.btnDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnDir.Location = new System.Drawing.Point(322, 27);
            this.btnDir.Name = "btnDir";
            this.btnDir.Size = new System.Drawing.Size(26, 25);
            this.btnDir.TabIndex = 14;
            this.btnDir.UseVisualStyleBackColor = true;
            this.btnDir.Click += new System.EventHandler(this.btnDir_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Image Directory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "ROI layer";
            // 
            // btnROI
            // 
            this.btnROI.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnROI.Location = new System.Drawing.Point(204, 79);
            this.btnROI.Name = "btnROI";
            this.btnROI.Size = new System.Drawing.Size(26, 25);
            this.btnROI.TabIndex = 17;
            this.btnROI.UseVisualStyleBackColor = true;
            this.btnROI.Click += new System.EventHandler(this.btnROI_Click);
            // 
            // cmbROI
            // 
            this.cmbROI.FormattingEnabled = true;
            this.cmbROI.Location = new System.Drawing.Point(14, 79);
            this.cmbROI.Name = "cmbROI";
            this.cmbROI.Size = new System.Drawing.Size(184, 21);
            this.cmbROI.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Output CSV file";
            // 
            // btnOut
            // 
            this.btnOut.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOut.Location = new System.Drawing.Point(322, 129);
            this.btnOut.Name = "btnOut";
            this.btnOut.Size = new System.Drawing.Size(26, 25);
            this.btnOut.TabIndex = 20;
            this.btnOut.UseVisualStyleBackColor = true;
            this.btnOut.Click += new System.EventHandler(this.btnOut_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(273, 160);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 22;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // txtInDir
            // 
            this.txtInDir.Location = new System.Drawing.Point(17, 27);
            this.txtInDir.Name = "txtInDir";
            this.txtInDir.Size = new System.Drawing.Size(299, 20);
            this.txtInDir.TabIndex = 23;
            // 
            // txtOutDir
            // 
            this.txtOutDir.Location = new System.Drawing.Point(14, 132);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(299, 20);
            this.txtOutDir.TabIndex = 24;
            // 
            // cmbExt
            // 
            this.cmbExt.FormattingEnabled = true;
            this.cmbExt.Location = new System.Drawing.Point(251, 82);
            this.cmbExt.Name = "cmbExt";
            this.cmbExt.Size = new System.Drawing.Size(97, 21);
            this.cmbExt.TabIndex = 25;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(248, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Image Extension";
            // 
            // frmPhotoAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 192);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbExt);
            this.Controls.Add(this.txtOutDir);
            this.Controls.Add(this.txtInDir);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnOut);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnROI);
            this.Controls.Add(this.cmbROI);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPhotoAnalysis";
            this.Text = "Photo Analysis";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnROI;
        private System.Windows.Forms.ComboBox cmbROI;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOut;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox txtInDir;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.ComboBox cmbExt;
        private System.Windows.Forms.Label label4;
    }
}