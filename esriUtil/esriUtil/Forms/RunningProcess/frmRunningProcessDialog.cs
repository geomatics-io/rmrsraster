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

    }
}
