using System;
using System.Collections;
using System.Windows.Forms;

namespace JSONLinter
{
    public partial class frmLogWindow : Form
    {
        private ArrayList events;

        public frmLogWindow()
        {
            InitializeComponent();
            events = new ArrayList();
        }

        public void logEvent(string call, string response)
        {
            events.Add(DateTime.Now.ToString() + ":");
            events.Add(call);
            events.Add(response);
            events.Add("\n");
        }

        private void frmLogWindow_Load(object sender, EventArgs e)
        {
            txtLog.Clear();
            foreach(string _event in events)
            {
                txtLog.AppendText(_event + "\n");
            }
        }
    }
}
