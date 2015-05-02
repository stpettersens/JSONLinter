using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace JSONLinter
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void openURL(string url)
        {
            ProcessStartInfo p = new ProcessStartInfo(url);
            p.UseShellExecute = true;
            Process.Start(p);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openURL(lnkLink.Text);
        }

        private void lnkLink2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openURL(lnkLink2.Text);
        }
    }
}
