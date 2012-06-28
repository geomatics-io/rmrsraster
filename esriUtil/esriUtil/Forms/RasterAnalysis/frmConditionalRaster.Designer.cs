namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmConditionalRaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConditionalRaster));
            this.btnExecute = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOutName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbInRaster2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbInRaster1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbInRaster3 = new System.Windows.Forms.ComboBox();
            this.btnInRaster3 = new System.Windows.Forms.Button();
            this.btnInRaster2 = new System.Windows.Forms.Button();
            this.btnInRaster1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(233, 179);
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
            this.label4.Location = new System.Drawing.Point(9, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Out Name";
            // 
            // txtOutName
            // 
            this.txtOutName.Location = new System.Drawing.Point(12, 182);
            this.txtOutName.Name = "txtOutName";
            this.txtOutName.Size = new System.Drawing.Size(210, 20);
            this.txtOutName.TabIndex = 32;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(166, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Condition True (1) Raster or value";
            // 
            // cmbInRaster2
            // 
            this.cmbInRaster2.FormattingEnabled = true;
            this.cmbInRaster2.Location = new System.Drawing.Point(12, 83);
            this.cmbInRaster2.Name = "cmbInRaster2";
            this.cmbInRaster2.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster2.TabIndex = 30;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Conditional Raster";
            // 
            // cmbInRaster1
            // 
            this.cmbInRaster1.FormattingEnabled = true;
            this.cmbInRaster1.Location = new System.Drawing.Point(12, 30);
            this.cmbInRaster1.Name = "cmbInRaster1";
            this.cmbInRaster1.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster1.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Condition False (0) Raster or value";
            // 
            // cmbInRaster3
            // 
            this.cmbInRaster3.FormattingEnabled = true;
            this.cmbInRaster3.Location = new System.Drawing.Point(12, 136);
            this.cmbInRaster3.Name = "cmbInRaster3";
            this.cmbInRaster3.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster3.TabIndex = 36;
            // 
            // btnInRaster3
            // 
            this.btnInRaster3.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnInRaster3.Location = new System.Drawing.Point(233, 134);
            this.btnInRaster3.Name = "btnInRaster3";
            this.btnInRaster3.Size = new System.Drawing.Size(27, 27);
            this.btnInRaster3.TabIndex = 35;
            this.btnInRaster3.UseVisualStyleBackColor = true;
            this.btnInRaster3.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // btnInRaster2
            // 
            this.btnInRaster2.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnInRaster2.Location = new System.Drawing.Point(233, 81);
            this.btnInRaster2.Name = "btnInRaster2";
            this.btnInRaster2.Size = new System.Drawing.Size(27, 27);
            this.btnInRaster2.TabIndex = 29;
            this.btnInRaster2.UseVisualStyleBackColor = true;
            this.btnInRaster2.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // btnInRaster1
            // 
            this.btnInRaster1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnInRaster1.Location = new System.Drawing.Point(233, 28);
            this.btnInRaster1.Name = "btnInRaster1";
            this.btnInRaster1.Size = new System.Drawing.Size(27, 27);
            this.btnInRaster1.TabIndex = 25;
            this.btnInRaster1.UseVisualStyleBackColor = true;
            this.btnInRaster1.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // frmConditionalRaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 224);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbInRaster3);
            this.Controls.Add(this.btnInRaster3);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOutName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbInRaster2);
            this.Controls.Add(this.btnInRaster2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbInRaster1);
            this.Controls.Add(this.btnInRaster1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmConditionalRaster";
            this.Text = "Conditional Analysis";
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbInRaster1;
        private System.Windows.Forms.Button btnInRaster1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbInRaster3;
        private System.Windows.Forms.Button btnInRaster3;
    }
}