namespace esriUtil.Forms.SasProcedures
{
    partial class frmRunPolytomousLogisticRegression
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRunPolytomousLogisticRegression));
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbRasterBands = new System.Windows.Forms.ComboBox();
            this.lstRasterBands = new System.Windows.Forms.ListBox();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnViewOrder = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnRasterBands = new System.Windows.Forms.Button();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Training Dataset";
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(20, 27);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(220, 21);
            this.cmbSampleFeatureClass.TabIndex = 22;
            this.cmbSampleFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Rasters (all bands will be used)";
            // 
            // cmbRasterBands
            // 
            this.cmbRasterBands.FormattingEnabled = true;
            this.cmbRasterBands.Location = new System.Drawing.Point(19, 73);
            this.cmbRasterBands.Name = "cmbRasterBands";
            this.cmbRasterBands.Size = new System.Drawing.Size(220, 21);
            this.cmbRasterBands.TabIndex = 27;
            this.cmbRasterBands.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // lstRasterBands
            // 
            this.lstRasterBands.FormattingEnabled = true;
            this.lstRasterBands.Location = new System.Drawing.Point(19, 101);
            this.lstRasterBands.Name = "lstRasterBands";
            this.lstRasterBands.Size = new System.Drawing.Size(220, 160);
            this.lstRasterBands.TabIndex = 30;
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(245, 161);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveAll.TabIndex = 34;
            this.btnRemoveAll.Text = "!";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(245, 103);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(25, 23);
            this.btnMinus.TabIndex = 33;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(245, 132);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(25, 23);
            this.btnAddAll.TabIndex = 32;
            this.btnAddAll.Text = "%";
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnViewOrder
            // 
            this.btnViewOrder.Location = new System.Drawing.Point(20, 270);
            this.btnViewOrder.Name = "btnViewOrder";
            this.btnViewOrder.Size = new System.Drawing.Size(95, 23);
            this.btnViewOrder.TabIndex = 35;
            this.btnViewOrder.Text = "Order of Rasters";
            this.btnViewOrder.UseVisualStyleBackColor = true;
            this.btnViewOrder.Click += new System.EventHandler(this.btnViewOrder_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(211, 270);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(59, 23);
            this.button1.TabIndex = 36;
            this.button1.Text = "Create";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnRasterBands
            // 
            this.btnRasterBands.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnRasterBands.Location = new System.Drawing.Point(240, 69);
            this.btnRasterBands.Name = "btnRasterBands";
            this.btnRasterBands.Size = new System.Drawing.Size(34, 27);
            this.btnRasterBands.TabIndex = 29;
            this.btnRasterBands.UseVisualStyleBackColor = true;
            this.btnRasterBands.Click += new System.EventHandler(this.btnRasterBands_Click);
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(241, 23);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(34, 27);
            this.btnOpenFeatureClass.TabIndex = 24;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeature_Click);
            // 
            // frmRunPolytomousLogisticRegression
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 308);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnViewOrder);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.btnAddAll);
            this.Controls.Add(this.lstRasterBands);
            this.Controls.Add(this.btnRasterBands);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbRasterBands);
            this.Controls.Add(this.btnOpenFeatureClass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSampleFeatureClass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRunPolytomousLogisticRegression";
            this.Text = "Create LR \\ PLR Surfaces";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
        private System.Windows.Forms.Button btnRasterBands;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbRasterBands;
        private System.Windows.Forms.ListBox lstRasterBands;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnViewOrder;
        private System.Windows.Forms.Button button1;
    }
}