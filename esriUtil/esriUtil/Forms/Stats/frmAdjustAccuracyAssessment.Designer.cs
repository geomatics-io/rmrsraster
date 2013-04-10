namespace esriUtil.Forms.Stats
{
    partial class frmAdjustAccuracyAssessment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAdjustAccuracyAssessment));
            this.txtOutModel = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.nudAlpha = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbMap = new System.Windows.Forms.ComboBox();
            this.txtOAA = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.btnProjectBoundary = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbProjectBoundary = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // txtOutModel
            // 
            this.txtOutModel.Location = new System.Drawing.Point(19, 170);
            this.txtOutModel.Name = "txtOutModel";
            this.txtOutModel.Size = new System.Drawing.Size(217, 20);
            this.txtOutModel.TabIndex = 59;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 210);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 58;
            this.label5.Text = "Alpha";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudAlpha
            // 
            this.nudAlpha.DecimalPlaces = 2;
            this.nudAlpha.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudAlpha.Location = new System.Drawing.Point(61, 207);
            this.nudAlpha.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAlpha.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudAlpha.Name = "nudAlpha";
            this.nudAlpha.Size = new System.Drawing.Size(49, 20);
            this.nudAlpha.TabIndex = 57;
            this.nudAlpha.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(193, 13);
            this.label4.TabIndex = 56;
            this.label4.Text = "Ouput New Accuracy Assessment Path";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(193, 206);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 54;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 52;
            this.label2.Text = "Map";
            // 
            // cmbMap
            // 
            this.cmbMap.FormattingEnabled = true;
            this.cmbMap.Location = new System.Drawing.Point(19, 71);
            this.cmbMap.Name = "cmbMap";
            this.cmbMap.Size = new System.Drawing.Size(220, 21);
            this.cmbMap.TabIndex = 51;
            // 
            // txtOAA
            // 
            this.txtOAA.Location = new System.Drawing.Point(19, 119);
            this.txtOAA.Name = "txtOAA";
            this.txtOAA.Size = new System.Drawing.Size(217, 20);
            this.txtOAA.TabIndex = 62;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 13);
            this.label1.TabIndex = 61;
            this.label1.Text = "Original Accuracy Assessment Path";
            // 
            // button1
            // 
            this.button1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button1.Location = new System.Drawing.Point(242, 115);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 25);
            this.button1.TabIndex = 60;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnOpenModel_Click);
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(242, 166);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(26, 25);
            this.btnOpenRaster.TabIndex = 55;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnSaveModel_Click);
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(245, 69);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(23, 24);
            this.btnOpenFeatureClass.TabIndex = 53;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeatureClass_Click);
            // 
            // btnProjectBoundary
            // 
            this.btnProjectBoundary.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnProjectBoundary.Location = new System.Drawing.Point(245, 24);
            this.btnProjectBoundary.Name = "btnProjectBoundary";
            this.btnProjectBoundary.Size = new System.Drawing.Size(23, 24);
            this.btnProjectBoundary.TabIndex = 65;
            this.btnProjectBoundary.UseVisualStyleBackColor = true;
            this.btnProjectBoundary.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 64;
            this.label3.Text = "Project Area Boundary";
            // 
            // cmbProjectBoundary
            // 
            this.cmbProjectBoundary.FormattingEnabled = true;
            this.cmbProjectBoundary.Location = new System.Drawing.Point(19, 26);
            this.cmbProjectBoundary.Name = "cmbProjectBoundary";
            this.cmbProjectBoundary.Size = new System.Drawing.Size(220, 21);
            this.cmbProjectBoundary.TabIndex = 63;
            // 
            // frmAdjustAccuracyAssessment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 244);
            this.Controls.Add(this.btnProjectBoundary);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbProjectBoundary);
            this.Controls.Add(this.txtOAA);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtOutModel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudAlpha);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnOpenRaster);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbMap);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAdjustAccuracyAssessment";
            this.Text = "Adjust Accuracy Assessment";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOutModel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudAlpha;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbMap;
        private System.Windows.Forms.TextBox txtOAA;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnProjectBoundary;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbProjectBoundary;
    }
}