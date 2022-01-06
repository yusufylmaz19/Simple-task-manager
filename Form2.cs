using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simpleTaskManager
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOpen.Text))
            {
                try
                {

                    ProcessStartInfo myStartInfo = new ProcessStartInfo(txtOpen.Text);
                    myStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    Process.Start(myStartInfo);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

       }
}
