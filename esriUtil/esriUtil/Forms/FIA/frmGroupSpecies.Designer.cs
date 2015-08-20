namespace esriUtil.Forms.FIA
{
    partial class frmGroupSpecies
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGroupSpecies));
            this.dgvSp = new System.Windows.Forms.DataGridView();
            this.clmGrp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmSp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbl1 = new System.Windows.Forms.Label();
            this.lstSpecies = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnAddGroup = new System.Windows.Forms.Button();
            this.btnMin = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.chbStatusCode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSp)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSp
            // 
            this.dgvSp.AllowUserToAddRows = false;
            this.dgvSp.AllowUserToDeleteRows = false;
            this.dgvSp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSp.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmGrp,
            this.clmSp});
            this.dgvSp.Location = new System.Drawing.Point(12, 171);
            this.dgvSp.MultiSelect = false;
            this.dgvSp.Name = "dgvSp";
            this.dgvSp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSp.Size = new System.Drawing.Size(372, 160);
            this.dgvSp.TabIndex = 0;
            // 
            // clmGrp
            // 
            this.clmGrp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmGrp.HeaderText = "Group";
            this.clmGrp.Name = "clmGrp";
            this.clmGrp.Width = 61;
            // 
            // clmSp
            // 
            this.clmSp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.clmSp.HeaderText = "Species";
            this.clmSp.Name = "clmSp";
            this.clmSp.Width = 70;
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Location = new System.Drawing.Point(13, 20);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(60, 13);
            this.lbl1.TabIndex = 2;
            this.lbl1.Text = "Species list";
            // 
            // lstSpecies
            // 
            this.lstSpecies.FormattingEnabled = true;
            this.lstSpecies.Location = new System.Drawing.Point(12, 37);
            this.lstSpecies.Name = "lstSpecies";
            this.lstSpecies.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSpecies.Size = new System.Drawing.Size(372, 95);
            this.lstSpecies.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(243, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Select species for each group from the species list";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(294, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 24);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Add To Group";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAddGroup
            // 
            this.btnAddGroup.Location = new System.Drawing.Point(336, 147);
            this.btnAddGroup.Name = "btnAddGroup";
            this.btnAddGroup.Size = new System.Drawing.Size(23, 23);
            this.btnAddGroup.TabIndex = 6;
            this.btnAddGroup.Text = "+";
            this.btnAddGroup.UseVisualStyleBackColor = true;
            this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
            // 
            // btnMin
            // 
            this.btnMin.Location = new System.Drawing.Point(362, 147);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(23, 23);
            this.btnMin.TabIndex = 7;
            this.btnMin.Text = "-";
            this.btnMin.UseVisualStyleBackColor = true;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(339, 335);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(45, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(288, 335);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(45, 23);
            this.btnLoad.TabIndex = 9;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // chbStatusCode
            // 
            this.chbStatusCode.AutoSize = true;
            this.chbStatusCode.Location = new System.Drawing.Point(13, 340);
            this.chbStatusCode.Name = "chbStatusCode";
            this.chbStatusCode.Size = new System.Drawing.Size(210, 17);
            this.chbStatusCode.TabIndex = 10;
            this.chbStatusCode.Text = "Group By Status Code (Live and Dead)";
            this.chbStatusCode.UseVisualStyleBackColor = true;
            // 
            // frmGroupSpecies
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 365);
            this.Controls.Add(this.chbStatusCode);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnMin);
            this.Controls.Add(this.btnAddGroup);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstSpecies);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.dgvSp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmGroupSpecies";
            this.Text = "Group Species";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGroupSpecies_FormClosing);
            this.Shown += new System.EventHandler(this.frmGroupSpecies_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSp;
        private System.Windows.Forms.Label lbl1;
        public System.Windows.Forms.ListBox lstSpecies;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnAddGroup;
        private System.Windows.Forms.Button btnMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmGrp;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSp;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        public System.Windows.Forms.CheckBox chbStatusCode;
    }
}