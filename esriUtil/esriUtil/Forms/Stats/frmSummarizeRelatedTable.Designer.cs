namespace esriUtil.Forms.Stats
{
    partial class frmSummarizeRelatedTable
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSummarizeRelatedTable));
            this.btnAllFields = new System.Windows.Forms.Button();
            this.btnClearFields = new System.Windows.Forms.Button();
            this.btnMinusFields = new System.Windows.Forms.Button();
            this.lsbFields = new System.Windows.Forms.ListBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.cmbFields = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbParent = new System.Windows.Forms.ComboBox();
            this.btnParentOpen = new System.Windows.Forms.Button();
            this.btnChildOpen = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbChild = new System.Windows.Forms.ComboBox();
            this.cmbPLink = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbCLink = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnClearStats = new System.Windows.Forms.Button();
            this.btnAllStats = new System.Windows.Forms.Button();
            this.btnMinusStats = new System.Windows.Forms.Button();
            this.lsbStats = new System.Windows.Forms.ListBox();
            this.cmbStats = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClearGroup = new System.Windows.Forms.Button();
            this.btnAllGroup = new System.Windows.Forms.Button();
            this.btnMinusGroup = new System.Windows.Forms.Button();
            this.lsbGroup = new System.Windows.Forms.ListBox();
            this.cmbGroup = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnQryParent = new System.Windows.Forms.Button();
            this.btnQueryChild = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAllFields
            // 
            this.btnAllFields.Location = new System.Drawing.Point(229, 309);
            this.btnAllFields.Name = "btnAllFields";
            this.btnAllFields.Size = new System.Drawing.Size(27, 23);
            this.btnAllFields.TabIndex = 85;
            this.btnAllFields.Text = "%";
            this.btnAllFields.UseVisualStyleBackColor = true;
            this.btnAllFields.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnClearFields
            // 
            this.btnClearFields.Location = new System.Drawing.Point(229, 280);
            this.btnClearFields.Name = "btnClearFields";
            this.btnClearFields.Size = new System.Drawing.Size(27, 23);
            this.btnClearFields.TabIndex = 84;
            this.btnClearFields.Text = "!";
            this.btnClearFields.UseVisualStyleBackColor = true;
            this.btnClearFields.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnMinusFields
            // 
            this.btnMinusFields.Location = new System.Drawing.Point(229, 254);
            this.btnMinusFields.Name = "btnMinusFields";
            this.btnMinusFields.Size = new System.Drawing.Size(27, 23);
            this.btnMinusFields.TabIndex = 83;
            this.btnMinusFields.Text = "-";
            this.btnMinusFields.UseVisualStyleBackColor = true;
            this.btnMinusFields.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // lsbFields
            // 
            this.lsbFields.FormattingEnabled = true;
            this.lsbFields.Location = new System.Drawing.Point(13, 253);
            this.lsbFields.Name = "lsbFields";
            this.lsbFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbFields.Size = new System.Drawing.Size(211, 108);
            this.lsbFields.TabIndex = 82;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(446, 367);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 81;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // cmbFields
            // 
            this.cmbFields.FormattingEnabled = true;
            this.cmbFields.Location = new System.Drawing.Point(11, 225);
            this.cmbFields.Name = "cmbFields";
            this.cmbFields.Size = new System.Drawing.Size(213, 21);
            this.cmbFields.TabIndex = 73;
            this.cmbFields.SelectedIndexChanged += new System.EventHandler(this.cmb_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 74;
            this.label4.Text = "Fields To Summarize";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 76;
            this.label1.Text = "Parent";
            // 
            // cmbParent
            // 
            this.cmbParent.FormattingEnabled = true;
            this.cmbParent.Location = new System.Drawing.Point(10, 31);
            this.cmbParent.Name = "cmbParent";
            this.cmbParent.Size = new System.Drawing.Size(220, 21);
            this.cmbParent.TabIndex = 75;
            this.cmbParent.SelectedIndexChanged += new System.EventHandler(this.cmbTable_SelectedIndexChanged);
            // 
            // btnParentOpen
            // 
            this.btnParentOpen.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnParentOpen.Location = new System.Drawing.Point(233, 27);
            this.btnParentOpen.Name = "btnParentOpen";
            this.btnParentOpen.Size = new System.Drawing.Size(25, 27);
            this.btnParentOpen.TabIndex = 77;
            this.btnParentOpen.UseVisualStyleBackColor = true;
            this.btnParentOpen.Click += new System.EventHandler(this.btnOpenTable_Click);
            // 
            // btnChildOpen
            // 
            this.btnChildOpen.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnChildOpen.Location = new System.Drawing.Point(233, 115);
            this.btnChildOpen.Name = "btnChildOpen";
            this.btnChildOpen.Size = new System.Drawing.Size(25, 27);
            this.btnChildOpen.TabIndex = 91;
            this.btnChildOpen.UseVisualStyleBackColor = true;
            this.btnChildOpen.Click += new System.EventHandler(this.btnOpenTable_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 90;
            this.label5.Text = "Child";
            // 
            // cmbChild
            // 
            this.cmbChild.FormattingEnabled = true;
            this.cmbChild.Location = new System.Drawing.Point(10, 119);
            this.cmbChild.Name = "cmbChild";
            this.cmbChild.Size = new System.Drawing.Size(220, 21);
            this.cmbChild.TabIndex = 89;
            this.cmbChild.SelectedIndexChanged += new System.EventHandler(this.cmbTable_SelectedIndexChanged);
            // 
            // cmbPLink
            // 
            this.cmbPLink.FormattingEnabled = true;
            this.cmbPLink.Location = new System.Drawing.Point(10, 75);
            this.cmbPLink.Name = "cmbPLink";
            this.cmbPLink.Size = new System.Drawing.Size(213, 21);
            this.cmbPLink.TabIndex = 92;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 93;
            this.label6.Text = "Parent Link";
            // 
            // cmbCLink
            // 
            this.cmbCLink.FormattingEnabled = true;
            this.cmbCLink.Location = new System.Drawing.Point(10, 164);
            this.cmbCLink.Name = "cmbCLink";
            this.cmbCLink.Size = new System.Drawing.Size(213, 21);
            this.cmbCLink.TabIndex = 94;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 95;
            this.label7.Text = "Child Link";
            // 
            // btnClearStats
            // 
            this.btnClearStats.Location = new System.Drawing.Point(494, 309);
            this.btnClearStats.Name = "btnClearStats";
            this.btnClearStats.Size = new System.Drawing.Size(27, 23);
            this.btnClearStats.TabIndex = 101;
            this.btnClearStats.Text = "%";
            this.btnClearStats.UseVisualStyleBackColor = true;
            this.btnClearStats.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAllStats
            // 
            this.btnAllStats.Location = new System.Drawing.Point(494, 280);
            this.btnAllStats.Name = "btnAllStats";
            this.btnAllStats.Size = new System.Drawing.Size(27, 23);
            this.btnAllStats.TabIndex = 100;
            this.btnAllStats.Text = "!";
            this.btnAllStats.UseVisualStyleBackColor = true;
            this.btnAllStats.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnMinusStats
            // 
            this.btnMinusStats.Location = new System.Drawing.Point(494, 254);
            this.btnMinusStats.Name = "btnMinusStats";
            this.btnMinusStats.Size = new System.Drawing.Size(27, 23);
            this.btnMinusStats.TabIndex = 99;
            this.btnMinusStats.Text = "-";
            this.btnMinusStats.UseVisualStyleBackColor = true;
            this.btnMinusStats.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // lsbStats
            // 
            this.lsbStats.FormattingEnabled = true;
            this.lsbStats.Location = new System.Drawing.Point(278, 253);
            this.lsbStats.Name = "lsbStats";
            this.lsbStats.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbStats.Size = new System.Drawing.Size(211, 108);
            this.lsbStats.TabIndex = 98;
            // 
            // cmbStats
            // 
            this.cmbStats.FormattingEnabled = true;
            this.cmbStats.Location = new System.Drawing.Point(276, 225);
            this.cmbStats.Name = "cmbStats";
            this.cmbStats.Size = new System.Drawing.Size(213, 21);
            this.cmbStats.TabIndex = 96;
            this.cmbStats.SelectedIndexChanged += new System.EventHandler(this.cmb_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(279, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 97;
            this.label2.Text = "Summary Statistics";
            // 
            // btnClearGroup
            // 
            this.btnClearGroup.Location = new System.Drawing.Point(494, 115);
            this.btnClearGroup.Name = "btnClearGroup";
            this.btnClearGroup.Size = new System.Drawing.Size(27, 23);
            this.btnClearGroup.TabIndex = 108;
            this.btnClearGroup.Text = "%";
            this.btnClearGroup.UseVisualStyleBackColor = true;
            this.btnClearGroup.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAllGroup
            // 
            this.btnAllGroup.Location = new System.Drawing.Point(494, 86);
            this.btnAllGroup.Name = "btnAllGroup";
            this.btnAllGroup.Size = new System.Drawing.Size(27, 23);
            this.btnAllGroup.TabIndex = 107;
            this.btnAllGroup.Text = "!";
            this.btnAllGroup.UseVisualStyleBackColor = true;
            this.btnAllGroup.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnMinusGroup
            // 
            this.btnMinusGroup.Location = new System.Drawing.Point(494, 60);
            this.btnMinusGroup.Name = "btnMinusGroup";
            this.btnMinusGroup.Size = new System.Drawing.Size(27, 23);
            this.btnMinusGroup.TabIndex = 106;
            this.btnMinusGroup.Text = "-";
            this.btnMinusGroup.UseVisualStyleBackColor = true;
            this.btnMinusGroup.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // lsbGroup
            // 
            this.lsbGroup.FormattingEnabled = true;
            this.lsbGroup.Location = new System.Drawing.Point(278, 59);
            this.lsbGroup.Name = "lsbGroup";
            this.lsbGroup.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbGroup.Size = new System.Drawing.Size(211, 121);
            this.lsbGroup.TabIndex = 105;
            // 
            // cmbGroup
            // 
            this.cmbGroup.FormattingEnabled = true;
            this.cmbGroup.Location = new System.Drawing.Point(276, 31);
            this.cmbGroup.Name = "cmbGroup";
            this.cmbGroup.Size = new System.Drawing.Size(213, 21);
            this.cmbGroup.TabIndex = 103;
            this.cmbGroup.SelectedIndexChanged += new System.EventHandler(this.cmb_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(279, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 104;
            this.label3.Text = "Group Values By";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 184);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(523, 13);
            this.label8.TabIndex = 109;
            this.label8.Text = "_________________________________________________________________________________" +
    "_____";
            // 
            // btnQryParent
            // 
            this.btnQryParent.Image = ((System.Drawing.Image)(resources.GetObject("btnQryParent.Image")));
            this.btnQryParent.Location = new System.Drawing.Point(232, 56);
            this.btnQryParent.Name = "btnQryParent";
            this.btnQryParent.Size = new System.Drawing.Size(26, 25);
            this.btnQryParent.TabIndex = 110;
            this.btnQryParent.UseVisualStyleBackColor = true;
            this.btnQryParent.Click += new System.EventHandler(this.btnQryParent_Click);
            // 
            // btnQueryChild
            // 
            this.btnQueryChild.Image = ((System.Drawing.Image)(resources.GetObject("btnQueryChild.Image")));
            this.btnQueryChild.Location = new System.Drawing.Point(233, 145);
            this.btnQueryChild.Name = "btnQueryChild";
            this.btnQueryChild.Size = new System.Drawing.Size(26, 25);
            this.btnQueryChild.TabIndex = 111;
            this.btnQueryChild.UseVisualStyleBackColor = true;
            this.btnQueryChild.Click += new System.EventHandler(this.btnQueryChild_Click);
            // 
            // frmSummarizeRelatedTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 397);
            this.Controls.Add(this.btnQueryChild);
            this.Controls.Add(this.btnQryParent);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnClearGroup);
            this.Controls.Add(this.btnAllGroup);
            this.Controls.Add(this.btnMinusGroup);
            this.Controls.Add(this.lsbGroup);
            this.Controls.Add(this.cmbGroup);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnClearStats);
            this.Controls.Add(this.btnAllStats);
            this.Controls.Add(this.btnMinusStats);
            this.Controls.Add(this.lsbStats);
            this.Controls.Add(this.cmbStats);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbCLink);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cmbPLink);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnChildOpen);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbChild);
            this.Controls.Add(this.btnAllFields);
            this.Controls.Add(this.btnClearFields);
            this.Controls.Add(this.btnMinusFields);
            this.Controls.Add(this.lsbFields);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.cmbFields);
            this.Controls.Add(this.btnParentOpen);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbParent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSummarizeRelatedTable";
            this.Text = "Summarize Related Table";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAllFields;
        private System.Windows.Forms.Button btnClearFields;
        private System.Windows.Forms.Button btnMinusFields;
        private System.Windows.Forms.ListBox lsbFields;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.ComboBox cmbFields;
        private System.Windows.Forms.Button btnParentOpen;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbParent;
        private System.Windows.Forms.Button btnChildOpen;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbChild;
        private System.Windows.Forms.ComboBox cmbPLink;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbCLink;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnClearStats;
        private System.Windows.Forms.Button btnAllStats;
        private System.Windows.Forms.Button btnMinusStats;
        private System.Windows.Forms.ListBox lsbStats;
        private System.Windows.Forms.ComboBox cmbStats;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClearGroup;
        private System.Windows.Forms.Button btnAllGroup;
        private System.Windows.Forms.Button btnMinusGroup;
        private System.Windows.Forms.ListBox lsbGroup;
        private System.Windows.Forms.ComboBox cmbGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnQryParent;
        private System.Windows.Forms.Button btnQueryChild;
    }
}