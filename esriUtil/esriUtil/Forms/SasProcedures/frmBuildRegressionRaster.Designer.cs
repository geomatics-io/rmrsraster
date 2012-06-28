namespace esriUtil.Forms.SasProcedures
{
    partial class frmBuildRegressionRaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBuildRegressionRaster));
            this.label6 = new System.Windows.Forms.Label();
            this.cmbWeight = new System.Windows.Forms.ComboBox();
            this.chbSas = new System.Windows.Forms.CheckBox();
            this.chbResults = new System.Windows.Forms.CheckBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.gpSelection = new System.Windows.Forms.GroupBox();
            this.chbStepWise = new System.Windows.Forms.CheckBox();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nudStay = new System.Windows.Forms.NumericUpDown();
            this.nudEnter = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbIndependent = new System.Windows.Forms.ComboBox();
            this.lstIndependent = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chbValidate = new System.Windows.Forms.CheckBox();
            this.cmbDepedent = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.lstDependent = new System.Windows.Forms.ListBox();
            this.btnMinDep = new System.Windows.Forms.Button();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.gpSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnter)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 223);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 13);
            this.label6.TabIndex = 44;
            this.label6.Text = "Weight Field (Optional)";
            // 
            // cmbWeight
            // 
            this.cmbWeight.FormattingEnabled = true;
            this.cmbWeight.Location = new System.Drawing.Point(24, 242);
            this.cmbWeight.Name = "cmbWeight";
            this.cmbWeight.Size = new System.Drawing.Size(220, 21);
            this.cmbWeight.TabIndex = 43;
            // 
            // chbSas
            // 
            this.chbSas.AutoSize = true;
            this.chbSas.Location = new System.Drawing.Point(195, 547);
            this.chbSas.Name = "chbSas";
            this.chbSas.Size = new System.Drawing.Size(68, 17);
            this.chbSas.TabIndex = 42;
            this.chbSas.Text = "Edit SAS";
            this.chbSas.UseVisualStyleBackColor = true;
            // 
            // chbResults
            // 
            this.chbResults.AutoSize = true;
            this.chbResults.Checked = true;
            this.chbResults.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbResults.Location = new System.Drawing.Point(102, 546);
            this.chbResults.Name = "chbResults";
            this.chbResults.Size = new System.Drawing.Size(87, 17);
            this.chbResults.TabIndex = 41;
            this.chbResults.Text = "View Results";
            this.chbResults.UseVisualStyleBackColor = true;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(195, 575);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 40;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // gpSelection
            // 
            this.gpSelection.Controls.Add(this.chbStepWise);
            this.gpSelection.Controls.Add(this.btnRemoveAll);
            this.gpSelection.Controls.Add(this.btnMinus);
            this.gpSelection.Controls.Add(this.btnAddAll);
            this.gpSelection.Controls.Add(this.label5);
            this.gpSelection.Controls.Add(this.label4);
            this.gpSelection.Controls.Add(this.nudStay);
            this.gpSelection.Controls.Add(this.nudEnter);
            this.gpSelection.Controls.Add(this.label3);
            this.gpSelection.Controls.Add(this.cmbIndependent);
            this.gpSelection.Controls.Add(this.lstIndependent);
            this.gpSelection.Location = new System.Drawing.Point(20, 269);
            this.gpSelection.Name = "gpSelection";
            this.gpSelection.Size = new System.Drawing.Size(257, 272);
            this.gpSelection.TabIndex = 39;
            this.gpSelection.TabStop = false;
            this.gpSelection.Text = "Parameters";
            // 
            // chbStepWise
            // 
            this.chbStepWise.AutoSize = true;
            this.chbStepWise.Checked = true;
            this.chbStepWise.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbStepWise.Location = new System.Drawing.Point(21, 33);
            this.chbStepWise.Name = "chbStepWise";
            this.chbStepWise.Size = new System.Drawing.Size(116, 17);
            this.chbStepWise.TabIndex = 31;
            this.chbStepWise.Text = "Stepwise Selection";
            this.chbStepWise.UseVisualStyleBackColor = true;
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(227, 160);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveAll.TabIndex = 30;
            this.btnRemoveAll.Text = "!";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(227, 102);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(25, 23);
            this.btnMinus.TabIndex = 29;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(227, 131);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(25, 23);
            this.btnAddAll.TabIndex = 28;
            this.btnAddAll.Text = "%";
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(208, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "Stay";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(111, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "% Alpha   Enter";
            // 
            // nudStay
            // 
            this.nudStay.Location = new System.Drawing.Point(211, 33);
            this.nudStay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudStay.Name = "nudStay";
            this.nudStay.Size = new System.Drawing.Size(40, 20);
            this.nudStay.TabIndex = 24;
            this.nudStay.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // nudEnter
            // 
            this.nudEnter.Location = new System.Drawing.Point(162, 33);
            this.nudEnter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudEnter.Name = "nudEnter";
            this.nudEnter.Size = new System.Drawing.Size(40, 20);
            this.nudEnter.TabIndex = 23;
            this.nudEnter.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Independent Fields";
            // 
            // cmbIndependent
            // 
            this.cmbIndependent.FormattingEnabled = true;
            this.cmbIndependent.Location = new System.Drawing.Point(3, 76);
            this.cmbIndependent.Name = "cmbIndependent";
            this.cmbIndependent.Size = new System.Drawing.Size(220, 21);
            this.cmbIndependent.TabIndex = 18;
            this.cmbIndependent.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // lstIndependent
            // 
            this.lstIndependent.FormattingEnabled = true;
            this.lstIndependent.Location = new System.Drawing.Point(3, 99);
            this.lstIndependent.Name = "lstIndependent";
            this.lstIndependent.Size = new System.Drawing.Size(220, 160);
            this.lstIndependent.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "Dependent Field(s)";
            // 
            // chbValidate
            // 
            this.chbValidate.AutoSize = true;
            this.chbValidate.Location = new System.Drawing.Point(24, 546);
            this.chbValidate.Name = "chbValidate";
            this.chbValidate.Size = new System.Drawing.Size(72, 17);
            this.chbValidate.TabIndex = 37;
            this.chbValidate.Text = "Validation";
            this.chbValidate.UseVisualStyleBackColor = true;
            // 
            // cmbDepedent
            // 
            this.cmbDepedent.FormattingEnabled = true;
            this.cmbDepedent.Location = new System.Drawing.Point(21, 73);
            this.cmbDepedent.Name = "cmbDepedent";
            this.cmbDepedent.Size = new System.Drawing.Size(220, 21);
            this.cmbDepedent.TabIndex = 36;
            this.cmbDepedent.SelectedIndexChanged += new System.EventHandler(this.cmbDepedent_SelectedIndexChanged);
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
            // lstDependent
            // 
            this.lstDependent.FormattingEnabled = true;
            this.lstDependent.Location = new System.Drawing.Point(21, 98);
            this.lstDependent.Name = "lstDependent";
            this.lstDependent.Size = new System.Drawing.Size(220, 95);
            this.lstDependent.TabIndex = 45;
            // 
            // btnMinDep
            // 
            this.btnMinDep.Location = new System.Drawing.Point(247, 73);
            this.btnMinDep.Name = "btnMinDep";
            this.btnMinDep.Size = new System.Drawing.Size(25, 23);
            this.btnMinDep.TabIndex = 46;
            this.btnMinDep.Text = "-";
            this.btnMinDep.UseVisualStyleBackColor = true;
            this.btnMinDep.Click += new System.EventHandler(this.btnMinDep_Click);
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
            // frmBuildRegressionRaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 602);
            this.Controls.Add(this.btnMinDep);
            this.Controls.Add(this.lstDependent);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbWeight);
            this.Controls.Add(this.chbSas);
            this.Controls.Add(this.chbResults);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.gpSelection);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chbValidate);
            this.Controls.Add(this.cmbDepedent);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSampleFeatureClass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmBuildRegressionRaster";
            this.Text = "Model Regression";
            this.TopMost = true;
            this.gpSelection.ResumeLayout(false);
            this.gpSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbWeight;
        private System.Windows.Forms.CheckBox chbSas;
        private System.Windows.Forms.CheckBox chbResults;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.GroupBox gpSelection;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudStay;
        private System.Windows.Forms.NumericUpDown nudEnter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbIndependent;
        private System.Windows.Forms.ListBox lstIndependent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chbValidate;
        private System.Windows.Forms.ComboBox cmbDepedent;
        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
        private System.Windows.Forms.ListBox lstDependent;
        private System.Windows.Forms.Button btnMinDep;
        private System.Windows.Forms.CheckBox chbStepWise;
    }
}