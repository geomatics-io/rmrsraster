namespace esriUtil.Forms.SasProcedures
{
    partial class frmBuildPolytomousLogisticRegressionModel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBuildPolytomousLogisticRegressionModel));
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.lstIndependent = new System.Windows.Forms.ListBox();
            this.cmbIndependent = new System.Windows.Forms.ComboBox();
            this.cmbDepedent = new System.Windows.Forms.ComboBox();
            this.chbValidate = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudEnter = new System.Windows.Forms.NumericUpDown();
            this.nudStay = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.gpSelection = new System.Windows.Forms.GroupBox();
            this.chbStepWise = new System.Windows.Forms.CheckBox();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.chbResults = new System.Windows.Forms.CheckBox();
            this.chbSas = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbWeight = new System.Windows.Forms.ComboBox();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudEnter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStay)).BeginInit();
            this.gpSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Training Dataset";
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(20, 28);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(220, 21);
            this.cmbSampleFeatureClass.TabIndex = 14;
            this.cmbSampleFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            // 
            // lstIndependent
            // 
            this.lstIndependent.FormattingEnabled = true;
            this.lstIndependent.Location = new System.Drawing.Point(3, 103);
            this.lstIndependent.Name = "lstIndependent";
            this.lstIndependent.Size = new System.Drawing.Size(220, 160);
            this.lstIndependent.TabIndex = 17;
            // 
            // cmbIndependent
            // 
            this.cmbIndependent.FormattingEnabled = true;
            this.cmbIndependent.Location = new System.Drawing.Point(3, 80);
            this.cmbIndependent.Name = "cmbIndependent";
            this.cmbIndependent.Size = new System.Drawing.Size(220, 21);
            this.cmbIndependent.TabIndex = 18;
            this.cmbIndependent.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // cmbDepedent
            // 
            this.cmbDepedent.FormattingEnabled = true;
            this.cmbDepedent.Location = new System.Drawing.Point(20, 72);
            this.cmbDepedent.Name = "cmbDepedent";
            this.cmbDepedent.Size = new System.Drawing.Size(220, 21);
            this.cmbDepedent.TabIndex = 19;
            // 
            // chbValidate
            // 
            this.chbValidate.AutoSize = true;
            this.chbValidate.Location = new System.Drawing.Point(23, 425);
            this.chbValidate.Name = "chbValidate";
            this.chbValidate.Size = new System.Drawing.Size(72, 17);
            this.chbValidate.TabIndex = 20;
            this.chbValidate.Text = "Validation";
            this.chbValidate.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Dependent Field (Categorical)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Independent Fields";
            // 
            // nudEnter
            // 
            this.nudEnter.Location = new System.Drawing.Point(160, 37);
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
            // nudStay
            // 
            this.nudStay.Location = new System.Drawing.Point(209, 37);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(109, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "% Alpha   Enter";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(206, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "Stay";
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
            this.gpSelection.Location = new System.Drawing.Point(17, 144);
            this.gpSelection.Name = "gpSelection";
            this.gpSelection.Size = new System.Drawing.Size(257, 275);
            this.gpSelection.TabIndex = 27;
            this.gpSelection.TabStop = false;
            this.gpSelection.Text = "Parameters";
            // 
            // chbStepWise
            // 
            this.chbStepWise.AutoSize = true;
            this.chbStepWise.Checked = true;
            this.chbStepWise.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbStepWise.Location = new System.Drawing.Point(34, 37);
            this.chbStepWise.Name = "chbStepWise";
            this.chbStepWise.Size = new System.Drawing.Size(116, 17);
            this.chbStepWise.TabIndex = 31;
            this.chbStepWise.Text = "Stepwise Selection";
            this.chbStepWise.UseVisualStyleBackColor = true;
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(227, 164);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveAll.TabIndex = 30;
            this.btnRemoveAll.Text = "!";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(227, 106);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(25, 23);
            this.btnMinus.TabIndex = 29;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(227, 135);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(25, 23);
            this.btnAddAll.TabIndex = 28;
            this.btnAddAll.Text = "%";
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(194, 454);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 28;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // chbResults
            // 
            this.chbResults.AutoSize = true;
            this.chbResults.Checked = true;
            this.chbResults.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbResults.Location = new System.Drawing.Point(101, 425);
            this.chbResults.Name = "chbResults";
            this.chbResults.Size = new System.Drawing.Size(87, 17);
            this.chbResults.TabIndex = 29;
            this.chbResults.Text = "View Results";
            this.chbResults.UseVisualStyleBackColor = true;
            // 
            // chbSas
            // 
            this.chbSas.AutoSize = true;
            this.chbSas.Location = new System.Drawing.Point(194, 426);
            this.chbSas.Name = "chbSas";
            this.chbSas.Size = new System.Drawing.Size(68, 17);
            this.chbSas.TabIndex = 30;
            this.chbSas.Text = "Edit SAS";
            this.chbSas.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "Weight Field (Optional)";
            // 
            // cmbWeight
            // 
            this.cmbWeight.FormattingEnabled = true;
            this.cmbWeight.Location = new System.Drawing.Point(23, 117);
            this.cmbWeight.Name = "cmbWeight";
            this.cmbWeight.Size = new System.Drawing.Size(220, 21);
            this.cmbWeight.TabIndex = 31;
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(246, 26);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(23, 24);
            this.btnOpenFeatureClass.TabIndex = 16;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeture_Click);
            // 
            // frmBuildPolytomousLogisticRegressionModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 490);
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
            this.Name = "frmBuildPolytomousLogisticRegressionModel";
            this.Text = "Model LR \\ PLR";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudEnter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStay)).EndInit();
            this.gpSelection.ResumeLayout(false);
            this.gpSelection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
        private System.Windows.Forms.ListBox lstIndependent;
        private System.Windows.Forms.ComboBox cmbIndependent;
        private System.Windows.Forms.ComboBox cmbDepedent;
        private System.Windows.Forms.CheckBox chbValidate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudEnter;
        private System.Windows.Forms.NumericUpDown nudStay;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox gpSelection;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.CheckBox chbResults;
        private System.Windows.Forms.CheckBox chbSas;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbWeight;
        private System.Windows.Forms.CheckBox chbStepWise;
    }
}