namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmBatchProcess
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBatchProcess));
            this.rtbBatch = new System.Windows.Forms.RichTextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.lsbFunctions = new System.Windows.Forms.ListBox();
            this.btnEqual = new System.Windows.Forms.Button();
            this.btnPl = new System.Windows.Forms.Button();
            this.btnSep = new System.Windows.Forms.Button();
            this.btnComma = new System.Windows.Forms.Button();
            this.btnPR = new System.Windows.Forms.Button();
            this.lsbLayers = new System.Windows.Forms.ListBox();
            this.lsbOptions = new System.Windows.Forms.ListBox();
            this.btnOpenLayer = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnsemicolon = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSyntax = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbBatch
            // 
            this.rtbBatch.Location = new System.Drawing.Point(5, 194);
            this.rtbBatch.Name = "rtbBatch";
            this.rtbBatch.Size = new System.Drawing.Size(312, 153);
            this.rtbBatch.TabIndex = 0;
            this.rtbBatch.Text = "";
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(8, 353);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(45, 24);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(58, 353);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 24);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(266, 353);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(51, 24);
            this.btnRun.TabIndex = 3;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lsbFunctions
            // 
            this.lsbFunctions.FormattingEnabled = true;
            this.lsbFunctions.HorizontalScrollbar = true;
            this.lsbFunctions.Location = new System.Drawing.Point(112, 28);
            this.lsbFunctions.Name = "lsbFunctions";
            this.lsbFunctions.Size = new System.Drawing.Size(101, 121);
            this.lsbFunctions.TabIndex = 4;
            this.lsbFunctions.SelectedIndexChanged += new System.EventHandler(this.lsbFunctions_SelectedIndexChanged);
            this.lsbFunctions.DoubleClick += new System.EventHandler(this.lsbFunctions_DoubleClick);
            // 
            // btnEqual
            // 
            this.btnEqual.Location = new System.Drawing.Point(48, 160);
            this.btnEqual.Name = "btnEqual";
            this.btnEqual.Size = new System.Drawing.Size(22, 23);
            this.btnEqual.TabIndex = 5;
            this.btnEqual.Text = "=";
            this.btnEqual.UseVisualStyleBackColor = true;
            this.btnEqual.Click += new System.EventHandler(this.btnEqual_Click);
            // 
            // btnPl
            // 
            this.btnPl.Location = new System.Drawing.Point(152, 160);
            this.btnPl.Name = "btnPl";
            this.btnPl.Size = new System.Drawing.Size(22, 23);
            this.btnPl.TabIndex = 9;
            this.btnPl.Text = "(";
            this.btnPl.UseVisualStyleBackColor = true;
            this.btnPl.Click += new System.EventHandler(this.btnPl_Click);
            // 
            // btnSep
            // 
            this.btnSep.Location = new System.Drawing.Point(126, 160);
            this.btnSep.Name = "btnSep";
            this.btnSep.Size = new System.Drawing.Size(22, 23);
            this.btnSep.TabIndex = 10;
            this.btnSep.Text = "-";
            this.btnSep.UseVisualStyleBackColor = true;
            this.btnSep.Click += new System.EventHandler(this.btnSep_Click);
            // 
            // btnComma
            // 
            this.btnComma.Location = new System.Drawing.Point(100, 160);
            this.btnComma.Name = "btnComma";
            this.btnComma.Size = new System.Drawing.Size(22, 23);
            this.btnComma.TabIndex = 11;
            this.btnComma.Text = ",";
            this.btnComma.UseVisualStyleBackColor = true;
            this.btnComma.Click += new System.EventHandler(this.btnComma_Click);
            // 
            // btnPR
            // 
            this.btnPR.Location = new System.Drawing.Point(178, 160);
            this.btnPR.Name = "btnPR";
            this.btnPR.Size = new System.Drawing.Size(22, 23);
            this.btnPR.TabIndex = 12;
            this.btnPR.Text = ")";
            this.btnPR.UseVisualStyleBackColor = true;
            this.btnPR.Click += new System.EventHandler(this.btnPR_Click);
            // 
            // lsbLayers
            // 
            this.lsbLayers.FormattingEnabled = true;
            this.lsbLayers.HorizontalScrollbar = true;
            this.lsbLayers.Location = new System.Drawing.Point(5, 28);
            this.lsbLayers.Name = "lsbLayers";
            this.lsbLayers.Size = new System.Drawing.Size(101, 121);
            this.lsbLayers.TabIndex = 13;
            this.lsbLayers.DoubleClick += new System.EventHandler(this.lsbLayers_SelectedIndexChanged);
            // 
            // lsbOptions
            // 
            this.lsbOptions.FormattingEnabled = true;
            this.lsbOptions.HorizontalScrollbar = true;
            this.lsbOptions.Location = new System.Drawing.Point(216, 28);
            this.lsbOptions.Name = "lsbOptions";
            this.lsbOptions.Size = new System.Drawing.Size(101, 121);
            this.lsbOptions.TabIndex = 14;
            this.lsbOptions.DoubleClick += new System.EventHandler(this.lsbOptions_SelectedIndexChanged);
            // 
            // btnOpenLayer
            // 
            this.btnOpenLayer.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenLayer.Location = new System.Drawing.Point(5, 156);
            this.btnOpenLayer.Name = "btnOpenLayer";
            this.btnOpenLayer.Size = new System.Drawing.Size(29, 30);
            this.btnOpenLayer.TabIndex = 15;
            this.btnOpenLayer.UseVisualStyleBackColor = true;
            this.btnOpenLayer.Click += new System.EventHandler(this.btnOpenLayer_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Layers";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(109, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Functions";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(213, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Options";
            // 
            // btnsemicolon
            // 
            this.btnsemicolon.Location = new System.Drawing.Point(74, 160);
            this.btnsemicolon.Name = "btnsemicolon";
            this.btnsemicolon.Size = new System.Drawing.Size(22, 23);
            this.btnsemicolon.TabIndex = 19;
            this.btnsemicolon.Text = ";";
            this.btnsemicolon.UseVisualStyleBackColor = true;
            this.btnsemicolon.Click += new System.EventHandler(this.btnsemicolon_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(204, 160);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(49, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "Return";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSyntax
            // 
            this.btnSyntax.Location = new System.Drawing.Point(263, 160);
            this.btnSyntax.Name = "btnSyntax";
            this.btnSyntax.Size = new System.Drawing.Size(54, 23);
            this.btnSyntax.TabIndex = 21;
            this.btnSyntax.Text = "Syntax";
            this.btnSyntax.UseVisualStyleBackColor = true;
            this.btnSyntax.Click += new System.EventHandler(this.btnSyntax_Click);
            // 
            // frmBatchProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 389);
            this.Controls.Add(this.btnSyntax);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnsemicolon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOpenLayer);
            this.Controls.Add(this.lsbOptions);
            this.Controls.Add(this.lsbLayers);
            this.Controls.Add(this.btnPR);
            this.Controls.Add(this.btnComma);
            this.Controls.Add(this.btnSep);
            this.Controls.Add(this.btnPl);
            this.Controls.Add(this.btnEqual);
            this.Controls.Add(this.lsbFunctions);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.rtbBatch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmBatchProcess";
            this.Text = "Batch Process";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbBatch;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.ListBox lsbFunctions;
        private System.Windows.Forms.Button btnEqual;
        private System.Windows.Forms.Button btnPl;
        private System.Windows.Forms.Button btnSep;
        private System.Windows.Forms.Button btnComma;
        private System.Windows.Forms.Button btnPR;
        private System.Windows.Forms.ListBox lsbLayers;
        private System.Windows.Forms.ListBox lsbOptions;
        private System.Windows.Forms.Button btnOpenLayer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnsemicolon;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSyntax;
    }
}