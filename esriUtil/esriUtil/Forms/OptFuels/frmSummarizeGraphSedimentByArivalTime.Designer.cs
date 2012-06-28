namespace esriUtil.Forms.OptFuels
{
    partial class frmSummarizeGraphSedimentByArivalTime
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSummarizeGraphSedimentByArivalTime));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbScenerio = new System.Windows.Forms.ComboBox();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbInFeatureClass = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbProject = new System.Windows.Forms.ComboBox();
            this.cmbF10 = new System.Windows.Forms.ComboBox();
            this.cmbF50 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbT50 = new System.Windows.Forms.ComboBox();
            this.cmbT10 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbHSF50 = new System.Windows.Forms.ComboBox();
            this.cmbHSF10 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chbIntermidiate = new System.Windows.Forms.CheckBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.nudFlameLength = new System.Windows.Forms.NumericUpDown();
            this.chbCore = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chbAO = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFlameLength)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(136, 270);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Alternative";
            // 
            // cmbScenerio
            // 
            this.cmbScenerio.FormattingEnabled = true;
            this.cmbScenerio.Location = new System.Drawing.Point(139, 287);
            this.cmbScenerio.Name = "cmbScenerio";
            this.cmbScenerio.Size = new System.Drawing.Size(121, 21);
            this.cmbScenerio.TabIndex = 16;
            this.cmbScenerio.SelectedIndexChanged += new System.EventHandler(this.cmbScenerio_SelectedIndexChanged);
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(251, 32);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(27, 28);
            this.btnOpenFeatureClass.TabIndex = 23;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeatureClass_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(205, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Riparian Contribution Zone (RCZ) Polygon";
            // 
            // cmbInFeatureClass
            // 
            this.cmbInFeatureClass.FormattingEnabled = true;
            this.cmbInFeatureClass.Location = new System.Drawing.Point(12, 37);
            this.cmbInFeatureClass.Name = "cmbInFeatureClass";
            this.cmbInFeatureClass.Size = new System.Drawing.Size(233, 21);
            this.cmbInFeatureClass.TabIndex = 21;
            this.cmbInFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbInFeatureClass_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 270);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Analysis Area";
            // 
            // cmbProject
            // 
            this.cmbProject.FormattingEnabled = true;
            this.cmbProject.Location = new System.Drawing.Point(12, 287);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Size = new System.Drawing.Size(121, 21);
            this.cmbProject.TabIndex = 24;
            this.cmbProject.SelectedIndexChanged += new System.EventHandler(this.cmbProject_SelectedIndexChanged);
            // 
            // cmbF10
            // 
            this.cmbF10.FormattingEnabled = true;
            this.cmbF10.Location = new System.Drawing.Point(5, 35);
            this.cmbF10.Name = "cmbF10";
            this.cmbF10.Size = new System.Drawing.Size(121, 21);
            this.cmbF10.TabIndex = 26;
            // 
            // cmbF50
            // 
            this.cmbF50.FormattingEnabled = true;
            this.cmbF50.Location = new System.Drawing.Point(132, 35);
            this.cmbF50.Name = "cmbF50";
            this.cmbF50.Size = new System.Drawing.Size(121, 21);
            this.cmbF50.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(129, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "NA50";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "NA10";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "TR10";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(129, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "TR50";
            // 
            // cmbT50
            // 
            this.cmbT50.FormattingEnabled = true;
            this.cmbT50.Location = new System.Drawing.Point(132, 80);
            this.cmbT50.Name = "cmbT50";
            this.cmbT50.Size = new System.Drawing.Size(121, 21);
            this.cmbT50.TabIndex = 31;
            // 
            // cmbT10
            // 
            this.cmbT10.FormattingEnabled = true;
            this.cmbT10.Location = new System.Drawing.Point(5, 80);
            this.cmbT10.Name = "cmbT10";
            this.cmbT10.Size = new System.Drawing.Size(121, 21);
            this.cmbT10.TabIndex = 30;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "HS10";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(129, 108);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 36;
            this.label9.Text = "HS50";
            // 
            // cmbHSF50
            // 
            this.cmbHSF50.FormattingEnabled = true;
            this.cmbHSF50.Location = new System.Drawing.Point(132, 126);
            this.cmbHSF50.Name = "cmbHSF50";
            this.cmbHSF50.Size = new System.Drawing.Size(121, 21);
            this.cmbHSF50.TabIndex = 35;
            // 
            // cmbHSF10
            // 
            this.cmbHSF10.FormattingEnabled = true;
            this.cmbHSF10.Location = new System.Drawing.Point(5, 126);
            this.cmbHSF10.Name = "cmbHSF10";
            this.cmbHSF10.Size = new System.Drawing.Size(121, 21);
            this.cmbHSF10.TabIndex = 34;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.cmbHSF50);
            this.groupBox1.Controls.Add(this.cmbHSF10);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cmbT50);
            this.groupBox1.Controls.Add(this.cmbT10);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmbF50);
            this.groupBox1.Controls.Add(this.cmbF10);
            this.groupBox1.Location = new System.Drawing.Point(7, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(271, 159);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Identify Sediment Fields";
            // 
            // chbIntermidiate
            // 
            this.chbIntermidiate.AutoSize = true;
            this.chbIntermidiate.Location = new System.Drawing.Point(103, 18);
            this.chbIntermidiate.Name = "chbIntermidiate";
            this.chbIntermidiate.Size = new System.Drawing.Size(76, 17);
            this.chbIntermidiate.TabIndex = 39;
            this.chbIntermidiate.Text = "All Rasters";
            this.chbIntermidiate.UseVisualStyleBackColor = true;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(205, 339);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 40;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(21, 247);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 13);
            this.label11.TabIndex = 44;
            this.label11.Text = "Flame Length (Meters)";
            // 
            // nudFlameLength
            // 
            this.nudFlameLength.Location = new System.Drawing.Point(142, 244);
            this.nudFlameLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFlameLength.Name = "nudFlameLength";
            this.nudFlameLength.Size = new System.Drawing.Size(79, 20);
            this.nudFlameLength.TabIndex = 43;
            this.nudFlameLength.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // chbCore
            // 
            this.chbCore.AutoSize = true;
            this.chbCore.Location = new System.Drawing.Point(9, 18);
            this.chbCore.Name = "chbCore";
            this.chbCore.Size = new System.Drawing.Size(87, 17);
            this.chbCore.TabIndex = 45;
            this.chbCore.Text = "Core Rasters";
            this.chbCore.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chbCore);
            this.groupBox2.Controls.Add(this.chbIntermidiate);
            this.groupBox2.Location = new System.Drawing.Point(5, 342);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(188, 43);
            this.groupBox2.TabIndex = 46;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Store Intermdiate";
            this.groupBox2.Visible = false;
            // 
            // chbAO
            // 
            this.chbAO.AutoSize = true;
            this.chbAO.Location = new System.Drawing.Point(14, 319);
            this.chbAO.Name = "chbAO";
            this.chbAO.Size = new System.Drawing.Size(145, 17);
            this.chbAO.TabIndex = 47;
            this.chbAO.Text = "Show Advanced Outputs";
            this.chbAO.UseVisualStyleBackColor = true;
            this.chbAO.CheckedChanged += new System.EventHandler(this.chbAO_CheckedChanged);
            // 
            // frmSummarizeGraphSedimentByArivalTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 389);
            this.Controls.Add(this.chbAO);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.nudFlameLength);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbProject);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbInFeatureClass);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbScenerio);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSummarizeGraphSedimentByArivalTime";
            this.Text = "Graph Sediment By Arrival Time";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFlameLength)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbScenerio;
        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbInFeatureClass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbProject;
        private System.Windows.Forms.ComboBox cmbF10;
        private System.Windows.Forms.ComboBox cmbF50;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbT50;
        private System.Windows.Forms.ComboBox cmbT10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbHSF50;
        private System.Windows.Forms.ComboBox cmbHSF10;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chbIntermidiate;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nudFlameLength;
        private System.Windows.Forms.CheckBox chbCore;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chbAO;
    }
}