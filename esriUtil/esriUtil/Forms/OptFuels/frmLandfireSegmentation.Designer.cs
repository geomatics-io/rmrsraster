namespace esriUtil.Forms.OptFuels
{
    partial class frmLandfireSegmentation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLandfireSegmentation));
            this.trbCover = new System.Windows.Forms.TrackBar();
            this.trbHeight = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSegment = new System.Windows.Forms.Button();
            this.txtLandFireDir = new System.Windows.Forms.TextBox();
            this.btnOpenWorkspace = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.nudMin = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudMax = new System.Windows.Forms.NumericUpDown();
            this.chbAspect = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trbCover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMax)).BeginInit();
            this.SuspendLayout();
            // 
            // trbCover
            // 
            this.trbCover.Location = new System.Drawing.Point(13, 29);
            this.trbCover.Name = "trbCover";
            this.trbCover.Size = new System.Drawing.Size(234, 45);
            this.trbCover.TabIndex = 1;
            // 
            // trbHeight
            // 
            this.trbHeight.Location = new System.Drawing.Point(12, 98);
            this.trbHeight.Name = "trbHeight";
            this.trbHeight.Size = new System.Drawing.Size(234, 45);
            this.trbHeight.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(93, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Canopy Cover";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(90, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Canopy Height";
            // 
            // btnSegment
            // 
            this.btnSegment.Location = new System.Drawing.Point(172, 251);
            this.btnSegment.Name = "btnSegment";
            this.btnSegment.Size = new System.Drawing.Size(75, 31);
            this.btnSegment.TabIndex = 8;
            this.btnSegment.Text = "Segement";
            this.btnSegment.UseVisualStyleBackColor = true;
            this.btnSegment.Click += new System.EventHandler(this.btnSegment_Click);
            // 
            // txtLandFireDir
            // 
            this.txtLandFireDir.Location = new System.Drawing.Point(13, 217);
            this.txtLandFireDir.Name = "txtLandFireDir";
            this.txtLandFireDir.Size = new System.Drawing.Size(200, 20);
            this.txtLandFireDir.TabIndex = 9;
            // 
            // btnOpenWorkspace
            // 
            this.btnOpenWorkspace.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenWorkspace.Location = new System.Drawing.Point(217, 211);
            this.btnOpenWorkspace.Name = "btnOpenWorkspace";
            this.btnOpenWorkspace.Size = new System.Drawing.Size(29, 31);
            this.btnOpenWorkspace.TabIndex = 10;
            this.btnOpenWorkspace.UseVisualStyleBackColor = true;
            this.btnOpenWorkspace.Click += new System.EventHandler(this.btnOpenWorkspace_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 198);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "LandFire Directory";
            // 
            // nudMin
            // 
            this.nudMin.Location = new System.Drawing.Point(16, 168);
            this.nudMin.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nudMin.Name = "nudMin";
            this.nudMin.Size = new System.Drawing.Size(71, 20);
            this.nudMin.TabIndex = 12;
            this.nudMin.ThousandsSeparator = true;
            this.nudMin.Value = new decimal(new int[] {
            20323,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 149);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Minimum Area";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(142, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Maximum Area";
            // 
            // nudMax
            // 
            this.nudMax.Location = new System.Drawing.Point(142, 168);
            this.nudMax.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.nudMax.Name = "nudMax";
            this.nudMax.Size = new System.Drawing.Size(71, 20);
            this.nudMax.TabIndex = 14;
            this.nudMax.ThousandsSeparator = true;
            this.nudMax.Value = new decimal(new int[] {
            121398,
            0,
            0,
            0});
            // 
            // chbAspect
            // 
            this.chbAspect.AutoSize = true;
            this.chbAspect.Checked = true;
            this.chbAspect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAspect.Location = new System.Drawing.Point(13, 259);
            this.chbAspect.Name = "chbAspect";
            this.chbAspect.Size = new System.Drawing.Size(81, 17);
            this.chbAspect.TabIndex = 16;
            this.chbAspect.Text = "Use Aspect";
            this.chbAspect.UseVisualStyleBackColor = true;
            // 
            // frmLandfireSegmentation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 289);
            this.Controls.Add(this.chbAspect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudMax);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudMin);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOpenWorkspace);
            this.Controls.Add(this.txtLandFireDir);
            this.Controls.Add(this.btnSegment);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.trbHeight);
            this.Controls.Add(this.trbCover);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLandfireSegmentation";
            this.Text = "Landfire Segmentation";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.trbCover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trbCover;
        private System.Windows.Forms.TrackBar trbHeight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSegment;
        private System.Windows.Forms.TextBox txtLandFireDir;
        private System.Windows.Forms.Button btnOpenWorkspace;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudMin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudMax;
        private System.Windows.Forms.CheckBox chbAspect;
    }
}