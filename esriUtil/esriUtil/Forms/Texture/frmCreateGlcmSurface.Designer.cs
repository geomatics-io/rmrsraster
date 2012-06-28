namespace esriUtil.Forms.Texture
{
    public partial class frmCreateGlcmSurface
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCreateGlcmSurface));
            this.btnOpenRaster = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbRaster = new System.Windows.Forms.ComboBox();
            this.gpbWindowType = new System.Windows.Forms.GroupBox();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.lblGlcmType = new System.Windows.Forms.Label();
            this.cmbGlcmTypes = new System.Windows.Forms.ComboBox();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.lsbGlcmType = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbDirections = new System.Windows.Forms.ComboBox();
            this.lblRows = new System.Windows.Forms.Label();
            this.cmbWindowType = new System.Windows.Forms.ComboBox();
            this.nudRows = new System.Windows.Forms.NumericUpDown();
            this.lblColumns = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudColumns = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.mtbOutName = new System.Windows.Forms.TextBox();
            this.gpbWindowType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumns)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOpenRaster
            // 
            this.btnOpenRaster.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRaster.Location = new System.Drawing.Point(248, 27);
            this.btnOpenRaster.Name = "btnOpenRaster";
            this.btnOpenRaster.Size = new System.Drawing.Size(25, 27);
            this.btnOpenRaster.TabIndex = 19;
            this.btnOpenRaster.UseVisualStyleBackColor = true;
            this.btnOpenRaster.Click += new System.EventHandler(this.btnOpenRaster_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(220, 474);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(57, 23);
            this.btnCreate.TabIndex = 18;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Raster";
            // 
            // cmbRaster
            // 
            this.cmbRaster.FormattingEnabled = true;
            this.cmbRaster.Location = new System.Drawing.Point(22, 30);
            this.cmbRaster.Name = "cmbRaster";
            this.cmbRaster.Size = new System.Drawing.Size(220, 21);
            this.cmbRaster.TabIndex = 15;
            // 
            // gpbWindowType
            // 
            this.gpbWindowType.Controls.Add(this.btnRemoveAll);
            this.gpbWindowType.Controls.Add(this.lblGlcmType);
            this.gpbWindowType.Controls.Add(this.cmbGlcmTypes);
            this.gpbWindowType.Controls.Add(this.btnMinus);
            this.gpbWindowType.Controls.Add(this.btnAddAll);
            this.gpbWindowType.Controls.Add(this.lsbGlcmType);
            this.gpbWindowType.Controls.Add(this.label4);
            this.gpbWindowType.Controls.Add(this.cmbDirections);
            this.gpbWindowType.Controls.Add(this.lblRows);
            this.gpbWindowType.Controls.Add(this.cmbWindowType);
            this.gpbWindowType.Controls.Add(this.nudRows);
            this.gpbWindowType.Controls.Add(this.lblColumns);
            this.gpbWindowType.Controls.Add(this.label3);
            this.gpbWindowType.Controls.Add(this.nudColumns);
            this.gpbWindowType.Location = new System.Drawing.Point(13, 121);
            this.gpbWindowType.Name = "gpbWindowType";
            this.gpbWindowType.Size = new System.Drawing.Size(267, 343);
            this.gpbWindowType.TabIndex = 17;
            this.gpbWindowType.TabStop = false;
            this.gpbWindowType.Text = "GLCM window";
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(235, 216);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveAll.TabIndex = 20;
            this.btnRemoveAll.Text = "!";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // lblGlcmType
            // 
            this.lblGlcmType.AutoSize = true;
            this.lblGlcmType.Location = new System.Drawing.Point(9, 111);
            this.lblGlcmType.Name = "lblGlcmType";
            this.lblGlcmType.Size = new System.Drawing.Size(36, 13);
            this.lblGlcmType.TabIndex = 19;
            this.lblGlcmType.Text = "Types";
            // 
            // cmbGlcmTypes
            // 
            this.cmbGlcmTypes.FormattingEnabled = true;
            this.cmbGlcmTypes.Location = new System.Drawing.Point(9, 129);
            this.cmbGlcmTypes.Name = "cmbGlcmTypes";
            this.cmbGlcmTypes.Size = new System.Drawing.Size(220, 21);
            this.cmbGlcmTypes.TabIndex = 18;
            this.cmbGlcmTypes.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(235, 158);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(25, 23);
            this.btnMinus.TabIndex = 17;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(235, 187);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(25, 23);
            this.btnAddAll.TabIndex = 16;
            this.btnAddAll.Text = "%";
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // lsbGlcmType
            // 
            this.lsbGlcmType.FormattingEnabled = true;
            this.lsbGlcmType.Location = new System.Drawing.Point(9, 156);
            this.lsbGlcmType.Name = "lsbGlcmType";
            this.lsbGlcmType.Size = new System.Drawing.Size(220, 173);
            this.lsbGlcmType.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(124, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Direction";
            // 
            // cmbDirections
            // 
            this.cmbDirections.FormattingEnabled = true;
            this.cmbDirections.Items.AddRange(new object[] {
            "Horizontal",
            "Vertical"});
            this.cmbDirections.Location = new System.Drawing.Point(127, 83);
            this.cmbDirections.Name = "cmbDirections";
            this.cmbDirections.Size = new System.Drawing.Size(121, 21);
            this.cmbDirections.TabIndex = 11;
            this.cmbDirections.Text = "HORIZONTAL";
            // 
            // lblRows
            // 
            this.lblRows.AutoSize = true;
            this.lblRows.Location = new System.Drawing.Point(67, 66);
            this.lblRows.Name = "lblRows";
            this.lblRows.Size = new System.Drawing.Size(34, 13);
            this.lblRows.TabIndex = 10;
            this.lblRows.Text = "Rows";
            // 
            // cmbWindowType
            // 
            this.cmbWindowType.FormattingEnabled = true;
            this.cmbWindowType.Location = new System.Drawing.Point(62, 38);
            this.cmbWindowType.Name = "cmbWindowType";
            this.cmbWindowType.Size = new System.Drawing.Size(121, 21);
            this.cmbWindowType.TabIndex = 5;
            this.cmbWindowType.Text = "RECTANGLE";
            this.cmbWindowType.SelectedIndexChanged += new System.EventHandler(this.cmbWindowType_SelectedIndexChanged);
            // 
            // nudRows
            // 
            this.nudRows.Location = new System.Drawing.Point(70, 84);
            this.nudRows.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRows.Name = "nudRows";
            this.nudRows.Size = new System.Drawing.Size(46, 20);
            this.nudRows.TabIndex = 8;
            this.nudRows.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblColumns
            // 
            this.lblColumns.AutoSize = true;
            this.lblColumns.Location = new System.Drawing.Point(7, 66);
            this.lblColumns.Name = "lblColumns";
            this.lblColumns.Size = new System.Drawing.Size(47, 13);
            this.lblColumns.TabIndex = 9;
            this.lblColumns.Text = "Columns";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(80, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Window Type";
            // 
            // nudColumns
            // 
            this.nudColumns.Location = new System.Drawing.Point(10, 84);
            this.nudColumns.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudColumns.Name = "nudColumns";
            this.nudColumns.Size = new System.Drawing.Size(46, 20);
            this.nudColumns.TabIndex = 7;
            this.nudColumns.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Output Name";
            // 
            // mtbOutName
            // 
            this.mtbOutName.Location = new System.Drawing.Point(22, 77);
            this.mtbOutName.Name = "mtbOutName";
            this.mtbOutName.Size = new System.Drawing.Size(144, 20);
            this.mtbOutName.TabIndex = 22;
            // 
            // frmCreateGlcmSurface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 504);
            this.Controls.Add(this.mtbOutName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOpenRaster);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbRaster);
            this.Controls.Add(this.gpbWindowType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCreateGlcmSurface";
            this.Text = "Create GLCM Surfaces";
            this.TopMost = true;
            this.gpbWindowType.ResumeLayout(false);
            this.gpbWindowType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumns)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenRaster;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRaster;
        private System.Windows.Forms.GroupBox gpbWindowType;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Label lblGlcmType;
        private System.Windows.Forms.ComboBox cmbGlcmTypes;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.ListBox lsbGlcmType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbDirections;
        private System.Windows.Forms.Label lblRows;
        private System.Windows.Forms.ComboBox cmbWindowType;
        private System.Windows.Forms.NumericUpDown nudRows;
        private System.Windows.Forms.Label lblColumns;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudColumns;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mtbOutName;
    }
}