using System;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace SD
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            GetConfigs();
        }

        public void GetConfigs()
        {
            StreamReader str = new StreamReader("config", Encoding.UTF8);

            while (!str.EndOfStream)
            {
                string line = str.ReadLine();
                if (line.StartsWith("autoupdate"))
                {
                    string[] i = line.Split('|');
                    if (i[1] == "true") chbAutoUpdate.Checked = true;
                    else chbAutoUpdate.Checked = false;
                }
                if (line.StartsWith("pathupdate"))
                {
                    string[] i = line.Split('|');
                    txbPathUpdate.Text = i[1];
                }
                if (line.StartsWith("putty"))
                {
                    string[] i = line.Split('|');
                    txbPathPutty.Text = i[1];
                }
                if (line.StartsWith("rms"))
                {
                    string[] i = line.Split('|');
                    txbPathRms.Text = i[1];
                }
                if (line.StartsWith("uvnc"))
                {
                    string[] i = line.Split('|');
                    txbPathUvnc.Text = i[1];
                }
                if (line.StartsWith("uphost"))
                {
                    string[] i = line.Split('|');
                    txbUpHost.Text = i[1];
                }
            }
            str.Close();
        }

        private void btnPathPutty_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            txbPathPutty.Text = openFileDialog.FileName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            StreamReader str = new StreamReader("config", Encoding.Default);
            StringBuilder stb = new StringBuilder();

            while (!str.EndOfStream)
            {
                string line = str.ReadLine();
                string[] i = line.Split('|');

                if (line.StartsWith("autoupdate"))
                {
                    if (chbAutoUpdate.Checked == true) i[1] = "true";
                    else i[1] = "false";
                    stb.AppendLine(i[0] + "|" + i[1]);
                }
                if (line.StartsWith("pathupdate"))
                {
                    i[1] = txbPathUpdate.Text;
                    stb.AppendLine(i[0] + "|" + i[1]);
                }
                if (line.StartsWith("putty"))
                {
                    i[1] = txbPathPutty.Text;
                    stb.AppendLine(i[0] + "|" + i[1]);
                }
                if (line.StartsWith("rms"))
                {
                    i[1] = txbPathRms.Text;
                    stb.AppendLine(i[0] + "|" + i[1]);
                }
                if (line.StartsWith("uvnc"))
                {
                    i[1] = txbPathUvnc.Text;
                    stb.AppendLine(i[0] + "|" + i[1]);
                }
                if (line.StartsWith("uphost"))
                {
                    i[1] = txbUpHost.Text;
                    stb.AppendLine(i[0] + "|" + i[1]);
                }
            }

            str.Close();

            StreamWriter sw = new StreamWriter("config", false, new UTF8Encoding(false));
            sw.Write(stb);
            sw.Close();
            Close();

            MessageBox.Show("Для применения настроек программа будет перезапущена.", "Внимание!", MessageBoxButtons.OK);
            Application.Restart();
        }

        private void btnPathRms_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            txbPathRms.Text = openFileDialog.FileName;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPathUvnc_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            txbPathUvnc.Text = openFileDialog.FileName;
        }
    }
}
