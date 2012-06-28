namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmLogicalRaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogicalRaster));
            this.btnExecute = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOutName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbInRaster2 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbInRaster1 = new System.Windows.Forms.ComboBox();
            this.cmbProcess = new System.Windows.Forms.ComboBox();
            this.btnInRaster2 = new System.Windows.Forms.Button();
            this.btnInRaster1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(289, 171);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(54, 24);
            this.btnExecute.TabIndex = 34;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 177);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Out Name";
            // 
            // txtOutName
            // 
            this.txtOutName.Location = new System.Drawing.Point(73, 174);
            this.txtOutName.Name = "txtOutName";
            this.txtOutName.Size = new System.Drawing.Size(205, 20);
            this.txtOutName.TabIndex = 32;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(65, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Input Raster 2 or number";
            // 
            // cmbInRaster2
            // 
            this.cmbInRaster2.FormattingEnabled = true;
            this.cmbInRaster2.Location = new System.Drawing.Point(68, 123);
            this.cmbInRaster2.Name = "cmbInRaster2";
            this.cmbInRaster2.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster2.TabIndex = 30;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Operator";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(65, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Input Raster 1";
            // 
            // cmbInRaster1
            // 
            this.cmbInRaster1.FormattingEnabled = true;
            this.cmbInRaster1.Location = new System.Drawing.Point(68, 34);
            this.cmbInRaster1.Name = "cmbInRaster1";
            this.cmbInRaster1.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster1.TabIndex = 26;
            // 
            // cmbProcess
            // 
            this.cmbProcess.FormattingEnabled = true;
            this.cmbProcess.Location = new System.Drawing.Point(152, 72);
            this.cmbProcess.Name = "cmbProcess";
            this.cmbProcess.Size = new System.Drawing.Size(59, 21);
            this.cmbProcess.TabIndex = 24;
            // 
            // btnInRaster2
            // 
            this.btnInRaster2.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnInRaster2.Location = new System.Drawing.Point(289, 121);
            this.btnInRaster2.Name = "btnInRaster2";
            this.btnInRaster2.Size = new System.Drawing.Size(27, 27);
            this.btnInRaster2.TabIndex = 29;
            this.btnInRaster2.UseVisualStyleBackColor = true;
            this.btnInRaster2.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // btnInRaster1
            // 
            this.btnInRaster1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnInRaster1.Location = new System.Drawing.Point(289, 32);
            this.btnInRaster1.Name = "btnInRaster1";
            this.btnInRaster1.Size = new System.Drawing.Size(27, 27);
            this.btnInRaster1.TabIndex = 25;
            this.btnInRaster1.UseVisualStyleBackColor = true;
            this.btnInRaster1.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 145);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(349, 13);
            this.label5.TabIndex = 35;
            this.label5.Text = "_________________________________________________________";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // frmLogicalRaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 213);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOutName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbInRaster2);
            this.Controls.Add(this.btnInRaster2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbInRaster1);
            this.Controls.Add(this.btnInRaster1);
            this.Controls.Add(this.cmbProcess);
            this.Controls.Add(this.label5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLogicalRaster";
            this.Text = "Logical Analysis";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOutName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbInRaster2;
        private System.Windows.Forms.Button btnInRaster2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbInRaster1;
        private System.Windows.Forms.Button btnInRaster1;
        private System.Windows.Forms.ComboBox cmbProcess;
        private System.Windows.Forms.Label label5;
    }
}