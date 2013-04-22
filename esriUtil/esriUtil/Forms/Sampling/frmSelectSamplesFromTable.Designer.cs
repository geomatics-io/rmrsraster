namespace esriUtil.Forms.Sampling
{
    partial class frmSelectSamplesFromTable
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelectSamplesFromTable));
            this.label1 = new System.Windows.Forms.Label();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.nudAlpha = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudProp = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbMap = new System.Windows.Forms.ComboBox();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnExcute = new System.Windows.Forms.Button();
            this.chbEqual = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudProp)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 47;
            this.label1.Text = "Model Path";
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(9, 122);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(220, 20);
            this.txtOutputPath.TabIndex = 46;
            // 
            // button1
            // 
            this.button1.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button1.Location = new System.Drawing.Point(232, 118);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 26);
            this.button1.TabIndex = 45;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // nudAlpha
            // 
            this.nudAlpha.DecimalPlaces = 2;
            this.nudAlpha.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudAlpha.Location = new System.Drawing.Point(101, 179);
            this.nudAlpha.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAlpha.Name = "nudAlpha";
            this.nudAlpha.Size = new System.Drawing.Size(47, 20);
            this.nudAlpha.TabIndex = 48;
            this.nudAlpha.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(98, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 49;
            this.label2.Text = "Alpha";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 51;
            this.label3.Text = "Width (%Mean)";
            // 
            // nudProp
            // 
            this.nudProp.DecimalPlaces = 2;
            this.nudProp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudProp.Location = new System.Drawing.Point(9, 179);
            this.nudProp.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudProp.Name = "nudProp";
            this.nudProp.Size = new System.Drawing.Size(47, 20);
            this.nudProp.TabIndex = 50;
            this.nudProp.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 13);
            this.label4.TabIndex = 56;
            this.label4.Text = "Cluster/Strata/Class Field";
            // 
            // cmbMap
            // 
            this.cmbMap.FormattingEnabled = true;
            this.cmbMap.Location = new System.Drawing.Point(9, 74);
            this.cmbMap.Name = "cmbMap";
            this.cmbMap.Size = new System.Drawing.Size(220, 21);
            this.cmbMap.TabIndex = 55;
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(303, 23);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(23, 24);
            this.btnOpenFeatureClass.TabIndex = 54;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 53;
            this.label6.Text = "Table";
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(9, 28);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(220, 21);
            this.cmbSampleFeatureClass.TabIndex = 52;
            this.cmbSampleFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.button2.Location = new System.Drawing.Point(232, 26);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(23, 24);
            this.button2.TabIndex = 57;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnOpenFeture_Click);
            // 
            // btnExcute
            // 
            this.btnExcute.Location = new System.Drawing.Point(194, 205);
            this.btnExcute.Name = "btnExcute";
            this.btnExcute.Size = new System.Drawing.Size(61, 23);
            this.btnExcute.TabIndex = 58;
            this.btnExcute.Text = "Execute";
            this.btnExcute.UseVisualStyleBackColor = true;
            this.btnExcute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // chbEqual
            // 
            this.chbEqual.AutoSize = true;
            this.chbEqual.Location = new System.Drawing.Point(9, 209);
            this.chbEqual.Name = "chbEqual";
            this.chbEqual.Size = new System.Drawing.Size(138, 17);
            this.chbEqual.TabIndex = 59;
            this.chbEqual.Text = "Adjust for equal weights";
            this.chbEqual.UseVisualStyleBackColor = true;
            // 
            // frmSelectSamplesFromTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 243);
            this.Controls.Add(this.chbEqual);
            this.Controls.Add(this.btnExcute);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbMap);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbSampleFeatureClass);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudProp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudAlpha);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSelectSamplesFromTable";
            this.Text = "Select Samples";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudProp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown nudAlpha;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudProp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbMap;
        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnExcute;
        private System.Windows.Forms.CheckBox chbEqual;
    }
}