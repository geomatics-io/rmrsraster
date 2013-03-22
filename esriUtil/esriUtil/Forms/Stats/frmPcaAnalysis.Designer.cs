namespace esriUtil.Forms.Stats
{
    partial class frmPcaAnalysis
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPcaAnalysis));
            this.label5 = new System.Windows.Forms.Label();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.gpSelection = new System.Windows.Forms.GroupBox();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbIndependent = new System.Windows.Forms.ComboBox();
            this.lstIndependent = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.gpSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 299);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 60;
            this.label5.Text = "Output Model Path";
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(15, 317);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(220, 20);
            this.txtOutputPath.TabIndex = 59;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(189, 351);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 57;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // gpSelection
            // 
            this.gpSelection.Controls.Add(this.btnRemoveAll);
            this.gpSelection.Controls.Add(this.btnMinus);
            this.gpSelection.Controls.Add(this.btnAddAll);
            this.gpSelection.Controls.Add(this.label3);
            this.gpSelection.Controls.Add(this.cmbIndependent);
            this.gpSelection.Controls.Add(this.lstIndependent);
            this.gpSelection.Location = new System.Drawing.Point(12, 50);
            this.gpSelection.Name = "gpSelection";
            this.gpSelection.Size = new System.Drawing.Size(257, 239);
            this.gpSelection.TabIndex = 56;
            this.gpSelection.TabStop = false;
            this.gpSelection.Text = "Parameters";
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(227, 129);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveAll.TabIndex = 30;
            this.btnRemoveAll.Text = "!";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(227, 71);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(25, 23);
            this.btnMinus.TabIndex = 29;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(227, 100);
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
            this.label3.Location = new System.Drawing.Point(3, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Variables";
            // 
            // cmbIndependent
            // 
            this.cmbIndependent.FormattingEnabled = true;
            this.cmbIndependent.Location = new System.Drawing.Point(3, 45);
            this.cmbIndependent.Name = "cmbIndependent";
            this.cmbIndependent.Size = new System.Drawing.Size(220, 21);
            this.cmbIndependent.TabIndex = 18;
            this.cmbIndependent.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // lstIndependent
            // 
            this.lstIndependent.FormattingEnabled = true;
            this.lstIndependent.Location = new System.Drawing.Point(3, 68);
            this.lstIndependent.Name = "lstIndependent";
            this.lstIndependent.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstIndependent.Size = new System.Drawing.Size(220, 160);
            this.lstIndependent.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 54;
            this.label2.Text = "Training Dataset";
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(15, 22);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(220, 21);
            this.cmbSampleFeatureClass.TabIndex = 53;
            this.cmbSampleFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button1.Location = new System.Drawing.Point(241, 316);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 24);
            this.button1.TabIndex = 58;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(241, 20);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(23, 24);
            this.btnOpenFeatureClass.TabIndex = 55;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeture_Click);
            // 
            // frmPcaAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 389);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.gpSelection);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSampleFeatureClass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPcaAnalysis";
            this.Text = "PCA Analysis";
            this.TopMost = true;
            this.gpSelection.ResumeLayout(false);
            this.gpSelection.PerformLayout();
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
        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
    }
}