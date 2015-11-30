namespace esriUtil.Forms.MapServices
{
    partial class frmDownloadMapServiceLayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDownloadMapServiceLayer));
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtOutFtrCls = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbService = new System.Windows.Forms.ComboBox();
            this.btnOutFtrCls = new System.Windows.Forms.Button();
            this.cmbLayer = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chbDisplayExtent = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(216, 156);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(56, 23);
            this.btnExecute.TabIndex = 29;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txtOutFtrCls
            // 
            this.txtOutFtrCls.Location = new System.Drawing.Point(2, 124);
            this.txtOutFtrCls.Name = "txtOutFtrCls";
            this.txtOutFtrCls.Size = new System.Drawing.Size(234, 20);
            this.txtOutFtrCls.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Output Path";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Map Service";
            // 
            // cmbService
            // 
            this.cmbService.FormattingEnabled = true;
            this.cmbService.Location = new System.Drawing.Point(2, 28);
            this.cmbService.Name = "cmbService";
            this.cmbService.Size = new System.Drawing.Size(234, 21);
            this.cmbService.TabIndex = 22;
            this.cmbService.SelectedIndexChanged += new System.EventHandler(this.cmbService_SelectedIndexChanged);
            // 
            // btnOutFtrCls
            // 
            this.btnOutFtrCls.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutFtrCls.Location = new System.Drawing.Point(242, 120);
            this.btnOutFtrCls.Name = "btnOutFtrCls";
            this.btnOutFtrCls.Size = new System.Drawing.Size(30, 26);
            this.btnOutFtrCls.TabIndex = 27;
            this.btnOutFtrCls.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOutFtrCls.UseVisualStyleBackColor = true;
            this.btnOutFtrCls.Click += new System.EventHandler(this.btnOutFtrCls_Click);
            // 
            // cmbLayer
            // 
            this.cmbLayer.FormattingEnabled = true;
            this.cmbLayer.Location = new System.Drawing.Point(2, 78);
            this.cmbLayer.Name = "cmbLayer";
            this.cmbLayer.Size = new System.Drawing.Size(234, 21);
            this.cmbLayer.TabIndex = 30;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Layer";
            // 
            // chbDisplayExtent
            // 
            this.chbDisplayExtent.AutoSize = true;
            this.chbDisplayExtent.Location = new System.Drawing.Point(5, 156);
            this.chbDisplayExtent.Name = "chbDisplayExtent";
            this.chbDisplayExtent.Size = new System.Drawing.Size(92, 17);
            this.chbDisplayExtent.TabIndex = 32;
            this.chbDisplayExtent.Text = "Clip to Display";
            this.chbDisplayExtent.UseVisualStyleBackColor = true;
            // 
            // frmDownloadMapServiceLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 194);
            this.Controls.Add(this.chbDisplayExtent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbLayer);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtOutFtrCls);
            this.Controls.Add(this.btnOutFtrCls);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbService);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDownloadMapServiceLayer";
            this.Text = "Download Map Service Layer";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtOutFtrCls;
        private System.Windows.Forms.Button btnOutFtrCls;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbService;
        private System.Windows.Forms.ComboBox cmbLayer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chbDisplayExtent;
    }
}