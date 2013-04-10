namespace esriUtil.Forms.Stats
{
    partial class frmViewModelSampleSize
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViewModelSampleSize));
            this.btnExcute = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.nudProp = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nudAlpha = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudProp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExcute
            // 
            this.btnExcute.Location = new System.Drawing.Point(200, 80);
            this.btnExcute.Name = "btnExcute";
            this.btnExcute.Size = new System.Drawing.Size(61, 23);
            this.btnExcute.TabIndex = 66;
            this.btnExcute.Text = "Execute";
            this.btnExcute.UseVisualStyleBackColor = true;
            this.btnExcute.Click += new System.EventHandler(this.btnExcute_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 65;
            this.label3.Text = "Width (%Mean)";
            // 
            // nudProp
            // 
            this.nudProp.DecimalPlaces = 2;
            this.nudProp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudProp.Location = new System.Drawing.Point(15, 83);
            this.nudProp.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudProp.Name = "nudProp";
            this.nudProp.Size = new System.Drawing.Size(47, 20);
            this.nudProp.TabIndex = 64;
            this.nudProp.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 63;
            this.label2.Text = "Alpha";
            // 
            // nudAlpha
            // 
            this.nudAlpha.DecimalPlaces = 2;
            this.nudAlpha.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudAlpha.Location = new System.Drawing.Point(107, 83);
            this.nudAlpha.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAlpha.Name = "nudAlpha";
            this.nudAlpha.Size = new System.Drawing.Size(47, 20);
            this.nudAlpha.TabIndex = 62;
            this.nudAlpha.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 61;
            this.label1.Text = "Model Path";
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(15, 26);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(220, 20);
            this.txtOutputPath.TabIndex = 60;
            // 
            // button1
            // 
            this.button1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button1.Location = new System.Drawing.Point(238, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 26);
            this.button1.TabIndex = 59;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmViewModelSampleSize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 117);
            this.Controls.Add(this.btnExcute);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudProp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudAlpha);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmViewModelSampleSize";
            this.Text = "Sample Size of Model";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudProp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExcute;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudProp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudAlpha;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button button1;
    }
}