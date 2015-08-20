namespace esriUtil.Forms.Stats
{
    partial class frmWeightedAverageByAreaOrLength
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWeightedAverageByAreaOrLength));
            this.btnStrata = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbStrata = new System.Windows.Forms.ComboBox();
            this.btnStands = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbStands = new System.Windows.Forms.ComboBox();
            this.btnClearFields = new System.Windows.Forms.Button();
            this.btnAllFields = new System.Windows.Forms.Button();
            this.btnMinusFields = new System.Windows.Forms.Button();
            this.lsbFields = new System.Windows.Forms.ListBox();
            this.cmbFields = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.chbLength = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnStrata
            // 
            this.btnStrata.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnStrata.Location = new System.Drawing.Point(238, 34);
            this.btnStrata.Name = "btnStrata";
            this.btnStrata.Size = new System.Drawing.Size(25, 27);
            this.btnStrata.TabIndex = 96;
            this.btnStrata.UseVisualStyleBackColor = true;
            this.btnStrata.Click += new System.EventHandler(this.btnOpenTable_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 95;
            this.label1.Text = "Value Polygon layer";
            // 
            // cmbStrata
            // 
            this.cmbStrata.FormattingEnabled = true;
            this.cmbStrata.Location = new System.Drawing.Point(12, 37);
            this.cmbStrata.Name = "cmbStrata";
            this.cmbStrata.Size = new System.Drawing.Size(220, 21);
            this.cmbStrata.TabIndex = 94;
            this.cmbStrata.SelectedIndexChanged += new System.EventHandler(this.cmbTable_SelectedIndexChanged);
            // 
            // btnStands
            // 
            this.btnStands.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnStands.Location = new System.Drawing.Point(238, 248);
            this.btnStands.Name = "btnStands";
            this.btnStands.Size = new System.Drawing.Size(25, 27);
            this.btnStands.TabIndex = 101;
            this.btnStands.UseVisualStyleBackColor = true;
            this.btnStands.Click += new System.EventHandler(this.btnOpenTable_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 233);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 100;
            this.label2.Text = "Zone Polygon Layer";
            // 
            // cmbStands
            // 
            this.cmbStands.FormattingEnabled = true;
            this.cmbStands.Location = new System.Drawing.Point(12, 251);
            this.cmbStands.Name = "cmbStands";
            this.cmbStands.Size = new System.Drawing.Size(220, 21);
            this.cmbStands.TabIndex = 99;
            // 
            // btnClearFields
            // 
            this.btnClearFields.Location = new System.Drawing.Point(233, 164);
            this.btnClearFields.Name = "btnClearFields";
            this.btnClearFields.Size = new System.Drawing.Size(27, 23);
            this.btnClearFields.TabIndex = 108;
            this.btnClearFields.Text = "%";
            this.btnClearFields.UseVisualStyleBackColor = true;
            this.btnClearFields.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAllFields
            // 
            this.btnAllFields.Location = new System.Drawing.Point(233, 135);
            this.btnAllFields.Name = "btnAllFields";
            this.btnAllFields.Size = new System.Drawing.Size(27, 23);
            this.btnAllFields.TabIndex = 107;
            this.btnAllFields.Text = "!";
            this.btnAllFields.UseVisualStyleBackColor = true;
            this.btnAllFields.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnMinusFields
            // 
            this.btnMinusFields.Location = new System.Drawing.Point(233, 109);
            this.btnMinusFields.Name = "btnMinusFields";
            this.btnMinusFields.Size = new System.Drawing.Size(27, 23);
            this.btnMinusFields.TabIndex = 106;
            this.btnMinusFields.Text = "-";
            this.btnMinusFields.UseVisualStyleBackColor = true;
            this.btnMinusFields.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // lsbFields
            // 
            this.lsbFields.FormattingEnabled = true;
            this.lsbFields.Location = new System.Drawing.Point(17, 108);
            this.lsbFields.Name = "lsbFields";
            this.lsbFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbFields.Size = new System.Drawing.Size(211, 108);
            this.lsbFields.TabIndex = 105;
            // 
            // cmbFields
            // 
            this.cmbFields.FormattingEnabled = true;
            this.cmbFields.Location = new System.Drawing.Point(15, 80);
            this.cmbFields.Name = "cmbFields";
            this.cmbFields.Size = new System.Drawing.Size(213, 21);
            this.cmbFields.TabIndex = 103;
            this.cmbFields.SelectedIndexChanged += new System.EventHandler(this.cmb_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 104;
            this.label4.Text = "Fields";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(185, 291);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 109;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // chbLength
            // 
            this.chbLength.AutoSize = true;
            this.chbLength.Location = new System.Drawing.Point(12, 295);
            this.chbLength.Name = "chbLength";
            this.chbLength.Size = new System.Drawing.Size(59, 17);
            this.chbLength.TabIndex = 110;
            this.chbLength.Text = "Length";
            this.chbLength.UseVisualStyleBackColor = true;
            // 
            // frmWeightedAverageByAreaOrLength
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 325);
            this.Controls.Add(this.chbLength);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnClearFields);
            this.Controls.Add(this.btnAllFields);
            this.Controls.Add(this.btnMinusFields);
            this.Controls.Add(this.lsbFields);
            this.Controls.Add(this.cmbFields);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnStands);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbStands);
            this.Controls.Add(this.btnStrata);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbStrata);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmWeightedAverageByAreaOrLength";
            this.Text = "Weighted Average by Area or Length";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStrata;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbStrata;
        private System.Windows.Forms.Button btnStands;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbStands;
        private System.Windows.Forms.Button btnClearFields;
        private System.Windows.Forms.Button btnAllFields;
        private System.Windows.Forms.Button btnMinusFields;
        private System.Windows.Forms.ListBox lsbFields;
        private System.Windows.Forms.ComboBox cmbFields;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.CheckBox chbLength;
    }
}