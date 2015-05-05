namespace esriUtil.Forms.FIA
{
    partial class frmFiaBiomass
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFiaBiomass));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.clbBiomassTypes = new System.Windows.Forms.CheckedListBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.cmbPlot = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbSubPlot = new System.Windows.Forms.ComboBox();
            this.txtAccessDb = new System.Windows.Forms.TextBox();
            this.btnGrp = new System.Windows.Forms.Button();
            this.btnAccessDb = new System.Windows.Forms.Button();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Sample Locations";
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(12, 30);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(210, 21);
            this.cmbSampleFeatureClass.TabIndex = 15;
            this.cmbSampleFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "FIA Database";
            // 
            // clbBiomassTypes
            // 
            this.clbBiomassTypes.FormattingEnabled = true;
            this.clbBiomassTypes.Location = new System.Drawing.Point(12, 193);
            this.clbBiomassTypes.Name = "clbBiomassTypes";
            this.clbBiomassTypes.Size = new System.Drawing.Size(210, 154);
            this.clbBiomassTypes.TabIndex = 21;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(185, 355);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 22;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // cmbPlot
            // 
            this.cmbPlot.FormattingEnabled = true;
            this.cmbPlot.Location = new System.Drawing.Point(12, 76);
            this.cmbPlot.Name = "cmbPlot";
            this.cmbPlot.Size = new System.Drawing.Size(210, 21);
            this.cmbPlot.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Plot Link Field";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Subplot Link Field";
            // 
            // cmbSubPlot
            // 
            this.cmbSubPlot.FormattingEnabled = true;
            this.cmbSubPlot.Location = new System.Drawing.Point(12, 122);
            this.cmbSubPlot.Name = "cmbSubPlot";
            this.cmbSubPlot.Size = new System.Drawing.Size(210, 21);
            this.cmbSubPlot.TabIndex = 25;
            // 
            // txtAccessDb
            // 
            this.txtAccessDb.Location = new System.Drawing.Point(13, 165);
            this.txtAccessDb.Name = "txtAccessDb";
            this.txtAccessDb.Size = new System.Drawing.Size(209, 20);
            this.txtAccessDb.TabIndex = 27;
            // 
            // btnGrp
            // 
            this.btnGrp.Location = new System.Drawing.Point(12, 354);
            this.btnGrp.Name = "btnGrp";
            this.btnGrp.Size = new System.Drawing.Size(49, 23);
            this.btnGrp.TabIndex = 28;
            this.btnGrp.Text = "Group";
            this.btnGrp.UseVisualStyleBackColor = true;
            this.btnGrp.Click += new System.EventHandler(this.btnGrp_Click);
            // 
            // btnAccessDb
            // 
            this.btnAccessDb.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnAccessDb.Location = new System.Drawing.Point(233, 161);
            this.btnAccessDb.Name = "btnAccessDb";
            this.btnAccessDb.Size = new System.Drawing.Size(27, 28);
            this.btnAccessDb.TabIndex = 20;
            this.btnAccessDb.UseVisualStyleBackColor = true;
            this.btnAccessDb.Click += new System.EventHandler(this.btnAccessDb_Click);
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(233, 26);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(27, 28);
            this.btnOpenFeatureClass.TabIndex = 17;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeatureClass_Click);
            // 
            // frmFiaBiomass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 382);
            this.Controls.Add(this.btnGrp);
            this.Controls.Add(this.txtAccessDb);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbSubPlot);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbPlot);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.clbBiomassTypes);
            this.Controls.Add(this.btnAccessDb);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSampleFeatureClass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFiaBiomass";
            this.Text = "FIA Biomass";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
        private System.Windows.Forms.Button btnAccessDb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox clbBiomassTypes;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.ComboBox cmbPlot;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbSubPlot;
        private System.Windows.Forms.TextBox txtAccessDb;
        private System.Windows.Forms.Button btnGrp;
    }
}