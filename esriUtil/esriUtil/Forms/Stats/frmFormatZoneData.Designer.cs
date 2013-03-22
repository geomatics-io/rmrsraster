namespace esriUtil.Forms.Stats
{
    partial class frmFormatZonalData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFormatZonalData));
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbdataset = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.cmbField = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(209, 172);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(55, 23);
            this.btnExecute.TabIndex = 52;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(242, 134);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(23, 24);
            this.btnOpenFeatureClass.TabIndex = 51;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 50;
            this.label2.Text = "Zonal Summary Table";
            // 
            // cmbdataset
            // 
            this.cmbdataset.FormattingEnabled = true;
            this.cmbdataset.Location = new System.Drawing.Point(16, 32);
            this.cmbdataset.Name = "cmbdataset";
            this.cmbdataset.Size = new System.Drawing.Size(220, 21);
            this.cmbdataset.TabIndex = 53;
            this.cmbdataset.SelectedIndexChanged += new System.EventHandler(this.cmbdataset_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 54;
            this.label1.Text = "Zone Dataset Table";
            // 
            // button2
            // 
            this.button2.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button2.Location = new System.Drawing.Point(242, 30);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(23, 24);
            this.button2.TabIndex = 55;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(16, 137);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(220, 20);
            this.txtOutputPath.TabIndex = 56;
            // 
            // cmbField
            // 
            this.cmbField.FormattingEnabled = true;
            this.cmbField.Location = new System.Drawing.Point(16, 84);
            this.cmbField.Name = "cmbField";
            this.cmbField.Size = new System.Drawing.Size(220, 21);
            this.cmbField.TabIndex = 57;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 58;
            this.label3.Text = "Link Field";
            // 
            // frmFormatZonalData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 209);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbField);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbdataset);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFormatZonalData";
            this.Text = "Format Zonal Data";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbdataset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.ComboBox cmbField;
        private System.Windows.Forms.Label label3;
    }
}