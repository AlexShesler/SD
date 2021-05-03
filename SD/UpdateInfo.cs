using System;
using System.Windows.Forms;
using System.Xml;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SD
{
    public partial class UpdateInfo : Form
    {
        public UpdateInfo()
        {
            InitializeComponent();
            GetInfoUpdate();
        }

        string version = "";

        public void GetInfoUpdate()
        {
            StringBuilder txt = new StringBuilder();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("version_new.xml");

            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Name == "version")
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("ver");
                    txt.AppendLine("Доступна новая версия SDTander:");
                    txt.AppendLine(attr.Value);
                    version = attr.Value;
                    txt.AppendLine("Что нового:");
                    txbVerUpdate.Text = txt.ToString();
                }
                if (xnode.Name == "whats_new")
                {
                    rtbUpdate.Text = xnode.InnerText;
                }

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            File.Delete("version_new.xml");
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = File.AppendText(@"uphistory"))
            {

                string uptext = rtbUpdate.Text.Replace("\n", "\n\r");
                sw.WriteLine("\n\r" + version + ":\n\r");
                string[] strs = rtbUpdate.Lines;

                foreach (string x in strs)
                {
                    sw.WriteLine(x);
                }
                //sw.WriteLine(rtbUpdate.Text);
                sw.Close();
            }

            try
            {
                ProcessStartInfo pSI = new ProcessStartInfo("Updater.exe");
                //Redirects output
                pSI.RedirectStandardOutput = true;
                pSI.UseShellExecute = false;
                //No black window
                pSI.CreateNoWindow = true;
                //Creates a process
                Process proc = new Process();
                //Set start info
                proc.StartInfo = pSI;
                //Start
                proc.Start();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    }
}
