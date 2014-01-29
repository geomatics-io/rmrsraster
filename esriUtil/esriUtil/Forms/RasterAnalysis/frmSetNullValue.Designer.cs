namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmSetNullValue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetNullValue));
            this.label2 = new System.Windows.Forms.Label();
            this.cmbInRaster1 = new System.Windows.Forms.ComboBox();
            this.btnInRaster1 = new System.Windows.Forms.Button();
            this.txtNodata = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOutName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Raster";
            // 
            // cmbInRaster1
            // 
            this.cmbInRaster1.FormattingEnabled = true;
            this.cmbInRaster1.Location = new System.Drawing.Point(12, 27);
            this.cmbInRaster1.Name = "cmbInRaster1";
            this.cmbInRaster1.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster1.TabIndex = 28;
            // 
            // btnInRaster1
            // 
            this.btnInRaster1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnInRaster1.Location = new System.Drawing.Point(233, 25);
            this.btnInRaster1.Name = "btnInRaster1";
            this.btnInRaster1.Size = new System.Drawing.Size(27, 27);
            this.btnInRaster1.TabIndex = 27;
            this.btnInRaster1.UseVisualStyleBackColor = true;
            this.btnInRaster1.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // txtNodata
            // 
            this.txtNodata.Location = new System.Drawing.Point(12, 71);
            this.txtNodata.Name = "txtNodata";
            this.txtNodata.Size = new System.Drawing.Size(71, 20);
            this.txtNodata.TabIndex = 30;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "NoData Value";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(206, 111);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(54, 24);
            this.btnExecute.TabIndex = 33;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Out Name";
            // 
            // txtOutName
            // 
            this.txtOutName.Location = new System.Drawing.Point(12, 114);
            this.txtOutName.Name = "txtOutName";
            this.txtOutName.Size = new System.Drawing.Size(188, 20);
            this.txtOutName.TabIndex = 34;
            // 
            // frmSetNullValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 151);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOutName);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtNodata);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbInRaster1);
            this.Controls.Add(this.btnInRaster1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSetNullValue";
            this.Text = "Set Null Value";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbInRaster1;
        private System.Windows.Forms.Button btnInRaster1;
        private System.Windows.Forms.TextBox txtNodata;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOutName;
    }
}