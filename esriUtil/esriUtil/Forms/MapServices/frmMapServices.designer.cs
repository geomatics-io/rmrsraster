namespace esriUtil.Forms.MapServices
{
    public partial  class frmMapServices
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMapServices));
            this.cmbCon = new System.Windows.Forms.ComboBox();
            this.cmbSrv = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chbLayers = new System.Windows.Forms.CheckedListBox();
            this.lblLayers = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnChangeOutput = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbCon
            // 
            this.cmbCon.FormattingEnabled = true;
            this.cmbCon.Location = new System.Drawing.Point(3, 53);
            this.cmbCon.Name = "cmbCon";
            this.cmbCon.Size = new System.Drawing.Size(271, 21);
            this.cmbCon.TabIndex = 0;
            this.cmbCon.SelectedValueChanged += new System.EventHandler(this.cmbCon_SelectedValueChanged);
            // 
            // cmbSrv
            // 
            this.cmbSrv.FormattingEnabled = true;
            this.cmbSrv.Location = new System.Drawing.Point(3, 95);
            this.cmbSrv.Name = "cmbSrv";
            this.cmbSrv.Size = new System.Drawing.Size(271, 21);
            this.cmbSrv.TabIndex = 1;
            this.cmbSrv.SelectedValueChanged += new System.EventHandler(this.cmbSrv_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Service Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "ArcGIS Server Connection";
            // 
            // chbLayers
            // 
            this.chbLayers.CheckOnClick = true;
            this.chbLayers.FormattingEnabled = true;
            this.chbLayers.HorizontalScrollbar = true;
            this.chbLayers.Location = new System.Drawing.Point(3, 149);
            this.chbLayers.Name = "chbLayers";
            this.chbLayers.Size = new System.Drawing.Size(271, 139);
            this.chbLayers.TabIndex = 4;
            this.chbLayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chbLayers_ItemCheck);
            // 
            // lblLayers
            // 
            this.lblLayers.AutoSize = true;
            this.lblLayers.Location = new System.Drawing.Point(0, 132);
            this.lblLayers.Name = "lblLayers";
            this.lblLayers.Size = new System.Drawing.Size(176, 13);
            this.lblLayers.TabIndex = 5;
            this.lblLayers.Text = "Download Data for Checked Layers";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(216, 25);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(27, 23);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(244, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "-";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnChangeOutput
            // 
            this.btnChangeOutput.Location = new System.Drawing.Point(3, 3);
            this.btnChangeOutput.Name = "btnChangeOutput";
            this.btnChangeOutput.Size = new System.Drawing.Size(108, 23);
            this.btnChangeOutput.TabIndex = 8;
            this.btnChangeOutput.Text = "Change Output DB";
            this.btnChangeOutput.UseVisualStyleBackColor = true;
            this.btnChangeOutput.Click += new System.EventHandler(this.btnChangeOutput_Click);
            // 
            // frmMapServices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 291);
            this.Controls.Add(this.btnChangeOutput);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblLayers);
            this.Controls.Add(this.chbLayers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSrv);
            this.Controls.Add(this.cmbCon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMapServices";
            this.Text = "Map Services Setup";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbCon;
        private System.Windows.Forms.ComboBox cmbSrv;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox chbLayers;
        private System.Windows.Forms.Label lblLayers;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnChangeOutput;
    }
}