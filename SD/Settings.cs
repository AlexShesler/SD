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
                if (line.StartsWith("winscp"))
                {
                    string[] i = line.Split('|');
                    txbPathWinSCP.Text = i[1];
                }
                if (line.StartsWith("sshuser"))
                {
                    string[] i = line.Split('|');
                    txbSshUser.Text = i[1];
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
            StreamReader str = new StreamReader("config", Encoding.UTF8);
            StringBuilder stb = new StringBuilder();

            if (chbAutoUpdate.Checked == true) stb.AppendLine("autoupdate|true");
            else stb.AppendLine("autoupdate|false");
            stb.AppendLine("pathupdate|" + txbPathUpdate.Text);
            stb.AppendLine("putty|" + txbPathPutty.Text);
            stb.AppendLine("rms|" + txbPathRms.Text);
            stb.AppendLine("uvnc|" + txbPathUvnc.Text);
            stb.AppendLine("uphost|" + txbUpHost.Text);
            stb.AppendLine("winscp|" + txbPathWinSCP.Text);
            stb.AppendLine("sshuser|" + txbSshUser.Text);

            while (!str.EndOfStream)
            {
                string line = str.ReadLine();
                string[] i = line.Split('|');
                if (i[0] == "topmost")
                {
                    if (i[1] == "true") stb.AppendLine("topmost|true");
                    else stb.AppendLine("topmost|false");
                }
            }

            str.Close();

            StreamWriter sw = new StreamWriter("config", false, new UTF8Encoding(false));
            sw.Write(stb);
            sw.Close();
            Close();

            if (MainForm.SelfRef != null)
            {
                MainForm.SelfRef.ReadConfig();
            }

            //MessageBox.Show("Для применения настроек программа будет перезапущена.", "Внимание!", MessageBoxButtons.OK);
            //Application.Restart();
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

        private void btnPathWinScp_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            txbPathWinSCP.Text = openFileDialog.FileName;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            TopMost = true;
        }
    }
}
