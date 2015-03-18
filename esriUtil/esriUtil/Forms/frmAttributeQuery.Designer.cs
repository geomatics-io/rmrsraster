namespace esriUtil.Forms
{
    partial class frmAttributeQuery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAttributeQuery));
            this.lstFields = new System.Windows.Forms.ListBox();
            this.btnEqual = new System.Windows.Forms.Button();
            this.btnNEqual = new System.Windows.Forms.Button();
            this.btnLike = new System.Windows.Forms.Button();
            this.btnAnd = new System.Windows.Forms.Button();
            this.btnGrEq = new System.Windows.Forms.Button();
            this.btnGr = new System.Windows.Forms.Button();
            this.btnOr = new System.Windows.Forms.Button();
            this.btnLsEq = new System.Windows.Forms.Button();
            this.btnLs = new System.Windows.Forms.Button();
            this.btnNot = new System.Windows.Forms.Button();
            this.btnPar = new System.Windows.Forms.Button();
            this.btn1 = new System.Windows.Forms.Button();
            this.btnIs = new System.Windows.Forms.Button();
            this.lstUniq = new System.Windows.Forms.ListBox();
            this.btnUniq = new System.Windows.Forms.Button();
            this.rctQry = new System.Windows.Forms.RichTextBox();
            this.lblSelect = new System.Windows.Forms.Label();
            this.btnFinish = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstFields
            // 
            this.lstFields.FormattingEnabled = true;
            this.lstFields.Location = new System.Drawing.Point(12, 11);
            this.lstFields.Name = "lstFields";
            this.lstFields.Size = new System.Drawing.Size(334, 108);
            this.lstFields.TabIndex = 0;
            this.lstFields.DoubleClick += new System.EventHandler(this.lstFields_DoubleClick);
            // 
            // btnEqual
            // 
            this.btnEqual.Location = new System.Drawing.Point(12, 136);
            this.btnEqual.Name = "btnEqual";
            this.btnEqual.Size = new System.Drawing.Size(33, 23);
            this.btnEqual.TabIndex = 1;
            this.btnEqual.Text = " = ";
            this.btnEqual.UseVisualStyleBackColor = true;
            this.btnEqual.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnNEqual
            // 
            this.btnNEqual.Location = new System.Drawing.Point(51, 136);
            this.btnNEqual.Name = "btnNEqual";
            this.btnNEqual.Size = new System.Drawing.Size(33, 23);
            this.btnNEqual.TabIndex = 2;
            this.btnNEqual.Text = " <> ";
            this.btnNEqual.UseVisualStyleBackColor = true;
            this.btnNEqual.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnLike
            // 
            this.btnLike.Location = new System.Drawing.Point(90, 136);
            this.btnLike.Name = "btnLike";
            this.btnLike.Size = new System.Drawing.Size(45, 23);
            this.btnLike.TabIndex = 3;
            this.btnLike.Text = " Like ";
            this.btnLike.UseVisualStyleBackColor = true;
            this.btnLike.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnAnd
            // 
            this.btnAnd.Location = new System.Drawing.Point(90, 165);
            this.btnAnd.Name = "btnAnd";
            this.btnAnd.Size = new System.Drawing.Size(45, 23);
            this.btnAnd.TabIndex = 6;
            this.btnAnd.Text = " And ";
            this.btnAnd.UseVisualStyleBackColor = true;
            this.btnAnd.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnGrEq
            // 
            this.btnGrEq.Location = new System.Drawing.Point(51, 165);
            this.btnGrEq.Name = "btnGrEq";
            this.btnGrEq.Size = new System.Drawing.Size(33, 23);
            this.btnGrEq.TabIndex = 5;
            this.btnGrEq.Text = " >= ";
            this.btnGrEq.UseVisualStyleBackColor = true;
            this.btnGrEq.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnGr
            // 
            this.btnGr.Location = new System.Drawing.Point(12, 165);
            this.btnGr.Name = "btnGr";
            this.btnGr.Size = new System.Drawing.Size(33, 23);
            this.btnGr.TabIndex = 4;
            this.btnGr.Text = " > ";
            this.btnGr.UseVisualStyleBackColor = true;
            this.btnGr.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnOr
            // 
            this.btnOr.Location = new System.Drawing.Point(90, 194);
            this.btnOr.Name = "btnOr";
            this.btnOr.Size = new System.Drawing.Size(45, 23);
            this.btnOr.TabIndex = 9;
            this.btnOr.Text = " Or ";
            this.btnOr.UseVisualStyleBackColor = true;
            this.btnOr.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnLsEq
            // 
            this.btnLsEq.Location = new System.Drawing.Point(51, 194);
            this.btnLsEq.Name = "btnLsEq";
            this.btnLsEq.Size = new System.Drawing.Size(33, 23);
            this.btnLsEq.TabIndex = 8;
            this.btnLsEq.Text = " <= ";
            this.btnLsEq.UseVisualStyleBackColor = true;
            this.btnLsEq.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnLs
            // 
            this.btnLs.Location = new System.Drawing.Point(12, 194);
            this.btnLs.Name = "btnLs";
            this.btnLs.Size = new System.Drawing.Size(33, 23);
            this.btnLs.TabIndex = 7;
            this.btnLs.Text = " < ";
            this.btnLs.UseVisualStyleBackColor = true;
            this.btnLs.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnNot
            // 
            this.btnNot.Location = new System.Drawing.Point(90, 223);
            this.btnNot.Name = "btnNot";
            this.btnNot.Size = new System.Drawing.Size(45, 23);
            this.btnNot.TabIndex = 12;
            this.btnNot.Text = " Not ";
            this.btnNot.UseVisualStyleBackColor = true;
            this.btnNot.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnPar
            // 
            this.btnPar.Location = new System.Drawing.Point(51, 223);
            this.btnPar.Name = "btnPar";
            this.btnPar.Size = new System.Drawing.Size(33, 23);
            this.btnPar.TabIndex = 11;
            this.btnPar.Text = " ( ) ";
            this.btnPar.UseVisualStyleBackColor = true;
            this.btnPar.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btn1
            // 
            this.btn1.Location = new System.Drawing.Point(12, 223);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(17, 23);
            this.btn1.TabIndex = 10;
            this.btn1.Text = "_";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnIs
            // 
            this.btnIs.Location = new System.Drawing.Point(12, 252);
            this.btnIs.Name = "btnIs";
            this.btnIs.Size = new System.Drawing.Size(33, 23);
            this.btnIs.TabIndex = 13;
            this.btnIs.Text = " Is ";
            this.btnIs.UseVisualStyleBackColor = true;
            this.btnIs.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // lstUniq
            // 
            this.lstUniq.FormattingEnabled = true;
            this.lstUniq.Location = new System.Drawing.Point(141, 140);
            this.lstUniq.Name = "lstUniq";
            this.lstUniq.Size = new System.Drawing.Size(205, 108);
            this.lstUniq.TabIndex = 16;
            this.lstUniq.DoubleClick += new System.EventHandler(this.lstUniq_DoubleClick);
            // 
            // btnUniq
            // 
            this.btnUniq.Location = new System.Drawing.Point(141, 252);
            this.btnUniq.Name = "btnUniq";
            this.btnUniq.Size = new System.Drawing.Size(101, 23);
            this.btnUniq.TabIndex = 17;
            this.btnUniq.Text = "Unique Values";
            this.btnUniq.UseVisualStyleBackColor = true;
            this.btnUniq.Click += new System.EventHandler(this.btnUniq_Click);
            // 
            // rctQry
            // 
            this.rctQry.Location = new System.Drawing.Point(17, 296);
            this.rctQry.Name = "rctQry";
            this.rctQry.Size = new System.Drawing.Size(329, 106);
            this.rctQry.TabIndex = 18;
            this.rctQry.Text = "";
            // 
            // lblSelect
            // 
            this.lblSelect.AutoSize = true;
            this.lblSelect.Location = new System.Drawing.Point(17, 280);
            this.lblSelect.Name = "lblSelect";
            this.lblSelect.Size = new System.Drawing.Size(89, 13);
            this.lblSelect.TabIndex = 19;
            this.lblSelect.Text = "SELECT * FROM";
            // 
            // btnFinish
            // 
            this.btnFinish.Location = new System.Drawing.Point(303, 408);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(43, 23);
            this.btnFinish.TabIndex = 20;
            this.btnFinish.Text = "OK";
            this.btnFinish.UseVisualStyleBackColor = true;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // btn2
            // 
            this.btn2.Location = new System.Drawing.Point(28, 223);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(17, 23);
            this.btn2.TabIndex = 21;
            this.btn2.Text = "%";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(246, 408);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 23);
            this.btnSave.TabIndex = 22;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(189, 408);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(51, 23);
            this.btnLoad.TabIndex = 23;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // frmAttributeQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 440);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btnFinish);
            this.Controls.Add(this.lblSelect);
            this.Controls.Add(this.rctQry);
            this.Controls.Add(this.btnUniq);
            this.Controls.Add(this.lstUniq);
            this.Controls.Add(this.btnIs);
            this.Controls.Add(this.btnNot);
            this.Controls.Add(this.btnPar);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.btnOr);
            this.Controls.Add(this.btnLsEq);
            this.Controls.Add(this.btnLs);
            this.Controls.Add(this.btnAnd);
            this.Controls.Add(this.btnGrEq);
            this.Controls.Add(this.btnGr);
            this.Controls.Add(this.btnLike);
            this.Controls.Add(this.btnNEqual);
            this.Controls.Add(this.btnEqual);
            this.Controls.Add(this.lstFields);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAttributeQuery";
            this.Text = "Define Query";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstFields;
        private System.Windows.Forms.Button btnEqual;
        private System.Windows.Forms.Button btnNEqual;
        private System.Windows.Forms.Button btnLike;
        private System.Windows.Forms.Button btnAnd;
        private System.Windows.Forms.Button btnGrEq;
        private System.Windows.Forms.Button btnGr;
        private System.Windows.Forms.Button btnOr;
        private System.Windows.Forms.Button btnLsEq;
        private System.Windows.Forms.Button btnLs;
        private System.Windows.Forms.Button btnNot;
        private System.Windows.Forms.Button btnPar;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btnIs;
        private System.Windows.Forms.ListBox lstUniq;
        private System.Windows.Forms.Button btnUniq;
        private System.Windows.Forms.RichTextBox rctQry;
        private System.Windows.Forms.Label lblSelect;
        private System.Windows.Forms.Button btnFinish;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
    }
}