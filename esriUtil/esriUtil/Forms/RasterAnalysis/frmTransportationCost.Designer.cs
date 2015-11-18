namespace esriUtil.Forms.RasterAnalysis
{
    partial class frmTransportationCost
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTransportationCost));
            this.cmbRoad = new System.Windows.Forms.ComboBox();
            this.lblRoads = new System.Windows.Forms.Label();
            this.btnOpenRoad = new System.Windows.Forms.Button();
            this.cmbSpeed = new System.Windows.Forms.ComboBox();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.cmbDem = new System.Windows.Forms.ComboBox();
            this.lblDem = new System.Windows.Forms.Label();
            this.cmbUnits = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grpOnRoad = new System.Windows.Forms.GroupBox();
            this.lblFacility = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFacility = new System.Windows.Forms.ComboBox();
            this.btnFacility = new System.Windows.Forms.Button();
            this.nudOnPayload = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOnRate = new System.Windows.Forms.Button();
            this.cmbOnRate = new System.Windows.Forms.ComboBox();
            this.grpOffRoad = new System.Windows.Forms.GroupBox();
            this.btnOffRate = new System.Windows.Forms.Button();
            this.cmbOffRate = new System.Windows.Forms.ComboBox();
            this.nudOffPayload = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnBarriers = new System.Windows.Forms.Button();
            this.lblBarriers = new System.Windows.Forms.Label();
            this.cmbBarrier = new System.Windows.Forms.ComboBox();
            this.btnOffRoadSpeed = new System.Windows.Forms.Button();
            this.lblOffSpeed = new System.Windows.Forms.Label();
            this.cmbOffRoadSpeed = new System.Windows.Forms.ComboBox();
            this.btnDem = new System.Windows.Forms.Button();
            this.grpOps = new System.Windows.Forms.GroupBox();
            this.btnOps = new System.Windows.Forms.Button();
            this.cmbOps = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.grpOther = new System.Windows.Forms.GroupBox();
            this.btnOther = new System.Windows.Forms.Button();
            this.cmbOther = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.chbHours = new System.Windows.Forms.CheckBox();
            this.btnOutWks = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOutWks = new System.Windows.Forms.TextBox();
            this.grpOnRoad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOnPayload)).BeginInit();
            this.grpOffRoad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOffPayload)).BeginInit();
            this.grpOps.SuspendLayout();
            this.grpOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbRoad
            // 
            this.cmbRoad.FormattingEnabled = true;
            this.cmbRoad.Location = new System.Drawing.Point(26, 91);
            this.cmbRoad.Name = "cmbRoad";
            this.cmbRoad.Size = new System.Drawing.Size(141, 21);
            this.cmbRoad.TabIndex = 0;
            this.cmbRoad.SelectedIndexChanged += new System.EventHandler(this.cmbRoad_SelectedIndexChanged);
            // 
            // lblRoads
            // 
            this.lblRoads.AutoSize = true;
            this.lblRoads.Location = new System.Drawing.Point(23, 72);
            this.lblRoads.Name = "lblRoads";
            this.lblRoads.Size = new System.Drawing.Size(67, 13);
            this.lblRoads.TabIndex = 1;
            this.lblRoads.Text = "Roads Layer";
            // 
            // btnOpenRoad
            // 
            this.btnOpenRoad.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOpenRoad.Location = new System.Drawing.Point(158, 34);
            this.btnOpenRoad.Name = "btnOpenRoad";
            this.btnOpenRoad.Size = new System.Drawing.Size(26, 25);
            this.btnOpenRoad.TabIndex = 2;
            this.btnOpenRoad.UseVisualStyleBackColor = true;
            this.btnOpenRoad.Click += new System.EventHandler(this.btnOpenRoad_Click);
            // 
            // cmbSpeed
            // 
            this.cmbSpeed.FormattingEnabled = true;
            this.cmbSpeed.Location = new System.Drawing.Point(11, 83);
            this.cmbSpeed.Name = "cmbSpeed";
            this.cmbSpeed.Size = new System.Drawing.Size(90, 21);
            this.cmbSpeed.TabIndex = 3;
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(8, 65);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(63, 13);
            this.lblSpeed.TabIndex = 4;
            this.lblSpeed.Text = "Speed Field";
            // 
            // cmbDem
            // 
            this.cmbDem.FormattingEnabled = true;
            this.cmbDem.Location = new System.Drawing.Point(26, 30);
            this.cmbDem.Name = "cmbDem";
            this.cmbDem.Size = new System.Drawing.Size(141, 21);
            this.cmbDem.TabIndex = 5;
            // 
            // lblDem
            // 
            this.lblDem.AutoSize = true;
            this.lblDem.Location = new System.Drawing.Point(26, 11);
            this.lblDem.Name = "lblDem";
            this.lblDem.Size = new System.Drawing.Size(29, 13);
            this.lblDem.TabIndex = 6;
            this.lblDem.Text = "Dem";
            // 
            // cmbUnits
            // 
            this.cmbUnits.FormattingEnabled = true;
            this.cmbUnits.Location = new System.Drawing.Point(205, 30);
            this.cmbUnits.Name = "cmbUnits";
            this.cmbUnits.Size = new System.Drawing.Size(73, 21);
            this.cmbUnits.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(202, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Speed Units";
            // 
            // grpOnRoad
            // 
            this.grpOnRoad.Controls.Add(this.lblFacility);
            this.grpOnRoad.Controls.Add(this.label1);
            this.grpOnRoad.Controls.Add(this.cmbFacility);
            this.grpOnRoad.Controls.Add(this.btnFacility);
            this.grpOnRoad.Controls.Add(this.nudOnPayload);
            this.grpOnRoad.Controls.Add(this.label4);
            this.grpOnRoad.Controls.Add(this.btnOnRate);
            this.grpOnRoad.Controls.Add(this.lblSpeed);
            this.grpOnRoad.Controls.Add(this.cmbSpeed);
            this.grpOnRoad.Controls.Add(this.cmbOnRate);
            this.grpOnRoad.Controls.Add(this.btnOpenRoad);
            this.grpOnRoad.Location = new System.Drawing.Point(15, 54);
            this.grpOnRoad.Name = "grpOnRoad";
            this.grpOnRoad.Size = new System.Drawing.Size(391, 114);
            this.grpOnRoad.TabIndex = 11;
            this.grpOnRoad.TabStop = false;
            this.grpOnRoad.Text = "On Road";
            // 
            // lblFacility
            // 
            this.lblFacility.AutoSize = true;
            this.lblFacility.Location = new System.Drawing.Point(204, 18);
            this.lblFacility.Name = "lblFacility";
            this.lblFacility.Size = new System.Drawing.Size(39, 13);
            this.lblFacility.TabIndex = 31;
            this.lblFacility.Text = "Facility";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(109, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Machine Rate ($/hour; Value or Raster)";
            // 
            // cmbFacility
            // 
            this.cmbFacility.FormattingEnabled = true;
            this.cmbFacility.Location = new System.Drawing.Point(207, 37);
            this.cmbFacility.Name = "cmbFacility";
            this.cmbFacility.Size = new System.Drawing.Size(141, 21);
            this.cmbFacility.TabIndex = 30;
            // 
            // btnFacility
            // 
            this.btnFacility.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnFacility.Location = new System.Drawing.Point(354, 34);
            this.btnFacility.Name = "btnFacility";
            this.btnFacility.Size = new System.Drawing.Size(26, 25);
            this.btnFacility.TabIndex = 32;
            this.btnFacility.UseVisualStyleBackColor = true;
            this.btnFacility.Click += new System.EventHandler(this.btnFacility_Click);
            // 
            // nudOnPayload
            // 
            this.nudOnPayload.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudOnPayload.Location = new System.Drawing.Point(310, 84);
            this.nudOnPayload.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudOnPayload.Name = "nudOnPayload";
            this.nudOnPayload.Size = new System.Drawing.Size(43, 20);
            this.nudOnPayload.TabIndex = 34;
            this.nudOnPayload.ThousandsSeparator = true;
            this.nudOnPayload.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(307, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Payload (Tons)";
            // 
            // btnOnRate
            // 
            this.btnOnRate.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOnRate.Location = new System.Drawing.Point(270, 81);
            this.btnOnRate.Name = "btnOnRate";
            this.btnOnRate.Size = new System.Drawing.Size(26, 25);
            this.btnOnRate.TabIndex = 36;
            this.btnOnRate.UseVisualStyleBackColor = true;
            this.btnOnRate.Click += new System.EventHandler(this.btnOnRate_Click);
            // 
            // cmbOnRate
            // 
            this.cmbOnRate.FormattingEnabled = true;
            this.cmbOnRate.Location = new System.Drawing.Point(112, 83);
            this.cmbOnRate.Name = "cmbOnRate";
            this.cmbOnRate.Size = new System.Drawing.Size(152, 21);
            this.cmbOnRate.TabIndex = 35;
            // 
            // grpOffRoad
            // 
            this.grpOffRoad.Controls.Add(this.btnOffRate);
            this.grpOffRoad.Controls.Add(this.cmbOffRate);
            this.grpOffRoad.Controls.Add(this.nudOffPayload);
            this.grpOffRoad.Controls.Add(this.label8);
            this.grpOffRoad.Controls.Add(this.label9);
            this.grpOffRoad.Controls.Add(this.btnBarriers);
            this.grpOffRoad.Controls.Add(this.lblBarriers);
            this.grpOffRoad.Controls.Add(this.cmbBarrier);
            this.grpOffRoad.Controls.Add(this.btnOffRoadSpeed);
            this.grpOffRoad.Controls.Add(this.lblOffSpeed);
            this.grpOffRoad.Controls.Add(this.cmbOffRoadSpeed);
            this.grpOffRoad.Location = new System.Drawing.Point(15, 174);
            this.grpOffRoad.Name = "grpOffRoad";
            this.grpOffRoad.Size = new System.Drawing.Size(391, 118);
            this.grpOffRoad.TabIndex = 12;
            this.grpOffRoad.TabStop = false;
            this.grpOffRoad.Text = "Off Road";
            // 
            // btnOffRate
            // 
            this.btnOffRate.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOffRate.Location = new System.Drawing.Point(354, 34);
            this.btnOffRate.Name = "btnOffRate";
            this.btnOffRate.Size = new System.Drawing.Size(26, 25);
            this.btnOffRate.TabIndex = 34;
            this.btnOffRate.UseVisualStyleBackColor = true;
            this.btnOffRate.Click += new System.EventHandler(this.btnOffRate_Click);
            // 
            // cmbOffRate
            // 
            this.cmbOffRate.FormattingEnabled = true;
            this.cmbOffRate.Location = new System.Drawing.Point(201, 37);
            this.cmbOffRate.Name = "cmbOffRate";
            this.cmbOffRate.Size = new System.Drawing.Size(144, 21);
            this.cmbOffRate.TabIndex = 33;
            // 
            // nudOffPayload
            // 
            this.nudOffPayload.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudOffPayload.Location = new System.Drawing.Point(201, 85);
            this.nudOffPayload.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudOffPayload.Name = "nudOffPayload";
            this.nudOffPayload.Size = new System.Drawing.Size(42, 20);
            this.nudOffPayload.TabIndex = 32;
            this.nudOffPayload.ThousandsSeparator = true;
            this.nudOffPayload.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(198, 66);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Payload (Tons)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(195, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(194, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Machine Rate ($/hour; Value or Raster)";
            // 
            // btnBarriers
            // 
            this.btnBarriers.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnBarriers.Location = new System.Drawing.Point(158, 81);
            this.btnBarriers.Name = "btnBarriers";
            this.btnBarriers.Size = new System.Drawing.Size(26, 25);
            this.btnBarriers.TabIndex = 20;
            this.btnBarriers.UseVisualStyleBackColor = true;
            this.btnBarriers.Click += new System.EventHandler(this.btnBarriers_Click);
            // 
            // lblBarriers
            // 
            this.lblBarriers.AutoSize = true;
            this.lblBarriers.Location = new System.Drawing.Point(8, 66);
            this.lblBarriers.Name = "lblBarriers";
            this.lblBarriers.Size = new System.Drawing.Size(119, 13);
            this.lblBarriers.TabIndex = 19;
            this.lblBarriers.Text = "Barriers Layer (Optional)";
            // 
            // cmbBarrier
            // 
            this.cmbBarrier.FormattingEnabled = true;
            this.cmbBarrier.Location = new System.Drawing.Point(11, 84);
            this.cmbBarrier.Name = "cmbBarrier";
            this.cmbBarrier.Size = new System.Drawing.Size(141, 21);
            this.cmbBarrier.TabIndex = 18;
            // 
            // btnOffRoadSpeed
            // 
            this.btnOffRoadSpeed.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOffRoadSpeed.Location = new System.Drawing.Point(158, 34);
            this.btnOffRoadSpeed.Name = "btnOffRoadSpeed";
            this.btnOffRoadSpeed.Size = new System.Drawing.Size(26, 25);
            this.btnOffRoadSpeed.TabIndex = 17;
            this.btnOffRoadSpeed.UseVisualStyleBackColor = true;
            this.btnOffRoadSpeed.Click += new System.EventHandler(this.btnOffRoadSpeed_Click);
            // 
            // lblOffSpeed
            // 
            this.lblOffSpeed.AutoSize = true;
            this.lblOffSpeed.Location = new System.Drawing.Point(8, 19);
            this.lblOffSpeed.Name = "lblOffSpeed";
            this.lblOffSpeed.Size = new System.Drawing.Size(120, 13);
            this.lblOffSpeed.TabIndex = 16;
            this.lblOffSpeed.Text = "Speed (Value or Raster)";
            // 
            // cmbOffRoadSpeed
            // 
            this.cmbOffRoadSpeed.FormattingEnabled = true;
            this.cmbOffRoadSpeed.Location = new System.Drawing.Point(11, 37);
            this.cmbOffRoadSpeed.Name = "cmbOffRoadSpeed";
            this.cmbOffRoadSpeed.Size = new System.Drawing.Size(141, 21);
            this.cmbOffRoadSpeed.TabIndex = 15;
            // 
            // btnDem
            // 
            this.btnDem.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnDem.Location = new System.Drawing.Point(173, 27);
            this.btnDem.Name = "btnDem";
            this.btnDem.Size = new System.Drawing.Size(26, 25);
            this.btnDem.TabIndex = 13;
            this.btnDem.UseVisualStyleBackColor = true;
            this.btnDem.Click += new System.EventHandler(this.btnDem_Click);
            // 
            // grpOps
            // 
            this.grpOps.Controls.Add(this.btnOps);
            this.grpOps.Controls.Add(this.cmbOps);
            this.grpOps.Controls.Add(this.label10);
            this.grpOps.Location = new System.Drawing.Point(15, 298);
            this.grpOps.Name = "grpOps";
            this.grpOps.Size = new System.Drawing.Size(198, 68);
            this.grpOps.TabIndex = 14;
            this.grpOps.TabStop = false;
            this.grpOps.Text = "Operations";
            // 
            // btnOps
            // 
            this.btnOps.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOps.Location = new System.Drawing.Point(158, 32);
            this.btnOps.Name = "btnOps";
            this.btnOps.Size = new System.Drawing.Size(26, 25);
            this.btnOps.TabIndex = 32;
            this.btnOps.UseVisualStyleBackColor = true;
            this.btnOps.Click += new System.EventHandler(this.btnOps_Click);
            // 
            // cmbOps
            // 
            this.cmbOps.FormattingEnabled = true;
            this.cmbOps.Location = new System.Drawing.Point(11, 35);
            this.cmbOps.Name = "cmbOps";
            this.cmbOps.Size = new System.Drawing.Size(141, 21);
            this.cmbOps.TabIndex = 31;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(148, 13);
            this.label10.TabIndex = 26;
            this.label10.Text = "Rate ($/Ton; Value or Raster)";
            // 
            // grpOther
            // 
            this.grpOther.Controls.Add(this.btnOther);
            this.grpOther.Controls.Add(this.cmbOther);
            this.grpOther.Controls.Add(this.label12);
            this.grpOther.Location = new System.Drawing.Point(205, 298);
            this.grpOther.Name = "grpOther";
            this.grpOther.Size = new System.Drawing.Size(198, 68);
            this.grpOther.TabIndex = 15;
            this.grpOther.TabStop = false;
            this.grpOther.Text = "Other";
            // 
            // btnOther
            // 
            this.btnOther.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOther.Location = new System.Drawing.Point(164, 32);
            this.btnOther.Name = "btnOther";
            this.btnOther.Size = new System.Drawing.Size(26, 25);
            this.btnOther.TabIndex = 28;
            this.btnOther.UseVisualStyleBackColor = true;
            this.btnOther.Click += new System.EventHandler(this.btnOther_Click);
            // 
            // cmbOther
            // 
            this.cmbOther.FormattingEnabled = true;
            this.cmbOther.Location = new System.Drawing.Point(11, 35);
            this.cmbOther.Name = "cmbOther";
            this.cmbOther.Size = new System.Drawing.Size(147, 21);
            this.cmbOther.TabIndex = 27;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(148, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "Rate ($/Ton; Value or Raster)";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(347, 386);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(56, 25);
            this.btnExecute.TabIndex = 16;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // chbHours
            // 
            this.chbHours.AutoSize = true;
            this.chbHours.Checked = true;
            this.chbHours.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbHours.Location = new System.Drawing.Point(289, 32);
            this.chbHours.Name = "chbHours";
            this.chbHours.Size = new System.Drawing.Size(122, 17);
            this.chbHours.TabIndex = 29;
            this.chbHours.Text = "Create Hour Rasters";
            this.chbHours.UseVisualStyleBackColor = true;
            this.chbHours.CheckedChanged += new System.EventHandler(this.chbHours_CheckedChanged);
            // 
            // btnOutWks
            // 
            this.btnOutWks.Image = global::esriUtil.Properties.Resources.cmdOpenProject;
            this.btnOutWks.Location = new System.Drawing.Point(302, 386);
            this.btnOutWks.Name = "btnOutWks";
            this.btnOutWks.Size = new System.Drawing.Size(25, 27);
            this.btnOutWks.TabIndex = 99;
            this.btnOutWks.UseVisualStyleBackColor = true;
            this.btnOutWks.Click += new System.EventHandler(this.btnOutWks_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 369);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 13);
            this.label3.TabIndex = 98;
            this.label3.Text = "Output File Geodatabase";
            // 
            // txtOutWks
            // 
            this.txtOutWks.Location = new System.Drawing.Point(15, 389);
            this.txtOutWks.Name = "txtOutWks";
            this.txtOutWks.Size = new System.Drawing.Size(283, 20);
            this.txtOutWks.TabIndex = 100;
            // 
            // frmTransportationCost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 424);
            this.Controls.Add(this.txtOutWks);
            this.Controls.Add(this.btnOutWks);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chbHours);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.grpOther);
            this.Controls.Add(this.grpOps);
            this.Controls.Add(this.btnDem);
            this.Controls.Add(this.grpOffRoad);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbUnits);
            this.Controls.Add(this.lblDem);
            this.Controls.Add(this.cmbDem);
            this.Controls.Add(this.lblRoads);
            this.Controls.Add(this.cmbRoad);
            this.Controls.Add(this.grpOnRoad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTransportationCost";
            this.Text = "Delivered Cost";
            this.TopMost = true;
            this.grpOnRoad.ResumeLayout(false);
            this.grpOnRoad.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOnPayload)).EndInit();
            this.grpOffRoad.ResumeLayout(false);
            this.grpOffRoad.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOffPayload)).EndInit();
            this.grpOps.ResumeLayout(false);
            this.grpOps.PerformLayout();
            this.grpOther.ResumeLayout(false);
            this.grpOther.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbRoad;
        private System.Windows.Forms.Label lblRoads;
        private System.Windows.Forms.Button btnOpenRoad;
        private System.Windows.Forms.ComboBox cmbSpeed;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.ComboBox cmbDem;
        private System.Windows.Forms.Label lblDem;
        private System.Windows.Forms.ComboBox cmbUnits;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grpOnRoad;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grpOffRoad;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnBarriers;
        private System.Windows.Forms.Label lblBarriers;
        private System.Windows.Forms.ComboBox cmbBarrier;
        private System.Windows.Forms.Button btnOffRoadSpeed;
        private System.Windows.Forms.Label lblOffSpeed;
        private System.Windows.Forms.ComboBox cmbOffRoadSpeed;
        private System.Windows.Forms.Button btnDem;
        private System.Windows.Forms.NumericUpDown nudOffPayload;
        private System.Windows.Forms.GroupBox grpOps;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox grpOther;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.NumericUpDown nudOnPayload;
        private System.Windows.Forms.CheckBox chbHours;
        private System.Windows.Forms.Button btnOnRate;
        private System.Windows.Forms.ComboBox cmbOnRate;
        private System.Windows.Forms.Button btnOffRate;
        private System.Windows.Forms.ComboBox cmbOffRate;
        private System.Windows.Forms.Button btnOps;
        private System.Windows.Forms.ComboBox cmbOps;
        private System.Windows.Forms.Button btnOther;
        private System.Windows.Forms.ComboBox cmbOther;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFacility;
        private System.Windows.Forms.ComboBox cmbFacility;
        private System.Windows.Forms.Button btnFacility;
        private System.Windows.Forms.Button btnOutWks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOutWks;
    }
}