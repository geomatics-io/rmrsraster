namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmSplitPoly
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSplitPoly));
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtOutFtrCls = new System.Windows.Forms.TextBox();
            this.btnOutFtrCls = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnInFtrCls = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbInFtrCls = new System.Windows.Forms.ComboBox();
            this.nudSplits = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudSplits)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(226, 160);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(56, 23);
            this.btnExecute.TabIndex = 20;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txtOutFtrCls
            // 
            this.txtOutFtrCls.Location = new System.Drawing.Point(12, 126);
            this.txtOutFtrCls.Name = "txtOutFtrCls";
            this.txtOutFtrCls.Size = new System.Drawing.Size(234, 20);
            this.txtOutFtrCls.TabIndex = 19;
            // 
            // btnOutFtrCls
            // 
            this.btnOutFtrCls.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutFtrCls.Location = new System.Drawing.Point(252, 122);
            this.btnOutFtrCls.Name = "btnOutFtrCls";
            this.btnOutFtrCls.Size = new System.Drawing.Size(30, 26);
            this.btnOutFtrCls.TabIndex = 18;
            this.btnOutFtrCls.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOutFtrCls.UseVisualStyleBackColor = true;
            this.btnOutFtrCls.Click += new System.EventHandler(this.btnOutFtrCls_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Output Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Number of Splits";
            // 
            // btnInFtrCls
            // 
            this.btnInFtrCls.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnInFtrCls.Location = new System.Drawing.Point(252, 22);
            this.btnInFtrCls.Name = "btnInFtrCls";
            this.btnInFtrCls.Size = new System.Drawing.Size(30, 26);
            this.btnInFtrCls.TabIndex = 13;
            this.btnInFtrCls.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnInFtrCls.UseVisualStyleBackColor = true;
            this.btnInFtrCls.Click += new System.EventHandler(this.btnInFtrCls_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Feature Class";
            // 
            // cmbInFtrCls
            // 
            this.cmbInFtrCls.FormattingEnabled = true;
            this.cmbInFtrCls.Location = new System.Drawing.Point(12, 26);
            this.cmbInFtrCls.Name = "cmbInFtrCls";
            this.cmbInFtrCls.Size = new System.Drawing.Size(234, 21);
            this.cmbInFtrCls.TabIndex = 11;
            // 
            // nudSplits
            // 
            this.nudSplits.Location = new System.Drawing.Point(15, 76);
            this.nudSplits.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudSplits.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSplits.Name = "nudSplits";
            this.nudSplits.Size = new System.Drawing.Size(231, 20);
            this.nudSplits.TabIndex = 21;
            this.nudSplits.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // frmSplitPoly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 196);
            this.Controls.Add(this.nudSplits);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtOutFtrCls);
            this.Controls.Add(this.btnOutFtrCls);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnInFtrCls);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbInFtrCls);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSplitPoly";
            this.Text = "Split Poly Feature Class";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudSplits)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtOutFtrCls;
        private System.Windows.Forms.Button btnOutFtrCls;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnInFtrCls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbInFtrCls;
        private System.Windows.Forms.NumericUpDown nudSplits;
    }
}