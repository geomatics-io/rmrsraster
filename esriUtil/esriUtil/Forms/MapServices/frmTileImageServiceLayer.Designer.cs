namespace esriUtil.Forms.MapServices
{
    partial class frmTileImageServiceLayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTileImageServiceLayer));
            this.btnGeoDb = new System.Windows.Forms.Button();
            this.txtGeoDb = new System.Windows.Forms.TextBox();
            this.cmbImageServiceLayer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDownload = new System.Windows.Forms.Button();
            this.cmbExtent = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnGeoDb
            // 
            this.btnGeoDb.Image = ((System.Drawing.Image)(resources.GetObject("btnGeoDb.Image")));
            this.btnGeoDb.Location = new System.Drawing.Point(182, 114);
            this.btnGeoDb.Name = "btnGeoDb";
            this.btnGeoDb.Size = new System.Drawing.Size(32, 23);
            this.btnGeoDb.TabIndex = 0;
            this.btnGeoDb.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGeoDb.UseVisualStyleBackColor = true;
            this.btnGeoDb.Click += new System.EventHandler(this.btnGeoDb_Click);
            // 
            // txtGeoDb
            // 
            this.txtGeoDb.Location = new System.Drawing.Point(12, 115);
            this.txtGeoDb.Name = "txtGeoDb";
            this.txtGeoDb.Size = new System.Drawing.Size(164, 20);
            this.txtGeoDb.TabIndex = 1;
            // 
            // cmbImageServiceLayer
            // 
            this.cmbImageServiceLayer.FormattingEnabled = true;
            this.cmbImageServiceLayer.Location = new System.Drawing.Point(12, 28);
            this.cmbImageServiceLayer.Name = "cmbImageServiceLayer";
            this.cmbImageServiceLayer.Size = new System.Drawing.Size(202, 21);
            this.cmbImageServiceLayer.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Image Service Layer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Database to Store Image tiles";
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(139, 143);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 5;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // cmbExtent
            // 
            this.cmbExtent.FormattingEnabled = true;
            this.cmbExtent.Location = new System.Drawing.Point(12, 72);
            this.cmbExtent.Name = "cmbExtent";
            this.cmbExtent.Size = new System.Drawing.Size(202, 21);
            this.cmbExtent.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Extent";
            // 
            // frmTileImageServiceLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 170);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbExtent);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbImageServiceLayer);
            this.Controls.Add(this.txtGeoDb);
            this.Controls.Add(this.btnGeoDb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTileImageServiceLayer";
            this.Text = "Download images from a service";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmTileImageServiceLayer_FormClosed);
            this.Load += new System.EventHandler(this.frmTileImageServiceLayer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGeoDb;
        private System.Windows.Forms.TextBox txtGeoDb;
        private System.Windows.Forms.ComboBox cmbImageServiceLayer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ComboBox cmbExtent;
        private System.Windows.Forms.Label label3;
    }
}