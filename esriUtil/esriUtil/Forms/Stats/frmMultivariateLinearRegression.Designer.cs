namespace esriUtil.Forms.Stats
{
    partial class frmMultivariateLinearRegression
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMultivariateLinearRegression));
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.gpSelection = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.nudAlpha = new System.Windows.Forms.NumericUpDown();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbIndependent = new System.Windows.Forms.ComboBox();
            this.lstIndependent = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.btnOutputModel = new System.Windows.Forms.Button();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRemoveall2 = new System.Windows.Forms.Button();
            this.btnMinus2 = new System.Windows.Forms.Button();
            this.btnAddAll2 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbDependent = new System.Windows.Forms.ComboBox();
            this.lstDependent = new System.Windows.Forms.ListBox();
            this.chbIntOrigin = new System.Windows.Forms.CheckBox();
            this.gpSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(279, 30);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(220, 20);
            this.txtOutputPath.TabIndex = 60;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(278, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 58;
            this.label6.Text = "Model Output Path";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(454, 313);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 57;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // gpSelection
            // 
            this.gpSelection.Controls.Add(this.label4);
            this.gpSelection.Controls.Add(this.nudAlpha);
            this.gpSelection.Controls.Add(this.btnRemoveAll);
            this.gpSelection.Controls.Add(this.btnMinus);
            this.gpSelection.Controls.Add(this.btnAddAll);
            this.gpSelection.Controls.Add(this.label3);
            this.gpSelection.Controls.Add(this.cmbIndependent);
            this.gpSelection.Controls.Add(this.lstIndependent);
            this.gpSelection.Location = new System.Drawing.Point(276, 64);
            this.gpSelection.Name = "gpSelection";
            this.gpSelection.Size = new System.Drawing.Size(257, 243);
            this.gpSelection.TabIndex = 56;
            this.gpSelection.TabStop = false;
            this.gpSelection.Text = "X Parameters";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(134, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Alpha";
            // 
            // nudAlpha
            // 
            this.nudAlpha.DecimalPlaces = 3;
            this.nudAlpha.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudAlpha.Location = new System.Drawing.Point(174, 20);
            this.nudAlpha.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAlpha.Name = "nudAlpha";
            this.nudAlpha.Size = new System.Drawing.Size(49, 20);
            this.nudAlpha.TabIndex = 31;
            this.nudAlpha.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(227, 131);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveAll.TabIndex = 30;
            this.btnRemoveAll.Text = "!";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(227, 73);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(25, 23);
            this.btnMinus.TabIndex = 29;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(227, 102);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(25, 23);
            this.btnAddAll.TabIndex = 28;
            this.btnAddAll.Text = "%";
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Independent Fields";
            // 
            // cmbIndependent
            // 
            this.cmbIndependent.FormattingEnabled = true;
            this.cmbIndependent.Location = new System.Drawing.Point(3, 47);
            this.cmbIndependent.Name = "cmbIndependent";
            this.cmbIndependent.Size = new System.Drawing.Size(220, 21);
            this.cmbIndependent.TabIndex = 18;
            this.cmbIndependent.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // lstIndependent
            // 
            this.lstIndependent.FormattingEnabled = true;
            this.lstIndependent.Location = new System.Drawing.Point(3, 70);
            this.lstIndependent.Name = "lstIndependent";
            this.lstIndependent.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstIndependent.Size = new System.Drawing.Size(220, 160);
            this.lstIndependent.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 52;
            this.label2.Text = "Training Dataset";
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(16, 29);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(220, 21);
            this.cmbSampleFeatureClass.TabIndex = 51;
            this.cmbSampleFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            // 
            // btnOutputModel
            // 
            this.btnOutputModel.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutputModel.Location = new System.Drawing.Point(503, 27);
            this.btnOutputModel.Name = "btnOutputModel";
            this.btnOutputModel.Size = new System.Drawing.Size(23, 24);
            this.btnOutputModel.TabIndex = 59;
            this.btnOutputModel.UseVisualStyleBackColor = true;
            this.btnOutputModel.Click += new System.EventHandler(this.btnOutputModel_Click);
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(242, 27);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(23, 24);
            this.btnOpenFeatureClass.TabIndex = 53;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeture_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRemoveall2);
            this.groupBox1.Controls.Add(this.btnMinus2);
            this.groupBox1.Controls.Add(this.btnAddAll2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cmbDependent);
            this.groupBox1.Controls.Add(this.lstDependent);
            this.groupBox1.Location = new System.Drawing.Point(8, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 243);
            this.groupBox1.TabIndex = 57;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Y Parameters";
            // 
            // btnRemoveall2
            // 
            this.btnRemoveall2.Location = new System.Drawing.Point(227, 131);
            this.btnRemoveall2.Name = "btnRemoveall2";
            this.btnRemoveall2.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveall2.TabIndex = 30;
            this.btnRemoveall2.Text = "!";
            this.btnRemoveall2.UseVisualStyleBackColor = true;
            this.btnRemoveall2.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnMinus2
            // 
            this.btnMinus2.Location = new System.Drawing.Point(227, 73);
            this.btnMinus2.Name = "btnMinus2";
            this.btnMinus2.Size = new System.Drawing.Size(25, 23);
            this.btnMinus2.TabIndex = 29;
            this.btnMinus2.Text = "-";
            this.btnMinus2.UseVisualStyleBackColor = true;
            this.btnMinus2.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll2
            // 
            this.btnAddAll2.Location = new System.Drawing.Point(227, 102);
            this.btnAddAll2.Name = "btnAddAll2";
            this.btnAddAll2.Size = new System.Drawing.Size(25, 23);
            this.btnAddAll2.TabIndex = 28;
            this.btnAddAll2.Text = "%";
            this.btnAddAll2.UseVisualStyleBackColor = true;
            this.btnAddAll2.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Dependent Fields";
            // 
            // cmbDependent
            // 
            this.cmbDependent.FormattingEnabled = true;
            this.cmbDependent.Location = new System.Drawing.Point(3, 47);
            this.cmbDependent.Name = "cmbDependent";
            this.cmbDependent.Size = new System.Drawing.Size(220, 21);
            this.cmbDependent.TabIndex = 18;
            this.cmbDependent.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // lstDependent
            // 
            this.lstDependent.FormattingEnabled = true;
            this.lstDependent.Location = new System.Drawing.Point(3, 70);
            this.lstDependent.Name = "lstDependent";
            this.lstDependent.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstDependent.Size = new System.Drawing.Size(220, 160);
            this.lstDependent.TabIndex = 17;
            // 
            // chbIntOrigin
            // 
            this.chbIntOrigin.AutoSize = true;
            this.chbIntOrigin.Location = new System.Drawing.Point(16, 318);
            this.chbIntOrigin.Name = "chbIntOrigin";
            this.chbIntOrigin.Size = new System.Drawing.Size(141, 17);
            this.chbIntOrigin.TabIndex = 61;
            this.chbIntOrigin.Text = "Intercept Through Origin";
            this.chbIntOrigin.UseVisualStyleBackColor = true;
            // 
            // frmMultivariateLinearRegression
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 343);
            this.Controls.Add(this.chbIntOrigin);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.btnOutputModel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.gpSelection);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSampleFeatureClass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMultivariateLinearRegression";
            this.Text = "Model Multivariate Regression";
            this.TopMost = true;
            this.gpSelection.ResumeLayout(false);
            this.gpSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button btnOutputModel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.GroupBox gpSelection;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudAlpha;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbIndependent;
        private System.Windows.Forms.ListBox lstIndependent;
        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRemoveall2;
        private System.Windows.Forms.Button btnMinus2;
        private System.Windows.Forms.Button btnAddAll2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbDependent;
        private System.Windows.Forms.ListBox lstDependent;
        private System.Windows.Forms.CheckBox chbIntOrigin;
    }
}