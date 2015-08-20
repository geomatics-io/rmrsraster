namespace esriUtil.Forms.Stats
{
    partial class frmSummarizeByField
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSummarizeByField));
            this.btnClearFields = new System.Windows.Forms.Button();
            this.btnAllFields = new System.Windows.Forms.Button();
            this.btnMinusFields = new System.Windows.Forms.Button();
            this.lsbFields = new System.Windows.Forms.ListBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.cmbFields = new System.Windows.Forms.ComboBox();
            this.btnOpenTable = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTable = new System.Windows.Forms.ComboBox();
            this.btnClearStats = new System.Windows.Forms.Button();
            this.btnAllStats = new System.Windows.Forms.Button();
            this.btnMinusStats = new System.Windows.Forms.Button();
            this.lsbStats = new System.Windows.Forms.ListBox();
            this.cmbStats = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnQry = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClearFields
            // 
            this.btnClearFields.Location = new System.Drawing.Point(225, 158);
            this.btnClearFields.Name = "btnClearFields";
            this.btnClearFields.Size = new System.Drawing.Size(27, 23);
            this.btnClearFields.TabIndex = 85;
            this.btnClearFields.Text = "%";
            this.btnClearFields.UseVisualStyleBackColor = true;
            this.btnClearFields.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAllFields
            // 
            this.btnAllFields.Location = new System.Drawing.Point(225, 129);
            this.btnAllFields.Name = "btnAllFields";
            this.btnAllFields.Size = new System.Drawing.Size(27, 23);
            this.btnAllFields.TabIndex = 84;
            this.btnAllFields.Text = "!";
            this.btnAllFields.UseVisualStyleBackColor = true;
            this.btnAllFields.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnMinusFields
            // 
            this.btnMinusFields.Location = new System.Drawing.Point(225, 103);
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
            this.lsbFields.Location = new System.Drawing.Point(9, 102);
            this.lsbFields.Name = "lsbFields";
            this.lsbFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbFields.Size = new System.Drawing.Size(211, 108);
            this.lsbFields.TabIndex = 82;
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(183, 378);
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
            this.cmbFields.Location = new System.Drawing.Point(7, 74);
            this.cmbFields.Name = "cmbFields";
            this.cmbFields.Size = new System.Drawing.Size(213, 21);
            this.cmbFields.TabIndex = 73;
            this.cmbFields.SelectedIndexChanged += new System.EventHandler(this.cmb_SelectedIndexChanged);
            // 
            // btnOpenTable
            // 
            this.btnOpenTable.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenTable.Location = new System.Drawing.Point(233, 20);
            this.btnOpenTable.Name = "btnOpenTable";
            this.btnOpenTable.Size = new System.Drawing.Size(25, 27);
            this.btnOpenTable.TabIndex = 77;
            this.btnOpenTable.UseVisualStyleBackColor = true;
            this.btnOpenTable.Click += new System.EventHandler(this.btnOpenTable_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 74;
            this.label4.Text = "Fields To Summarize";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 76;
            this.label1.Text = "Table";
            // 
            // cmbTable
            // 
            this.cmbTable.FormattingEnabled = true;
            this.cmbTable.Location = new System.Drawing.Point(7, 23);
            this.cmbTable.Name = "cmbTable";
            this.cmbTable.Size = new System.Drawing.Size(220, 21);
            this.cmbTable.TabIndex = 75;
            this.cmbTable.SelectedIndexChanged += new System.EventHandler(this.cmbTable_SelectedIndexChanged);
            // 
            // btnClearStats
            // 
            this.btnClearStats.Location = new System.Drawing.Point(226, 320);
            this.btnClearStats.Name = "btnClearStats";
            this.btnClearStats.Size = new System.Drawing.Size(27, 23);
            this.btnClearStats.TabIndex = 91;
            this.btnClearStats.Text = "%";
            this.btnClearStats.UseVisualStyleBackColor = true;
            this.btnClearStats.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAllStats
            // 
            this.btnAllStats.Location = new System.Drawing.Point(226, 291);
            this.btnAllStats.Name = "btnAllStats";
            this.btnAllStats.Size = new System.Drawing.Size(27, 23);
            this.btnAllStats.TabIndex = 90;
            this.btnAllStats.Text = "!";
            this.btnAllStats.UseVisualStyleBackColor = true;
            this.btnAllStats.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnMinusStats
            // 
            this.btnMinusStats.Location = new System.Drawing.Point(226, 265);
            this.btnMinusStats.Name = "btnMinusStats";
            this.btnMinusStats.Size = new System.Drawing.Size(27, 23);
            this.btnMinusStats.TabIndex = 89;
            this.btnMinusStats.Text = "-";
            this.btnMinusStats.UseVisualStyleBackColor = true;
            this.btnMinusStats.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // lsbStats
            // 
            this.lsbStats.FormattingEnabled = true;
            this.lsbStats.Location = new System.Drawing.Point(10, 264);
            this.lsbStats.Name = "lsbStats";
            this.lsbStats.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbStats.Size = new System.Drawing.Size(211, 108);
            this.lsbStats.TabIndex = 88;
            // 
            // cmbStats
            // 
            this.cmbStats.FormattingEnabled = true;
            this.cmbStats.Location = new System.Drawing.Point(8, 236);
            this.cmbStats.Name = "cmbStats";
            this.cmbStats.Size = new System.Drawing.Size(213, 21);
            this.cmbStats.TabIndex = 86;
            this.cmbStats.SelectedIndexChanged += new System.EventHandler(this.cmb_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 217);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 87;
            this.label2.Text = "Sumary Statistics";
            // 
            // btnQry
            // 
            this.btnQry.Image = ((System.Drawing.Image)(resources.GetObject("btnQry.Image")));
            this.btnQry.Location = new System.Drawing.Point(232, 49);
            this.btnQry.Name = "btnQry";
            this.btnQry.Size = new System.Drawing.Size(26, 25);
            this.btnQry.TabIndex = 92;
            this.btnQry.UseVisualStyleBackColor = true;
            this.btnQry.Click += new System.EventHandler(this.btnQry_Click);
            // 
            // frmSummarizeByField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 405);
            this.Controls.Add(this.btnQry);
            this.Controls.Add(this.btnClearStats);
            this.Controls.Add(this.btnAllStats);
            this.Controls.Add(this.btnMinusStats);
            this.Controls.Add(this.lsbStats);
            this.Controls.Add(this.cmbStats);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnClearFields);
            this.Controls.Add(this.btnAllFields);
            this.Controls.Add(this.btnMinusFields);
            this.Controls.Add(this.lsbFields);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.cmbFields);
            this.Controls.Add(this.btnOpenTable);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSummarizeByField";
            this.Text = "Summarize Values by Fields";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClearFields;
        private System.Windows.Forms.Button btnAllFields;
        private System.Windows.Forms.Button btnMinusFields;
        private System.Windows.Forms.ListBox lsbFields;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.ComboBox cmbFields;
        private System.Windows.Forms.Button btnOpenTable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTable;
        private System.Windows.Forms.Button btnClearStats;
        private System.Windows.Forms.Button btnAllStats;
        private System.Windows.Forms.Button btnMinusStats;
        private System.Windows.Forms.ListBox lsbStats;
        private System.Windows.Forms.ComboBox cmbStats;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnQry;
    }
}