namespace esriUtil.Forms.Stats
{
    partial class frmRasterFromModel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRasterFromModel));
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOutNm = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.lsbRaster = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbInRaster1 = new System.Windows.Forms.ComboBox();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.btnVariables = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(12, 29);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(213, 20);
            this.txtOutputPath.TabIndex = 40;
            this.txtOutputPath.TextChanged += new System.EventHandler(this.txtOutputPath_Validated);
            // 
            // button1
            // 
            this.button1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button1.Location = new System.Drawing.Point(231, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 26);
            this.button1.TabIndex = 38;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Model Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 235);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 50;
            this.label2.Text = "Out Raster Name";
            // 
            // txtOutNm
            // 
            this.txtOutNm.Location = new System.Drawing.Point(15, 252);
            this.txtOutNm.Name = "txtOutNm";
            this.txtOutNm.Size = new System.Drawing.Size(181, 20);
            this.txtOutNm.TabIndex = 49;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(202, 250);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(56, 23);
            this.btnExecute.TabIndex = 48;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(231, 145);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(27, 23);
            this.btnClear.TabIndex = 47;
            this.btnClear.Text = "!";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(231, 119);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(27, 23);
            this.btnMinus.TabIndex = 46;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // lsbRaster
            // 
            this.lsbRaster.FormattingEnabled = true;
            this.lsbRaster.Location = new System.Drawing.Point(15, 119);
            this.lsbRaster.Name = "lsbRaster";
            this.lsbRaster.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbRaster.Size = new System.Drawing.Size(211, 108);
            this.lsbRaster.TabIndex = 45;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Indipendent Rasters";
            // 
            // cmbInRaster1
            // 
            this.cmbInRaster1.FormattingEnabled = true;
            this.cmbInRaster1.Location = new System.Drawing.Point(15, 92);
            this.cmbInRaster1.Name = "cmbInRaster1";
            this.cmbInRaster1.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster1.TabIndex = 43;
            this.cmbInRaster1.SelectedIndexChanged += new System.EventHandler(this.cmbInRaster1_SelectedIndexChanged);
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(231, 90);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(27, 27);
            this.btnOpenRaster.TabIndex = 42;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // btnVariables
            // 
            this.btnVariables.Enabled = false;
            this.btnVariables.Location = new System.Drawing.Point(197, 62);
            this.btnVariables.Name = "btnVariables";
            this.btnVariables.Size = new System.Drawing.Size(61, 22);
            this.btnVariables.TabIndex = 51;
            this.btnVariables.Text = "Variables";
            this.btnVariables.UseVisualStyleBackColor = true;
            this.btnVariables.Click += new System.EventHandler(this.btnVariables_Click);
            // 
            // frmRasterFromModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 299);
            this.Controls.Add(this.btnVariables);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOutNm);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.lsbRaster);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbInRaster1);
            this.Controls.Add(this.btnOpenRaster);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRasterFromModel";
            this.Text = "Create Raster From Model";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOutNm;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.ListBox lsbRaster;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbInRaster1;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.Button btnVariables;
    }
}