namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmSetupToolbar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetupToolbar));
            this.chbAutoUpdate = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chbAutoUpdate
            // 
            this.chbAutoUpdate.AutoSize = true;
            this.chbAutoUpdate.Checked = true;
            this.chbAutoUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAutoUpdate.Location = new System.Drawing.Point(12, 21);
            this.chbAutoUpdate.Name = "chbAutoUpdate";
            this.chbAutoUpdate.Size = new System.Drawing.Size(83, 17);
            this.chbAutoUpdate.TabIndex = 0;
            this.chbAutoUpdate.Text = "AutoUpdate";
            this.chbAutoUpdate.UseVisualStyleBackColor = true;
            this.chbAutoUpdate.Click += new System.EventHandler(this.chbAutoUpdate_Click);
            // 
            // frmSetupToolbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 59);
            this.Controls.Add(this.chbAutoUpdate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSetupToolbar";
            this.Text = "Setup Toolbar";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chbAutoUpdate;
    }
}