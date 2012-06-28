namespace esriUtil.Forms.SasProcedures
{
    partial class frmAccuracyAssessment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAccuracyAssessment));
            this.label6 = new System.Windows.Forms.Label();
            this.cmbWeight = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbMap = new System.Windows.Forms.ComboBox();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbRef = new System.Windows.Forms.ComboBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.chbEditSas = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbRst = new System.Windows.Forms.ComboBox();
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.nudAlpha = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.chbExact = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "Weight Field (Optional)";
            // 
            // cmbWeight
            // 
            this.cmbWeight.FormattingEnabled = true;
            this.cmbWeight.Location = new System.Drawing.Point(19, 173);
            this.cmbWeight.Name = "cmbWeight";
            this.cmbWeight.Size = new System.Drawing.Size(220, 21);
            this.cmbWeight.TabIndex = 38;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Mapped Field";
            // 
            // cmbMap
            // 
            this.cmbMap.FormattingEnabled = true;
            this.cmbMap.Location = new System.Drawing.Point(19, 81);
            this.cmbMap.Name = "cmbMap";
            this.cmbMap.Size = new System.Drawing.Size(220, 21);
            this.cmbMap.TabIndex = 36;
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(245, 33);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(23, 24);
            this.btnOpenFeatureClass.TabIndex = 35;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeture_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Feature Class";
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(19, 35);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(220, 21);
            this.cmbSampleFeatureClass.TabIndex = 33;
            this.cmbSampleFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 41;
            this.label3.Text = "Reference Field";
            // 
            // cmbRef
            // 
            this.cmbRef.FormattingEnabled = true;
            this.cmbRef.Location = new System.Drawing.Point(19, 130);
            this.cmbRef.Name = "cmbRef";
            this.cmbRef.Size = new System.Drawing.Size(220, 21);
            this.cmbRef.TabIndex = 40;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(192, 257);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 42;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // chbEditSas
            // 
            this.chbEditSas.AutoSize = true;
            this.chbEditSas.Location = new System.Drawing.Point(19, 258);
            this.chbEditSas.Name = "chbEditSas";
            this.chbEditSas.Size = new System.Drawing.Size(72, 17);
            this.chbEditSas.TabIndex = 44;
            this.chbEditSas.Text = "Edit Code";
            this.chbEditSas.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 200);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 13);
            this.label4.TabIndex = 47;
            this.label4.Text = "Map to Update CI (Optional)";
            // 
            // cmbRst
            // 
            this.cmbRst.FormattingEnabled = true;
            this.cmbRst.Location = new System.Drawing.Point(19, 218);
            this.cmbRst.Name = "cmbRst";
            this.cmbRst.Size = new System.Drawing.Size(220, 21);
            this.cmbRst.TabIndex = 46;
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(242, 216);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(26, 25);
            this.btnOpenRaster.TabIndex = 45;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // nudAlpha
            // 
            this.nudAlpha.DecimalPlaces = 2;
            this.nudAlpha.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudAlpha.Location = new System.Drawing.Point(219, 9);
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
            this.nudAlpha.TabIndex = 48;
            this.nudAlpha.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(180, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 49;
            this.label5.Text = "Alpha";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chbExact
            // 
            this.chbExact.AutoSize = true;
            this.chbExact.Location = new System.Drawing.Point(104, 258);
            this.chbExact.Name = "chbExact";
            this.chbExact.Size = new System.Drawing.Size(53, 17);
            this.chbExact.TabIndex = 50;
            this.chbExact.Text = "Exact";
            this.chbExact.UseVisualStyleBackColor = true;
            // 
            // frmAccuracyAssessment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 291);
            this.Controls.Add(this.chbExact);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudAlpha);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbRst);
            this.Controls.Add(this.btnOpenRaster);
            this.Controls.Add(this.chbEditSas);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbRef);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbWeight);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbMap);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSampleFeatureClass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAccuracyAssessment";
            this.Text = "Accuracy Assessment";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbWeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbMap;
        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbRef;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.CheckBox chbEditSas;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbRst;
        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.NumericUpDown nudAlpha;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chbExact;
    }
}