namespace esriUtil.Forms.Stats
{
    partial class frmRandomForest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRandomForest));
            this.label5 = new System.Windows.Forms.Label();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.gpSelection = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.nudSplit = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nudratio = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudTrees = new System.Windows.Forms.NumericUpDown();
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
            this.button1 = new System.Windows.Forms.Button();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.chbReg = new System.Windows.Forms.CheckBox();
            this.chbVI = new System.Windows.Forms.CheckBox();
            this.gpSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSplit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudratio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrees)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 406);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "Output Model Path";
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(21, 424);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(220, 20);
            this.txtOutputPath.TabIndex = 41;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(195, 458);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 39;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // gpSelection
            // 
            this.gpSelection.Controls.Add(this.label7);
            this.gpSelection.Controls.Add(this.nudSplit);
            this.gpSelection.Controls.Add(this.label6);
            this.gpSelection.Controls.Add(this.nudratio);
            this.gpSelection.Controls.Add(this.label4);
            this.gpSelection.Controls.Add(this.nudTrees);
            this.gpSelection.Controls.Add(this.btnRemoveAll);
            this.gpSelection.Controls.Add(this.btnMinus);
            this.gpSelection.Controls.Add(this.btnAddAll);
            this.gpSelection.Controls.Add(this.label3);
            this.gpSelection.Controls.Add(this.cmbIndependent);
            this.gpSelection.Controls.Add(this.lstIndependent);
            this.gpSelection.Location = new System.Drawing.Point(18, 102);
            this.gpSelection.Name = "gpSelection";
            this.gpSelection.Size = new System.Drawing.Size(257, 288);
            this.gpSelection.TabIndex = 38;
            this.gpSelection.TabStop = false;
            this.gpSelection.Text = "Parameters";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(149, 233);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 13);
            this.label7.TabIndex = 36;
            this.label7.Text = "# Variables (Splitting)";
            // 
            // nudSplit
            // 
            this.nudSplit.Location = new System.Drawing.Point(152, 251);
            this.nudSplit.Name = "nudSplit";
            this.nudSplit.Size = new System.Drawing.Size(50, 20);
            this.nudSplit.TabIndex = 35;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(76, 233);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "Train Ratio";
            // 
            // nudratio
            // 
            this.nudratio.DecimalPlaces = 2;
            this.nudratio.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudratio.Location = new System.Drawing.Point(79, 251);
            this.nudratio.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudratio.Name = "nudratio";
            this.nudratio.Size = new System.Drawing.Size(50, 20);
            this.nudratio.TabIndex = 33;
            this.nudratio.Value = new decimal(new int[] {
            66,
            0,
            0,
            131072});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 233);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "# of Trees";
            // 
            // nudTrees
            // 
            this.nudTrees.Location = new System.Drawing.Point(6, 251);
            this.nudTrees.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudTrees.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudTrees.Name = "nudTrees";
            this.nudTrees.Size = new System.Drawing.Size(50, 20);
            this.nudTrees.TabIndex = 31;
            this.nudTrees.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(227, 123);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveAll.TabIndex = 30;
            this.btnRemoveAll.Text = "!";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(227, 65);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(25, 23);
            this.btnMinus.TabIndex = 29;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(227, 94);
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
            this.label3.Location = new System.Drawing.Point(3, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Independent Fields";
            // 
            // cmbIndependent
            // 
            this.cmbIndependent.FormattingEnabled = true;
            this.cmbIndependent.Location = new System.Drawing.Point(3, 39);
            this.cmbIndependent.Name = "cmbIndependent";
            this.cmbIndependent.Size = new System.Drawing.Size(220, 21);
            this.cmbIndependent.TabIndex = 18;
            this.cmbIndependent.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // lstIndependent
            // 
            this.lstIndependent.FormattingEnabled = true;
            this.lstIndependent.Location = new System.Drawing.Point(3, 62);
            this.lstIndependent.Name = "lstIndependent";
            this.lstIndependent.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstIndependent.Size = new System.Drawing.Size(220, 160);
            this.lstIndependent.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Dependent Field";
            // 
            // cmbDepedent
            // 
            this.cmbDepedent.FormattingEnabled = true;
            this.cmbDepedent.Location = new System.Drawing.Point(21, 75);
            this.cmbDepedent.Name = "cmbDepedent";
            this.cmbDepedent.Size = new System.Drawing.Size(220, 21);
            this.cmbDepedent.TabIndex = 36;
            this.cmbDepedent.SelectedIndexChanged += new System.EventHandler(this.cmbDepedent_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Training Dataset";
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(21, 31);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(220, 21);
            this.cmbSampleFeatureClass.TabIndex = 33;
            this.cmbSampleFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button1.Location = new System.Drawing.Point(247, 423);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 24);
            this.button1.TabIndex = 40;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(247, 29);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(23, 24);
            this.btnOpenFeatureClass.TabIndex = 35;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeture_Click);
            // 
            // chbReg
            // 
            this.chbReg.AutoSize = true;
            this.chbReg.Location = new System.Drawing.Point(24, 450);
            this.chbReg.Name = "chbReg";
            this.chbReg.Size = new System.Drawing.Size(79, 17);
            this.chbReg.TabIndex = 43;
            this.chbReg.Text = "Regression";
            this.chbReg.UseVisualStyleBackColor = true;
            // 
            // chbVI
            // 
            this.chbVI.AutoSize = true;
            this.chbVI.Location = new System.Drawing.Point(24, 476);
            this.chbVI.Name = "chbVI";
            this.chbVI.Size = new System.Drawing.Size(120, 17);
            this.chbVI.TabIndex = 44;
            this.chbVI.Text = "Variable Importance";
            this.chbVI.UseVisualStyleBackColor = true;
            // 
            // frmRandomForest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 498);
            this.Controls.Add(this.chbVI);
            this.Controls.Add(this.chbReg);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.gpSelection);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbDepedent);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSampleFeatureClass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRandomForest";
            this.Text = "Random Forest";
            this.TopMost = true;
            this.gpSelection.ResumeLayout(false);
            this.gpSelection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSplit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudratio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrees)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button button1;
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
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudSplit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudratio;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudTrees;
        private System.Windows.Forms.CheckBox chbReg;
        private System.Windows.Forms.CheckBox chbVI;
    }
}