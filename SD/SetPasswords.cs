using System;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace SD
{
    public partial class SetPasswords : Form
    {
        public SetPasswords()
        {
            InitializeComponent();
        }

        private void btnShowPass1_MouseDown(object sender, MouseEventArgs e)
        {
            TextBox txb = this.Controls["txbPass" + ((Button)sender).Tag.ToString()] as TextBox;
            txb.PasswordChar = '\0';
        }

        private void btnShowPass1_MouseUp(object sender, MouseEventArgs e)
        {
            TextBox txb = this.Controls["txbPass" + ((Button)sender).Tag.ToString()] as TextBox;
            txb.PasswordChar = '•';
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //string loginSrvSsh = "smena";
            if (File.Exists("LockStorage")) File.Delete("LockStorage");

            XmlTextWriter textWritter = new XmlTextWriter("LockStorage", Encoding.UTF8);
            textWritter.WriteStartDocument();
            textWritter.Formatting = Formatting.Indented;
            textWritter.WriteStartElement("data");
            textWritter.WriteEndElement();
            textWritter.Close();           

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("LockStorage");

            XmlElement uvnc = xmlDoc.CreateElement("Uvnc");
            uvnc.InnerText = txbPassUvnc.Text;
            xmlDoc.DocumentElement.AppendChild(uvnc);
            Constants.UvnsDigiPass = txbPassUvnc.Text;

            XmlElement digissh = xmlDoc.CreateElement("DigiSsh");
            digissh.SetAttribute("login", txbLoginPuttyDigi.Text);
            digissh.InnerText = txbPassPuttyDigi.Text;
            xmlDoc.DocumentElement.AppendChild(digissh);
            Constants.DigiSshLogin = txbLoginPuttyDigi.Text;
            Constants.DigiSshPass = txbPassPuttyDigi.Text;

            XmlElement salepoint = xmlDoc.CreateElement("Salepoint");
            salepoint.SetAttribute("login", txbLoginSalepoint.Text);
            salepoint.InnerText = txbPassSalepoint.Text;
            xmlDoc.DocumentElement.AppendChild(salepoint);
            Constants.SalepointLogin = txbLoginSalepoint.Text;
            Constants.SalepointPass = txbPassSalepoint.Text;

            //XmlElement srvsmenaroot = xmlDoc.CreateElement("SrvSmenaRoot");
            //if (rbRoot.Checked)
            //    loginSrvSsh = "root";
            //if (rbSmena.Checked)
            //    loginSrvSsh = "smena";
            //srvsmenaroot.SetAttribute("login", loginSrvSsh);
            //srvsmenaroot.InnerText = txbPassSmenaRoot.Text;
            //xmlDoc.DocumentElement.AppendChild(srvsmenaroot);
            //Constants.SrvSmenaRootLogin = loginSrvSsh;
            //Constants.SrvSmenaRootPass = txbPassSmenaRoot.Text;

            //XmlElement srvspdaemon = xmlDoc.CreateElement("SrvSpdaemon");
            //srvspdaemon.SetAttribute("login", "spdaemon");
            //srvspdaemon.InnerText = txbPassSpdaemon.Text;
            //xmlDoc.DocumentElement.AppendChild(srvspdaemon);
            //Constants.SrvSpdaemonLogin = "spdaemon";
            //Constants.SrvSpdaemonPass = txbPassSpdaemon.Text;

            XmlElement digitprice = xmlDoc.CreateElement("Digitprice");
            digitprice.SetAttribute("login", txbLoginDigitprice.Text);
            digitprice.InnerText = txbPassDigitprice.Text;
            xmlDoc.DocumentElement.AppendChild(digitprice);
            Constants.DigitpriceLogin = txbLoginDigitprice.Text;
            Constants.DigitpricePass = txbPassDigitprice.Text;

            XmlElement database = xmlDoc.CreateElement("database");
            database.SetAttribute("login", txbLoginBd.Text);
            database.InnerText = txbPassBd.Text;
            xmlDoc.DocumentElement.AppendChild(database);
            Constants.BdLogin = txbLoginBd.Text;
            Constants.BdLogin = txbPassBd.Text;

            XmlElement sshpass = xmlDoc.CreateElement("SshPass");
            sshpass.InnerText = txbSSHPass.Text;
            xmlDoc.DocumentElement.AppendChild(sshpass);
            Constants.SshPass = txbSSHPass.Text;

            xmlDoc.Save("LockStorage");
            Close();

            Mode.mode = true;

            //Шифруем
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "Software\\SDTander";
            const string keyName = userRoot + "\\" + subkey;

            using (var aes = new AesCryptoServiceProvider())
            {
                // Создаем генератор случайных чисел
                var rnd = RNGCryptoServiceProvider.Create();
                // Создаем буфер, равный длине ключа, и длине вектора (16 байт)
                byte[] buff0 = new byte[aes.KeySize / 8],
                       buff1 = new byte[16];
                // Заполняем ключ случайными числами
                rnd.GetNonZeroBytes(buff0);
                // Заполняем вектор случайными числами
                rnd.GetNonZeroBytes(buff1);
                // Пишем ключ в реестр
                byte[] buff = new byte[buff0.Length + buff1.Length];
                Array.Copy(buff0, 0, buff, 0, buff0.Length);
                Array.Copy(buff1, 0, buff, buff0.Length, buff1.Length);
                Registry.SetValue(keyName, "key", buff, RegistryValueKind.Binary);

                Crypt.CryptFile("LockStorage", "LockStorage", aes, buff0, buff1);

                File.Replace("LockStorage.crypt", "LockStorage", "LockStorage.back");
                File.Delete("LockStorage.back");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Учетные данные не заданы. ПО будет работать\nв режиме ручного ввода учетных данных.\nВы можете в любое время изменить режим работы,\nзаполнив форму учетных данных.\nВызов формы доступен в меню Файл.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Mode.mode = false;
            if (File.Exists("LockStorage"))
                File.Delete("LockStorage");
            Close();
        }

        private void SetPasswords_Load(object sender, EventArgs e)
        {
            TopMost = true;
        }

    }
}
