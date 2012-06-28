namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmCreateMosaic
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCreateMosaic));
            this.label1 = new System.Windows.Forms.Label();
            this.txtOutNm = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.lsbRaster = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbInRaster1 = new System.Windows.Forms.ComboBox();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.btnWorkspace = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWorkspace = new System.Windows.Forms.TextBox();
            this.cmbMethod = new System.Windows.Forms.ComboBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.chbFootprint = new System.Windows.Forms.CheckBox();
            this.chbBoundary = new System.Windows.Forms.CheckBox();
            this.chbOverview = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chbSeamlines = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 274);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 45;
            this.label1.Text = "Out Mosaic Name";
            // 
            // txtOutNm
            // 
            this.txtOutNm.Location = new System.Drawing.Point(11, 292);
            this.txtOutNm.Name = "txtOutNm";
            this.txtOutNm.Size = new System.Drawing.Size(240, 20);
            this.txtOutNm.TabIndex = 44;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(197, 355);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(56, 23);
            this.btnExecute.TabIndex = 43;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(226, 84);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(27, 23);
            this.btnClear.TabIndex = 42;
            this.btnClear.Text = "!";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(226, 58);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(27, 23);
            this.btnMinus.TabIndex = 41;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // lsbRaster
            // 
            this.lsbRaster.FormattingEnabled = true;
            this.lsbRaster.Location = new System.Drawing.Point(10, 58);
            this.lsbRaster.Name = "lsbRaster";
            this.lsbRaster.Size = new System.Drawing.Size(211, 108);
            this.lsbRaster.TabIndex = 40;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 39;
            this.label2.Text = "Input Rasters To Mosaic";
            // 
            // cmbInRaster1
            // 
            this.cmbInRaster1.FormattingEnabled = true;
            this.cmbInRaster1.Location = new System.Drawing.Point(10, 31);
            this.cmbInRaster1.Name = "cmbInRaster1";
            this.cmbInRaster1.Size = new System.Drawing.Size(211, 21);
            this.cmbInRaster1.TabIndex = 38;
            this.cmbInRaster1.SelectedIndexChanged += new System.EventHandler(this.cmbInRaster1_SelectedIndexChanged);
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(226, 29);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(27, 27);
            this.btnOpenRaster.TabIndex = 37;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // btnWorkspace
            // 
            this.btnWorkspace.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnWorkspace.Location = new System.Drawing.Point(226, 240);
            this.btnWorkspace.Name = "btnWorkspace";
            this.btnWorkspace.Size = new System.Drawing.Size(27, 27);
            this.btnWorkspace.TabIndex = 48;
            this.btnWorkspace.UseVisualStyleBackColor = true;
            this.btnWorkspace.Click += new System.EventHandler(this.btnWorkspace_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 47;
            this.label3.Text = "Out Workspace";
            // 
            // txtWorkspace
            // 
            this.txtWorkspace.Location = new System.Drawing.Point(11, 244);
            this.txtWorkspace.Name = "txtWorkspace";
            this.txtWorkspace.Size = new System.Drawing.Size(211, 20);
            this.txtWorkspace.TabIndex = 46;
            // 
            // cmbMethod
            // 
            this.cmbMethod.FormattingEnabled = true;
            this.cmbMethod.Location = new System.Drawing.Point(11, 193);
            this.cmbMethod.Name = "cmbMethod";
            this.cmbMethod.Size = new System.Drawing.Size(115, 21);
            this.cmbMethod.TabIndex = 49;
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(136, 193);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(115, 21);
            this.cmbType.TabIndex = 51;
            // 
            // chbFootprint
            // 
            this.chbFootprint.AutoSize = true;
            this.chbFootprint.Checked = true;
            this.chbFootprint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbFootprint.Location = new System.Drawing.Point(14, 331);
            this.chbFootprint.Name = "chbFootprint";
            this.chbFootprint.Size = new System.Drawing.Size(67, 17);
            this.chbFootprint.TabIndex = 53;
            this.chbFootprint.Text = "Footprint";
            this.chbFootprint.UseVisualStyleBackColor = true;
            // 
            // chbBoundary
            // 
            this.chbBoundary.AutoSize = true;
            this.chbBoundary.Checked = true;
            this.chbBoundary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbBoundary.Location = new System.Drawing.Point(101, 331);
            this.chbBoundary.Name = "chbBoundary";
            this.chbBoundary.Size = new System.Drawing.Size(71, 17);
            this.chbBoundary.TabIndex = 57;
            this.chbBoundary.Text = "Boundary";
            this.chbBoundary.UseVisualStyleBackColor = true;
            // 
            // chbOverview
            // 
            this.chbOverview.AutoSize = true;
            this.chbOverview.Checked = true;
            this.chbOverview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbOverview.Location = new System.Drawing.Point(180, 331);
            this.chbOverview.Name = "chbOverview";
            this.chbOverview.Size = new System.Drawing.Size(71, 17);
            this.chbOverview.TabIndex = 58;
            this.chbOverview.Text = "Overview";
            this.chbOverview.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 50;
            this.label4.Text = "Method";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(141, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 52;
            this.label5.Text = "Type";
            // 
            // chbSeamlines
            // 
            this.chbSeamlines.AutoSize = true;
            this.chbSeamlines.Checked = true;
            this.chbSeamlines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSeamlines.Location = new System.Drawing.Point(14, 355);
            this.chbSeamlines.Name = "chbSeamlines";
            this.chbSeamlines.Size = new System.Drawing.Size(74, 17);
            this.chbSeamlines.TabIndex = 59;
            this.chbSeamlines.Text = "Seamlines";
            this.chbSeamlines.UseVisualStyleBackColor = true;
            // 
            // frmCreateMosaic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 385);
            this.Controls.Add(this.chbSeamlines);
            this.Controls.Add(this.chbOverview);
            this.Controls.Add(this.chbBoundary);
            this.Controls.Add(this.chbFootprint);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbMethod);
            this.Controls.Add(this.btnWorkspace);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtWorkspace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutNm);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.lsbRaster);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbInRaster1);
            this.Controls.Add(this.btnOpenRaster);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCreateMosaic";
            this.Text = "Create Mosaic";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOutNm;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.ListBox lsbRaster;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbInRaster1;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.Button btnWorkspace;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWorkspace;
        private System.Windows.Forms.ComboBox cmbMethod;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.CheckBox chbFootprint;
        private System.Windows.Forms.CheckBox chbBoundary;
        private System.Windows.Forms.CheckBox chbOverview;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chbSeamlines;
    }
}