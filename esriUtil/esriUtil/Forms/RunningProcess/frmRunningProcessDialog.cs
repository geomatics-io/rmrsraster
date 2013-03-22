using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace esriUtil.Forms.RunningProcess
{
    public partial class frmRunningProcessDialog : Form
    {
        public frmRunningProcessDialog()
        {
            InitializeComponent();
        }
        public frmRunningProcessDialog(bool autoClose)
        {
            InitializeComponent();
            aC = autoClose;
        }
        private bool aC = false;
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        public void addMessage(string msg)
        {
            rtbMessages.AppendText(msg + "\n");
            this.Refresh();
        }
        public string[] getMessages()
        {
            return rtbMessages.Lines;
        }
        public void stepPGBar(int steps)
        {
            
            pgbProcess.Style = ProgressBarStyle.Blocks;
            int pgValue = pgbProcess.Value;
            int dif = pgbProcess.Maximum - pgValue;
            if (dif < steps)
            {
                steps = dif;
            }
            if (steps > 0)
            {
                pgbProcess.Increment(steps);
            }
            //pgbProcess.Style = ProgressBarStyle.Marquee;
            this.Refresh();

        }
        public void enableClose()
        {
            btnClose.Enabled = true;
            btnSave.Enabled = true;
            rtbMessages.Enabled = true;
            this.pgbProcess.Style = ProgressBarStyle.Blocks;
            this.Refresh();

            if (aC)
            {
                this.Close();
            }

        }
        public void showInSepperateProcess()
        {
            Thread t = new Thread(() => Application.Run(this));
            t.Start();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.AddExtension = true;
            sd.DefaultExt = "txt";
            sd.Filter = "Text|*.txt";
            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sd.FileName))
                {
                    foreach (string s in rtbMessages.Lines)
                    {
                        sw.WriteLine(s);
                    }
                    sw.Close();
                }
                if (System.Windows.Forms.MessageBox.Show("Report has been saved. Do you want to open it?", "Open", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(sd.FileName);
                }
            }
        }

        private void frmRunningProcessDialog_ResizeEnd(object sender, EventArgs e)
        {
            
            rtbMessages.Width = this.Size.Width - 3;
            rtbMessages.Height = this.Size.Height - 59;
        }

    }
}
