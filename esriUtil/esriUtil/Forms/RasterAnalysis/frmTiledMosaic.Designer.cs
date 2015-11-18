namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmTiledMosaic
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTiledMosaic));
            this.cmbFtrCls = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFtrCls = new System.Windows.Forms.Button();
            this.btnImg = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtImgWks = new System.Windows.Forms.TextBox();
            this.txtOutWks = new System.Windows.Forms.TextBox();
            this.btnOut = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbFtrCls
            // 
            this.cmbFtrCls.FormattingEnabled = true;
            this.cmbFtrCls.Location = new System.Drawing.Point(29, 30);
            this.cmbFtrCls.Name = "cmbFtrCls";
            this.cmbFtrCls.Size = new System.Drawing.Size(234, 21);
            this.cmbFtrCls.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Feature Class";
            // 
            // btnFtrCls
            // 
            this.btnFtrCls.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnFtrCls.Location = new System.Drawing.Point(269, 26);
            this.btnFtrCls.Name = "btnFtrCls";
            this.btnFtrCls.Size = new System.Drawing.Size(30, 26);
            this.btnFtrCls.TabIndex = 2;
            this.btnFtrCls.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFtrCls.UseVisualStyleBackColor = true;
            this.btnFtrCls.Click += new System.EventHandler(this.btnFtrCls_Click);
            // 
            // btnImg
            // 
            this.btnImg.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnImg.Location = new System.Drawing.Point(269, 76);
            this.btnImg.Name = "btnImg";
            this.btnImg.Size = new System.Drawing.Size(30, 26);
            this.btnImg.TabIndex = 5;
            this.btnImg.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImg.UseVisualStyleBackColor = true;
            this.btnImg.Click += new System.EventHandler(this.btnImg_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Image Directory\\Workspace";
            // 
            // txtImgWks
            // 
            this.txtImgWks.Location = new System.Drawing.Point(29, 80);
            this.txtImgWks.Name = "txtImgWks";
            this.txtImgWks.Size = new System.Drawing.Size(234, 20);
            this.txtImgWks.TabIndex = 6;
            // 
            // txtOutWks
            // 
            this.txtOutWks.Location = new System.Drawing.Point(29, 130);
            this.txtOutWks.Name = "txtOutWks";
            this.txtOutWks.Size = new System.Drawing.Size(234, 20);
            this.txtOutWks.TabIndex = 9;
            // 
            // btnOut
            // 
            this.btnOut.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOut.Location = new System.Drawing.Point(269, 126);
            this.btnOut.Name = "btnOut";
            this.btnOut.Size = new System.Drawing.Size(30, 26);
            this.btnOut.TabIndex = 8;
            this.btnOut.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOut.UseVisualStyleBackColor = true;
            this.btnOut.Click += new System.EventHandler(this.btnOut_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Output Workspace";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(243, 164);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(56, 23);
            this.btnExecute.TabIndex = 10;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // frmTiledMosaic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 194);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtOutWks);
            this.Controls.Add(this.btnOut);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtImgWks);
            this.Controls.Add(this.btnImg);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnFtrCls);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbFtrCls);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTiledMosaic";
            this.Text = "Tiled Mosaic";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbFtrCls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFtrCls;
        private System.Windows.Forms.Button btnImg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtImgWks;
        private System.Windows.Forms.TextBox txtOutWks;
        private System.Windows.Forms.Button btnOut;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnExecute;
    }
}