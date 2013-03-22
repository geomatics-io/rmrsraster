namespace esriUtil.Forms.Stats
{
    partial class frmLinearRegressionRaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLinearRegressionRaster));
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbDepedent = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.btnOutputModel = new System.Windows.Forms.Button();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.chbIntOrigin = new System.Windows.Forms.CheckBox();
            this.gpSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(202, 399);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 40;
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
            this.gpSelection.Location = new System.Drawing.Point(20, 102);
            this.gpSelection.Name = "gpSelection";
            this.gpSelection.Size = new System.Drawing.Size(257, 243);
            this.gpSelection.TabIndex = 39;
            this.gpSelection.TabStop = false;
            this.gpSelection.Text = "X Parameters";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(134, 16);
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
            this.nudAlpha.Location = new System.Drawing.Point(174, 13);
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
            this.btnRemoveAll.Location = new System.Drawing.Point(227, 124);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveAll.TabIndex = 30;
            this.btnRemoveAll.Text = "!";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(227, 66);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(25, 23);
            this.btnMinus.TabIndex = 29;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(227, 95);
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
            this.label3.Location = new System.Drawing.Point(3, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Independent Fields";
            // 
            // cmbIndependent
            // 
            this.cmbIndependent.FormattingEnabled = true;
            this.cmbIndependent.Location = new System.Drawing.Point(3, 40);
            this.cmbIndependent.Name = "cmbIndependent";
            this.cmbIndependent.Size = new System.Drawing.Size(220, 21);
            this.cmbIndependent.TabIndex = 18;
            this.cmbIndependent.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // lstIndependent
            // 
            this.lstIndependent.FormattingEnabled = true;
            this.lstIndependent.Location = new System.Drawing.Point(3, 63);
            this.lstIndependent.Name = "lstIndependent";
            this.lstIndependent.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstIndependent.Size = new System.Drawing.Size(220, 160);
            this.lstIndependent.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "Dependent Field";
            // 
            // cmbDepedent
            // 
            this.cmbDepedent.FormattingEnabled = true;
            this.cmbDepedent.Location = new System.Drawing.Point(21, 73);
            this.cmbDepedent.Name = "cmbDepedent";
            this.cmbDepedent.Size = new System.Drawing.Size(220, 21);
            this.cmbDepedent.TabIndex = 36;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Training Dataset";
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(21, 29);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(220, 21);
            this.cmbSampleFeatureClass.TabIndex = 33;
            this.cmbSampleFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 352);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 47;
            this.label6.Text = "Model Output Path";
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(20, 369);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(220, 20);
            this.txtOutputPath.TabIndex = 50;
            // 
            // btnOutputModel
            // 
            this.btnOutputModel.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutputModel.Location = new System.Drawing.Point(243, 368);
            this.btnOutputModel.Name = "btnOutputModel";
            this.btnOutputModel.Size = new System.Drawing.Size(23, 24);
            this.btnOutputModel.TabIndex = 48;
            this.btnOutputModel.UseVisualStyleBackColor = true;
            this.btnOutputModel.Click += new System.EventHandler(this.btnOutputModel_Click);
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(247, 27);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(23, 24);
            this.btnOpenFeatureClass.TabIndex = 35;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeture_Click);
            // 
            // chbIntOrigin
            // 
            this.chbIntOrigin.AutoSize = true;
            this.chbIntOrigin.Location = new System.Drawing.Point(23, 402);
            this.chbIntOrigin.Name = "chbIntOrigin";
            this.chbIntOrigin.Size = new System.Drawing.Size(141, 17);
            this.chbIntOrigin.TabIndex = 51;
            this.chbIntOrigin.Text = "Intercept Through Origin";
            this.chbIntOrigin.UseVisualStyleBackColor = true;
            // 
            // frmLinearRegressionRaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 429);
            this.Controls.Add(this.chbIntOrigin);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.btnOutputModel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.gpSelection);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbDepedent);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSampleFeatureClass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLinearRegressionRaster";
            this.Text = "Model Regression";
            this.TopMost = true;
            this.gpSelection.ResumeLayout(false);
            this.gpSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.GroupBox gpSelection;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbIndependent;
        private System.Windows.Forms.ListBox lstIndependent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbDepedent;
        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnOutputModel;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.NumericUpDown nudAlpha;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chbIntOrigin;
    }
}