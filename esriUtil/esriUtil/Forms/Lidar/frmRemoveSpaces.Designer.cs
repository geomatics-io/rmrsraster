namespace esriUtil.Forms.Lidar
{
    partial class frmRemoveSpaces
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRemoveSpaces));
            this.txtLasDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpenLasDir = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLasDir
            // 
            this.txtLasDir.Location = new System.Drawing.Point(12, 35);
            this.txtLasDir.Name = "txtLasDir";
            this.txtLasDir.Size = new System.Drawing.Size(215, 20);
            this.txtLasDir.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "LiDAR .las Directory";
            // 
            // btnOpenLasDir
            // 
            this.btnOpenLasDir.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenLasDir.Location = new System.Drawing.Point(233, 31);
            this.btnOpenLasDir.Name = "btnOpenLasDir";
            this.btnOpenLasDir.Size = new System.Drawing.Size(27, 28);
            this.btnOpenLasDir.TabIndex = 14;
            this.btnOpenLasDir.UseVisualStyleBackColor = true;
            this.btnOpenLasDir.Click += new System.EventHandler(this.btnOpenLasDir_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(186, 68);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(74, 27);
            this.btnExecute.TabIndex = 16;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // frmRemoveSpaces
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 104);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtLasDir);
            this.Controls.Add(this.btnOpenLasDir);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRemoveSpaces";
            this.Text = "Clean Directories and Files";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLasDir;
        private System.Windows.Forms.Button btnOpenLasDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExecute;
    }
}