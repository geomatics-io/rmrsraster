namespace DownloadData
{
    partial class testform
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
            this.btnShowDialog = new System.Windows.Forms.Button();
            this.btnAddMessage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnShowDialog
            // 
            this.btnShowDialog.Location = new System.Drawing.Point(124, 105);
            this.btnShowDialog.Name = "btnShowDialog";
            this.btnShowDialog.Size = new System.Drawing.Size(75, 23);
            this.btnShowDialog.TabIndex = 0;
            this.btnShowDialog.Text = "Show Process";
            this.btnShowDialog.UseVisualStyleBackColor = true;
            this.btnShowDialog.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnAddMessage
            // 
            this.btnAddMessage.Location = new System.Drawing.Point(140, 186);
            this.btnAddMessage.Name = "btnAddMessage";
            this.btnAddMessage.Size = new System.Drawing.Size(132, 23);
            this.btnAddMessage.TabIndex = 1;
            this.btnAddMessage.Text = "Add Message";
            this.btnAddMessage.UseVisualStyleBackColor = true;
            this.btnAddMessage.Click += new System.EventHandler(this.btnAddMessage_Click);
            // 
            // testform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btnAddMessage);
            this.Controls.Add(this.btnShowDialog);
            this.Name = "testform";
            this.Text = "testform";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.testform_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnShowDialog;
        private System.Windows.Forms.Button btnAddMessage;
    }
}