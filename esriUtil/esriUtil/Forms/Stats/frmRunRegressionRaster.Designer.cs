namespace esriUtil.Forms.Stats
{
    partial class frmRunRegressionRaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRunRegressionRaster));
            this.button1 = new System.Windows.Forms.Button();
            this.btnViewOrder = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.lstRasterBands = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbRasterBands = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSampleFeatureClass = new System.Windows.Forms.ComboBox();
            this.btnRasterBands = new System.Windows.Forms.Button();
            this.btnOpenFeatureClass = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbModelDir = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(203, 310);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(59, 23);
            this.button1.TabIndex = 49;
            this.button1.Text = "Create";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnViewOrder
            // 
            this.btnViewOrder.Location = new System.Drawing.Point(12, 310);
            this.btnViewOrder.Name = "btnViewOrder";
            this.btnViewOrder.Size = new System.Drawing.Size(44, 23);
            this.btnViewOrder.TabIndex = 48;
            this.btnViewOrder.Text = "Order";
            this.btnViewOrder.UseVisualStyleBackColor = true;
            this.btnViewOrder.Click += new System.EventHandler(this.btnViewOrder_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(237, 201);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(25, 23);
            this.btnRemoveAll.TabIndex = 47;
            this.btnRemoveAll.Text = "!";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Location = new System.Drawing.Point(237, 143);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(25, 23);
            this.btnMinus.TabIndex = 46;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnMinus_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(237, 172);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(25, 23);
            this.btnAddAll.TabIndex = 45;
            this.btnAddAll.Text = "%";
            this.btnAddAll.UseVisualStyleBackColor = true;
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // lstRasterBands
            // 
            this.lstRasterBands.FormattingEnabled = true;
            this.lstRasterBands.Location = new System.Drawing.Point(11, 141);
            this.lstRasterBands.Name = "lstRasterBands";
            this.lstRasterBands.Size = new System.Drawing.Size(220, 160);
            this.lstRasterBands.TabIndex = 43;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 13);
            this.label3.TabIndex = 41;
            this.label3.Text = "Rasters (all bands will be used)";
            // 
            // cmbRasterBands
            // 
            this.cmbRasterBands.FormattingEnabled = true;
            this.cmbRasterBands.Location = new System.Drawing.Point(11, 113);
            this.cmbRasterBands.Name = "cmbRasterBands";
            this.cmbRasterBands.Size = new System.Drawing.Size(220, 21);
            this.cmbRasterBands.TabIndex = 40;
            this.cmbRasterBands.SelectedIndexChanged += new System.EventHandler(this.btnPlus_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Training Dataset Workspace";
            // 
            // cmbSampleFeatureClass
            // 
            this.cmbSampleFeatureClass.FormattingEnabled = true;
            this.cmbSampleFeatureClass.Location = new System.Drawing.Point(12, 25);
            this.cmbSampleFeatureClass.Name = "cmbSampleFeatureClass";
            this.cmbSampleFeatureClass.Size = new System.Drawing.Size(220, 21);
            this.cmbSampleFeatureClass.TabIndex = 37;
            this.cmbSampleFeatureClass.SelectedIndexChanged += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            this.cmbSampleFeatureClass.Click += new System.EventHandler(this.cmbSampleFeatureClass_SelectedIndexChanged);
            // 
            // btnRasterBands
            // 
            this.btnRasterBands.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnRasterBands.Location = new System.Drawing.Point(232, 111);
            this.btnRasterBands.Name = "btnRasterBands";
            this.btnRasterBands.Size = new System.Drawing.Size(34, 27);
            this.btnRasterBands.TabIndex = 42;
            this.btnRasterBands.UseVisualStyleBackColor = true;
            this.btnRasterBands.Click += new System.EventHandler(this.btnRasterBands_Click);
            // 
            // btnOpenFeatureClass
            // 
            this.btnOpenFeatureClass.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenFeatureClass.Location = new System.Drawing.Point(233, 23);
            this.btnOpenFeatureClass.Name = "btnOpenFeatureClass";
            this.btnOpenFeatureClass.Size = new System.Drawing.Size(34, 27);
            this.btnOpenFeatureClass.TabIndex = 39;
            this.btnOpenFeatureClass.UseVisualStyleBackColor = true;
            this.btnOpenFeatureClass.Click += new System.EventHandler(this.btnOpenFeature_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 51;
            this.label1.Text = "Regression Model Directory";
            // 
            // cmbModelDir
            // 
            this.cmbModelDir.FormattingEnabled = true;
            this.cmbModelDir.Location = new System.Drawing.Point(13, 68);
            this.cmbModelDir.Name = "cmbModelDir";
            this.cmbModelDir.Size = new System.Drawing.Size(220, 21);
            this.cmbModelDir.TabIndex = 50;
            this.cmbModelDir.SelectedIndexChanged += new System.EventHandler(this.cmbModelDir_SelectedIndexChanged);
            // 
            // frmRunRegressionRaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 337);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbModelDir);
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
            this.Name = "frmRunRegressionRaster";
            this.Text = "Create Regression Surface(s)";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnViewOrder;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.ListBox lstRasterBands;
        private System.Windows.Forms.Button btnRasterBands;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbRasterBands;
        private System.Windows.Forms.Button btnOpenFeatureClass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSampleFeatureClass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbModelDir;
    }
}