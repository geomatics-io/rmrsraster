using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestConsole
{
    public partial class testform : Form
    {
        public testform()
        {
            InitializeComponent();

        }
        
        private int cnt = 0;
        private int Count { get { cnt += 1; return cnt; } }
        private esriUtil.Forms.RunningProcess.frmRunningProcessDialog frm = new esriUtil.Forms.RunningProcess.frmRunningProcessDialog();
        private void button1_Click(object sender, EventArgs e)
        {
            frm.showInSepperateProcess();

        }

        private void btnAddMessage_Click(object sender, EventArgs e)
        {
            frm.addMessage("Message " + Count.ToString());
            frm.stepPGBar(10);
        }

        private void testform_FormClosing(object sender, FormClosingEventArgs e)
        {
            frm.enableClose();
        }
        
    }
}
