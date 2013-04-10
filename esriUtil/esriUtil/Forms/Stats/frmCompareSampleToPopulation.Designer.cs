namespace esriUtil.Forms.Stats
{
    partial class frmCompareSampleToPopulation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCompareSampleToPopulation));
            this.btnExcute = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSamp = new System.Windows.Forms.TextBox();
            this.btnSamp = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPop = new System.Windows.Forms.TextBox();
            this.btnPop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExcute
            // 
            this.btnExcute.Location = new System.Drawing.Point(211, 105);
            this.btnExcute.Name = "btnExcute";
            this.btnExcute.Size = new System.Drawing.Size(61, 23);
            this.btnExcute.TabIndex = 70;
            this.btnExcute.Text = "Execute";
            this.btnExcute.UseVisualStyleBackColor = true;
            this.btnExcute.Click += new System.EventHandler(this.btnExcute_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 13);
            this.label1.TabIndex = 69;
            this.label1.Text = "Sample Strata/Cluster Model";
            // 
            // txtSamp
            // 
            this.txtSamp.Location = new System.Drawing.Point(23, 77);
            this.txtSamp.Name = "txtSamp";
            this.txtSamp.Size = new System.Drawing.Size(220, 20);
            this.txtSamp.TabIndex = 68;
            // 
            // btnSamp
            // 
            this.btnSamp.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnSamp.Location = new System.Drawing.Point(246, 73);
            this.btnSamp.Name = "btnSamp";
            this.btnSamp.Size = new System.Drawing.Size(26, 26);
            this.btnSamp.TabIndex = 67;
            this.btnSamp.UseVisualStyleBackColor = true;
            this.btnSamp.Click += new System.EventHandler(this.btnSamp_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(156, 13);
            this.label2.TabIndex = 73;
            this.label2.Text = "Population Strata/Cluster model";
            // 
            // txtPop
            // 
            this.txtPop.Location = new System.Drawing.Point(23, 28);
            this.txtPop.Name = "txtPop";
            this.txtPop.Size = new System.Drawing.Size(220, 20);
            this.txtPop.TabIndex = 72;
            // 
            // btnPop
            // 
            this.btnPop.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnPop.Location = new System.Drawing.Point(246, 24);
            this.btnPop.Name = "btnPop";
            this.btnPop.Size = new System.Drawing.Size(26, 26);
            this.btnPop.TabIndex = 71;
            this.btnPop.UseVisualStyleBackColor = true;
            this.btnPop.Click += new System.EventHandler(this.btnPop_Click);
            // 
            // frmCompareSampleToPopulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 145);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPop);
            this.Controls.Add(this.btnPop);
            this.Controls.Add(this.btnExcute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSamp);
            this.Controls.Add(this.btnSamp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCompareSampleToPopulation";
            this.Text = "Compare Sample To Population";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExcute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSamp;
        private System.Windows.Forms.Button btnSamp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPop;
        private System.Windows.Forms.Button btnPop;
    }
}