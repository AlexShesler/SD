using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml;
using System.Globalization;
using System.Net;
using System.Xml;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Security.Cryptography;
using JRO;
using ADODB;
using System.Data.SqlClient;

namespace SD
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            SetRights();
            ReadConfig();
            //LoadAnswers();
            //LoadDecision();
            LoadUseful();
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("ru-RU"));
            GetAutoUpdate();
            //FillDatabase();
            LoadComboBox();
            SetTopMostForm();
        }

        //Переменные
        string line;
        int prbarCount = 0;
        bool autoupdate;
        string pathUpdate, pathPutty, pathRms, pathUvnc, upHost, pathWinscp, subnetIp;
        bool update = false;
        bool topMostForm;
        int[] SrvArr = new int[13] { 1, 2, 3, 5, 6, 7, 18, 250, 253, 254, 87, 93, 141 };
        int[] IpmiArr = new int[7] { 217, 218, 12, 13, 14, 15, 16 };

        string[] subnetItems;
        Color[] colors = { Color.White, Color.Black };

        DataSet dsHm = new DataSet();

        public void SetRights()
        {
            //if (Constants.UserStatus != "fullrights") tabPageDelivery.Parent = null;
            //if (Constants.UserLogin == "shesler_au") btnTabPageDelivery.Visible = true;

            //foreach (Control control in this.Controls)
            //    if (control is Button)
            //        control.ForeColor = control.Enabled == true ? Color.Cyan : Color.Gray;
        }

        public void InsertDataToDB()
        {
            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(Application.StartupPath + @"\connection.xlsx"); //название файла Excel                                             
            xlSht = xlWB.Worksheets[1]; //название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];
            int iLastRow = xlSht.Cells[xlSht.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;  //последняя заполненная строка в столбце А            
            var arrDataList = (object[,])xlSht.Range["A1:F" + iLastRow].Value; //берём данные с листа Excel
            //xlApp.Visible = true; //отображаем Excel     
            xlWB.Close(false); //закрываем книгу, изменения не сохраняем
            xlApp.Quit(); //закрываем Excel
            GC.Collect(); // убрать за собой -- в том числе не используемые явно объекты !

            int RowsCount = arrDataList.GetUpperBound(0);
            int ColumnsCount = arrDataList.GetUpperBound(1);

            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DB.mdb;Jet OLEDB:Database Password=parolDlya_BD;");
            OleDbCommand commandDel = new OleDbCommand("DELETE FROM PASS_MD_MK", connection);
            connection.Open();
            commandDel.ExecuteNonQuery();
            connection.Close();
            CompactAccessDB("DB.mdb", "DB_compact.mdb");
            File.Delete("DB.mdb");
            File.Move("DB_compact.mdb", "DB.mdb");
            connection.Open();

            for (int i = 2; i <= RowsCount; i++)
            {
                string query = "INSERT INTO PASS_MD_MK (filial, rms_1, dbase, server, mail) VALUES (@filial, @rms_1, @dbase, @server, @mail )";

                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@filial", arrDataList[i, 1].ToString());
                command.Parameters.AddWithValue("@rms_1", arrDataList[i, 2].ToString());
                command.Parameters.AddWithValue("@dbase", arrDataList[i, 4].ToString());
                command.Parameters.AddWithValue("@server", arrDataList[i, 5].ToString());
                command.Parameters.AddWithValue("@mail", arrDataList[i, 6].ToString());
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public void ReadConfig()
        {
            StreamReader str = new StreamReader("config", Encoding.Default);

            while (!str.EndOfStream)
            {
                string cline = str.ReadLine();
                if (cline.StartsWith("autoupdate"))
                {
                    string[] i = cline.Split('|');
                    if (i[1] == "true") autoupdate = true;
                    else autoupdate = false;
                }
                if (cline.StartsWith("pathupdate"))
                {
                    string[] i = cline.Split('|');
                    pathUpdate = i[1];
                }
                if (cline.StartsWith("putty"))
                {
                    string[] i = cline.Split('|');
                    pathPutty = i[1];
                }
                if (cline.StartsWith("rms"))
                {
                    string[] i = cline.Split('|');
                    pathRms = i[1];
                }
                if (cline.StartsWith("uvnc"))
                {
                    string[] i = cline.Split('|');
                    pathUvnc = i[1];
                }
                if (cline.StartsWith("uphost"))
                {
                    string[] i = cline.Split('|');
                    upHost = i[1];
                }
                if (cline.StartsWith("topmost"))
                {
                    string[] i = cline.Split('|');
                    if (i[1] == "true") topMostForm = true;
                    else topMostForm = false;
                }
                if (cline.StartsWith("winscp"))
                {
                    string[] i = cline.Split('|');
                    pathWinscp = i[1];
                }
            }

            str.Close();

        }

        public void CreatDbGM()
        {
            prbarCount = 0;
            try
            {
                //получаем полный код html страницы
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                StringBuilder txt = new StringBuilder();
                WebRequest req = WebRequest.Create(pathUpdate);
                WebResponse resp = req.GetResponse();
                using (System.IO.Stream stream = resp.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        txt.AppendLine(sr.ReadToEnd());
                    }
                }
                StreamWriter swGetHtml = new StreamWriter(@"db.txt", false, new UTF8Encoding(false));
                swGetHtml.Write(txt);
                swGetHtml.Close();
                //Начинаем парсить
                StreamReader str = new StreamReader("db.txt", Encoding.UTF8);
                StringBuilder sb = new StringBuilder();
                while (!str.EndOfStream)
                {
                    string st = str.ReadLine();
                    if (st.StartsWith("<tr>"))
                    {
                        //удаляем ненужный текст в конце
                        int ind = st.IndexOf("</table>");
                        st = st.Remove(ind);
                        //пишем в stringBuilder
                        sb.AppendLine(st);
                        str.Close();
                        break;// останавливаем цикл
                    }
                }
                //Заменяем теги
                sb.Replace("</tr>", "\n");
                sb.Replace("</td><td>", "\t");
                sb.Replace("<tr><td>", "");
                sb.Replace("</td>", "");

                //Пишем результат в файл
                StreamWriter sw = new StreamWriter("db.txt", false, new UTF8Encoding(false));
                sw.Write(sb);
                sw.Close();
                //Удаляем пустые строки и считаем колличество
                StreamReader strCountGM = new StreamReader("db.txt", Encoding.UTF8);
                StringBuilder sbWithoutEmptyString = new StringBuilder();
                while (!strCountGM.EndOfStream)
                {
                    string line = strCountGM.ReadLine();
                    if (line != "")
                    {
                        sbWithoutEmptyString.AppendLine(line);
                        prbarCount++;
                    }
                }
                strCountGM.Close();
                StreamWriter swWithoutEmptyString = new StreamWriter("db.txt", false, new UTF8Encoding(false));
                swWithoutEmptyString.Write(sbWithoutEmptyString);
                swWithoutEmptyString.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка обновления БД");
                return;
            }
        }

        public void GetAutoUpdate()
        {
            if (autoupdate)
                CreatDbGM();
        }

        public void LoadComboBox()
        {
            ////992320	Абинск 1 Колхозная	10.2.68.1	 Новороссийск	2.1.6.2.1	21.62.1	000	+0	1	1	17.09.2010	открыт	присутствует
            //progressBar.Value = 0;
            //progressBar.Maximum = prbarCount;
            //progressBar.Step = 1;
            //int countGM = 0, openGM = 0;
            //StreamReader file = new StreamReader("db.txt");
            //while ((line = file.ReadLine()) != null)
            //{
            //    string[] array = line.Split('\t');
            //    cbNameGM.Items.Add(array[1]);
            //    cbCodeGM.Items.Add(array[0]);
            //    cbIpGM.Items.Add(array[2]);
            //    countGM++;
            //    if (array[11] == "принят")
            //        openGM++;
            //    progressBar.PerformStep();
            //}
            //file.Close();
            ////txbAllGM.Text = "Всего ГМ:  " + countGM.ToString() + " Принято:  " + openGM;
            
            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DB.mdb;Jet OLEDB:Database Password=parolDlya_BD;");
            connection.Open();
            OleDbDataAdapter ole_adapter = new OleDbDataAdapter("SELECT * FROM HM", connection);
            ole_adapter.Fill(dsHm);

            cbCodeGM.DataSource = dsHm.Tables[0];
            cbCodeGM.DisplayMember = "code";
            cbCodeGM.ValueMember = "code";

            cbNameGM.DataSource = dsHm.Tables[0];
            cbNameGM.DisplayMember = "hm_name";
            cbNameGM.ValueMember = "hm_name";

            cbIpGM.DataSource = dsHm.Tables[0];
            cbIpGM.DisplayMember = "ip_hm";
            cbIpGM.ValueMember = "ip_hm";

            cbCodeGM.SelectedIndex = -1;
            cbNameGM.SelectedIndex = -1;
            cbIpGM.SelectedIndex = -1;

            connection.Close();
        }

        public void GetLineAndFillBoxes(string param)
        {
            //StreamReader file = new StreamReader("db.txt");
            //while ((line = file.ReadLine()) != null)
            //{

            //    if (param == "name")
            //    {
            //        if (line.Contains(cbNameGM.Items[cbNameGM.SelectedIndex].ToString()))
            //            FillExceptNameGM();
            //    }
            //    if (param == "code")
            //    {
            //        if (line.Contains(cbCodeGM.Items[cbCodeGM.SelectedIndex].ToString()))
            //            FillExceptCodeGM();
            //    }
            //    if (param == "ip")
            //    {
            //        if (line.Contains(cbIpGM.Items[cbIpGM.SelectedIndex].ToString()))
            //            FillExceptIpGM();
            //    }
            //}
            //file.Close();

            //FillIp();
            //progressBar.Value = 0;
            //cbNameGM.SelectionLength = 0;
            //cbNameGM.SelectionStart = cbNameGM.Text.Length;
        }

        public void FillExceptNameGM()
        {
            //997424  Белорецк 1 Гафури(а)   10.3.16.1    Магнитогорск   2.1.5.7 21.57   21.57 + 2  1   1   31.10.2014  открыт присутствует
            string[] array = line.Split('\t');
            subnetIp = array[2];

            //вырезаем первые 3 октета IP
            string[] arrip = array[2].Split('.');
            //txbIpScalesStart.Text = "";
            //for (int i = 0; i < 3; i++)
            //{
            //    txbIpScalesStart.Text += arrip[i] + '.';
            //}
            cbIpScalesStart.Items.Clear();
            string ipPlus = (Convert.ToInt32(arrip[2]) + 1).ToString();
            cbIpScalesStart.Items.Add(arrip[0] + '.' + arrip[1] + '.' + arrip[2] + '.');
            cbIpScalesStart.Items.Add(arrip[0] + '.' + arrip[1] + '.' + ipPlus + '.');
            cbIpScalesStart.SelectedIndex = 0;

            subnetItems = cbIpScalesStart.Items.Cast<string>().ToArray();

            txbFilial.Text = array[3];
            txbDateOpen.Text = array[10];
            txbTimeGM.Text = array[7];

            cbCodeGM.SelectedIndex = cbCodeGM.FindString(array[0]);
            cbIpGM.SelectedIndex = cbIpGM.FindString(array[2]);

            if (array[11] == "принят")
                lblStatusGM.Text = "";
            else
            {
                lblStatusGM.Text = array[11];
                lblStatusGM.BackColor = Color.Red;
            }

        }

        public void FillExceptCodeGM()
        {
            string[] array = line.Split('\t');
            subnetIp = array[2];
            //вырезаем первые 3 октета IP
            string[] arrip = array[2].Split('.');
            //txbIpScalesStart.Text = "";
            //for (int i = 0; i < 3; i++)
            //{
            //    txbIpScalesStart.Text += arrip[i] + '.';
            //}
            cbIpScalesStart.Items.Clear();
            string ipPlus = (Convert.ToInt32(arrip[2]) + 1).ToString();
            cbIpScalesStart.Items.Add(arrip[0] + '.' + arrip[1] + '.' + arrip[2] + '.');
            cbIpScalesStart.Items.Add(arrip[0] + '.' + arrip[1] + '.' + ipPlus + '.');
            cbIpScalesStart.SelectedIndex = 0;

            txbFilial.Text = array[3];
            txbDateOpen.Text = array[10];
            txbTimeGM.Text = array[7];

            cbNameGM.SelectedIndex = cbNameGM.FindString(array[1]);
            cbIpGM.SelectedIndex = cbIpGM.FindString(array[2]);

            if (array[11] == "принят")
                lblStatusGM.Text = "";
            else
            {
                lblStatusGM.Text = array[11];
                lblStatusGM.BackColor = Color.Red;
            }
        }

        public void FillExceptIpGM()
        {
            //992304	Краснодар 3 Солнечная	10.2.120.1	Краснодар	2.1.3.0	21.30	21.30	+0	1	1	12.11.2008
            string[] array = line.Split('\t');
            subnetIp = array[2];
            //вырезаем первые 3 октета IP
            string[] arrip = array[2].Split('.');
            //txbIpScalesStart.Text = "";
            //for (int i = 0; i < 3; i++)
            //{
            //    txbIpScalesStart.Text += arrip[i] + '.';
            //}

            cbIpScalesStart.Items.Clear();
            string ipPlus = (Convert.ToInt32(arrip[2]) + 1).ToString();
            cbIpScalesStart.Items.Add(arrip[0] + '.' + arrip[1] + '.' + arrip[2] + '.');
            cbIpScalesStart.Items.Add(arrip[0] + '.' + arrip[1] + '.' + ipPlus + '.');
            cbIpScalesStart.SelectedIndex = 0;

            txbFilial.Text = array[3];
            txbDateOpen.Text = array[10];
            txbTimeGM.Text = array[7];

            cbNameGM.SelectedIndex = cbNameGM.FindString(array[1]);
            cbCodeGM.SelectedIndex = cbCodeGM.FindString(array[0]);

            if (array[11] == "принят")
                lblStatusGM.Text = "";
            else
            {
                lblStatusGM.Text = array[11];
                lblStatusGM.BackColor = Color.Red;
            }
        }

        private void FillIp(string ipHm)
        {
            txbRobotPass.Text = "";
            txbRobotName.Text = "";

            string[] arrip = ipHm.Split('.');

            int bit = Convert.ToInt16(arrip[2]) + 1;
            string ip = arrip[0] + '.' + arrip[1] + '.' + arrip[2] + '.';
            string ipplus = arrip[0] + '.' + arrip[1] + '.' + bit.ToString() + '.';

            toolSshIbgm.Tag = ip + "1";
            toolWinscpIbgm.Tag = ip + "1";
            toolSshIbgmBack.Tag = ip + "2";
            toolWinscpIbgmBack.Tag = ip + "2";
            toolRmsTerm.Tag = ip + "3";
            toolRdpTerm.Tag = ip + "3";
            toolSshZope.Tag = ip + "5";
            toolWinscpZope.Tag = ip + "5";
            toolRmsWinBack.Tag = ip + "6";
            toolRdpWinBack.Tag = ip + "6";
            toolSshDp.Tag = ip + "18";
            toolRmsUtm.Tag = ipplus + "93";
            toolRmsScala.Tag = ipplus + "87";

            toolIpmiIbgm.Tag = ip + "217";
            toolIpmiIbgmBack.Tag = ip + "218";
            toolIpmiZope.Tag = ip + "15";
            toolIpmiTerm.Tag = ip + "12";
            toolIpmiWinBack.Tag = ip + "16";

            lblPing1.Tag = ip + "1";
            lblPing2.Tag = ip + "2";
            lblPing3.Tag = ip + "3";
            lblPing5.Tag = ip + "5";
            lblPing6.Tag = ip + "6";
            lblPing7.Tag = ip + "7";
            lblPing18.Tag = ip + "18";
            lblPing93.Tag = ipplus + "93";
            lblPing250.Tag = ip + "250";
            lblPing253.Tag = ip + "253";
            lblPing141.Tag = ipplus + "141";
            lblPing254.Tag = ip + "254";
        }

        public bool IsGmSelected()
        {
            if (cbNameGM.Text == "")
            {
                MessageBox.Show("ВротМнеТапки! Не позорь мою лысую голову!\nГипер кто за тебя выберет?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; ;
            }
            else
                return true;
        }

        public void GetPassRobot()
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("robotpass");

                XmlElement xRoot = xDoc.DocumentElement;
                // обход всех узлов в корневом элементе
                foreach (XmlNode xnode in xRoot)
                {
                    // обходим все дочерние узлы элемента taskparams
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        // если узел - UserName
                        if (childnode.Name == "UserName")
                        {
                            XmlNode attr = childnode.Attributes.GetNamedItem("value");
                            txbRobotName.Text = attr.Value;
                        }
                        // если узел Password
                        if (childnode.Name == "Password")
                        {
                            XmlNode attr = childnode.Attributes.GetNamedItem("value");
                            txbRobotPass.Text = attr.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось получить пароль. Проверьте наличие файла на сервере IGBM.\n Возможно он имеет отличную от стандарта структуру.\n Подробно:\n" + ex.ToString(), "Ошибка!");
            }        
        }

        public void SrvRmsConnect(string ip)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = pathRms;
            string arg = " /d " + '\u0022' + pathRms + '\u0022' + " /name:" + '\u0022' + cbCodeGM.Text + " " + cbNameGM.Text + " " + ip + '\u0022' + " /create /host:" + ip + " /FULLCONTROL";
            psi.Arguments = arg;
            Process.Start(psi);
        }

        public IPStatus Ping(string host)
        {
            IPStatus status = IPStatus.Unknown;
            try
            {
                status = new Ping().Send(host, 3000).Status;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка выполнения Ping", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return (status);
        }

        public void PingSrv(string server, string host)
        {
            Label lbl = tpHM.Controls["gbServersAvailability"].Controls["lblPing" + server] as Label;
            IPStatus status = IPStatus.Unknown;
            try
            {
                status = new Ping().Send(host, 3000).Status;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка выполнения Ping", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (status == IPStatus.Success)
            {
                //pingstatus = true;
                lbl.BackColor = Color.Green;
                lbl.ForeColor = Color.White;
            }
            else
            {
                lbl.BackColor = Color.Red;
            }
        }

        public void PingT(string ip)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd";
            psi.Arguments = @"/c ping " + ip + " -t";
            Process.Start(psi);
        }

        public void ProcStart(string process, string argument)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = process;
            psi.Arguments = argument;
            Process.Start(psi);
        }

        public void SrvRdpConnect(string ip)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "mstsc";
            string arg = "/v:" + ip;
            psi.Arguments = arg;
            Process.Start(psi);
        }

        public void GetMode()
        {
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "Software\\SDTander";
            const string keyName = userRoot + "\\" + subkey;
            string key = "false";
            byte[] buff;


            if (File.Exists("LockStorage"))
            {
                try
                {
                    //key = Registry.GetValue(keyName, "key", "false").ToString();
                    var result = Registry.GetValue(keyName, "key", false)?.ToString();
                    key = result == null ? "false" : result.ToString();
                }
                catch (Exception ex)
                {
                    if (key != "false")
                    {
                        MessageBox.Show(ex.ToString(), "Ошибка получения ключа");
                        return;
                    }
                }
                if (key == "false")
                {
                    File.Delete("LockStorage");
                    DialogResult result = MessageBox.Show("Не удалось получить учетные данные для подключения\nлибо это первый запуск ПО.\nНеобходимо заполнить форму для автоматического ввода\nучетных данных при подключении к серверам.\nВ случае отказа, при подключении необходимо будет\nсамостоятельно указывать логин и пароль.", "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.OK)
                    {
                        SetPasswords fsp = new SetPasswords();
                        fsp.ShowDialog();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        Mode.mode = false;
                        return;
                    }
                }
                else
                {
                    Mode.mode = true;
                    //дешифруем
                    //Получаем ключ
                    try
                    {
                        buff = (byte[])Registry.GetValue(keyName, "key", "false");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Ошибка получения ключа");
                        return;
                    }

                    using (var aes = new AesCryptoServiceProvider())
                    {
                        byte[] buff0 = new byte[aes.KeySize / 8],
                               buff1 = new byte[16];
                        //Делим на ключ и вектор
                        Array.Copy(buff, 0, buff0, 0, buff0.Length);
                        Array.Copy(buff, buff0.Length, buff1, 0, buff1.Length);
                        //Расшифровываем
                        try
                        {
                            Crypt.DecryptFile("LockStorage", "LockStorage", aes, buff0, buff1);
                        }
                        catch (Exception ex)
                        {
                            File.Delete("LockStorage");
                            MessageBox.Show(ex.ToString(), "Ошибка ключа");
                            DialogResult result = MessageBox.Show("Ключ не валиден.\nНеобходимо заполнить форму\nдля автоматического ввода учетных данных\nпри подключении к серверам.\nВ случае отказа, при подключении необходимо будет\nсамостоятельно указывать логин и пароль.", "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                            if (result == DialogResult.OK)
                            {
                                SetPasswords fsp = new SetPasswords();
                                fsp.ShowDialog();
                            }
                            else if (result == DialogResult.Cancel)
                            {
                                Mode.mode = false;
                                return;
                            }
                        }

                        if (File.Exists("LockStorage.decrypt"))
                        {
                            File.Replace("LockStorage.decrypt", "LockStorage", "LockStorage.back");
                        }
                        File.Delete("LockStorage.back");
                    }
                    SetConstants();
                    //шифруем обратно
                    if (Constants.Mode)
                    {
                        using (var aes = new AesCryptoServiceProvider())
                        {
                            byte[] buff0 = new byte[aes.KeySize / 8],
                                   buff1 = new byte[16];

                            //Делим на ключ и вектор
                            Array.Copy(buff, 0, buff0, 0, buff0.Length);
                            Array.Copy(buff, buff0.Length, buff1, 0, buff1.Length);

                            Crypt.CryptFile("LockStorage", "LockStorage", aes, buff0, buff1);

                            File.Replace("LockStorage.crypt", "LockStorage", "LockStorage.back");
                            File.Delete("LockStorage.back");
                        }
                    }
                    else return;
                }
            }
            else
            {
                DialogResult result = MessageBox.Show("Не удалось получить учетные данные для подключения\nлибо это первый запуск ПО.\nНеобходимо заполнить форму для автоматического ввода\nучетных данных при подключении к серверам.\nВ случае отказа, при подключении необходимо будет\nсамостоятельно указывать логин и пароль.", "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    SetPasswords fsp = new SetPasswords();
                    fsp.ShowDialog();
                }
            }
        }

        public void SetConstants()
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load("LockStorage");
                Constants.Mode = true;
            }
            catch
            {
                File.Delete("LockStorage");
                MessageBox.Show("Ошибка заполнения констант. Расшифровка не удалась.\nВозможно был поврежден ключ или файл с данными.\nВключен режим ручного ввода паролей при подключении.\nВыберите в меню Файл пункт Задать пароли для изменения режима на автоматический.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Mode.mode = false;
                Constants.Mode = false;
                return;
            }


            XmlElement xRoot = xDoc.DocumentElement;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Name == "Uvnc")
                {
                    Constants.UvnsDigiPass = xnode.InnerText;
                }
                if (xnode.Name == "DigiSsh")
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("login");
                    Constants.DigiSshLogin = attr.Value;
                    Constants.DigiSshPass = xnode.InnerText;
                }
                if (xnode.Name == "Salepoint")
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("login");
                    Constants.SalepointLogin = attr.Value;
                    Constants.SalepointPass = xnode.InnerText;
                }
                if (xnode.Name == "SrvSpdaemon")
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("login");
                    Constants.SrvSpdaemonLogin = attr.Value;
                    Constants.SrvSpdaemonPass = xnode.InnerText;
                }
                if (xnode.Name == "Digitprice")
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("login");
                    Constants.DigitpriceLogin = attr.Value;
                    Constants.DigitpricePass = xnode.InnerText;
                }
                if (xnode.Name == "database")
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("login");
                    Constants.BdLogin = attr.Value;
                    Constants.BdPass = xnode.InnerText;
                }
            }
        }

        public void SetTopMostForm()
        {
            if (topMostForm == true)
            {
                TopMost = true;
                chbOver.Checked = true;
            }
        }

        public int GetVersion(string fileName)
        {
            string version = "";
            string verConcat = "";
            XmlDocument xDoc = new XmlDocument();

            try
            {
                xDoc.Load(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "File not found");
                return (0);
            }


            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Name == "version")
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("ver");
                    version = attr.Value;
                }
            }
            string[] verArr = version.Split('.');

            for (int i = 0; i < verArr.Length; i++)
            {
                if (verArr[i].Length < 2)
                {
                    verArr[i] = "0" + verArr[i];
                    verConcat += verArr[i];
                }
                else verConcat += verArr[i];
            }
            return Convert.ToInt32(verConcat);
        }

        public void CheckUpdate()
        {
            int versionNew;
            int versionOld;

            try
            {
                WebClient client = new WebClient() { Proxy = null };
                if (File.Exists(@"\\" + upHost + @"\Update\Updater.exe"))
                    client.DownloadFile(@"\\" + upHost + @"\Update\Updater.exe", "Updater.exe");

                if (File.Exists(@"\\" + upHost + @"\Update\version.xml"))
                {
                    client.DownloadFile(@"\\" + upHost + @"\Update\version.xml", "version_new.xml");
                }
                else return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }


            versionNew = GetVersion("version_new.xml");
            versionOld = GetVersion("version.xml");

            if (versionNew > versionOld)
            {
                UpdateInfo form = new UpdateInfo();
                update = true;
                form.ShowDialog();
            }
            else
            {
                File.Delete("version_new.xml");
                return;
            }

        }

        private void LoadUseful()
        {
            string fileName = "useful.txt";
            if (File.Exists(fileName))
            {
                var sr = new StreamReader(fileName, Encoding.UTF8);
                string text = sr.ReadToEnd();
                rtbUseful.AppendText(text);
                sr.Close();
            }
        }

        public void findUseful(int i)
        {
            string s = rtbUseful.Text, s2 = txbFindUseful.Text;

            if (s.Contains(s2))
            {
                i = s.IndexOf(s2, i);
                if (i == -1)
                    i = s.IndexOf(s2, 0);

                rtbUseful.Select(i, s2.Length);
                rtbUseful.Focus();
            }
        }

        private void SetColor(Control control, Color color, Color forecolor)
        {
            control.BackColor = color;
            control.ForeColor = forecolor;

            if (control.HasChildren)
            {
                // Recursively call this method for each child control.
                foreach (Control childControl in control.Controls)
                {
                    SetColor(childControl, color, forecolor);
                }
            }
        }

        public object[,] GetObjectList() //получаем список ТО из xls
        {
            //считываем данные из Excel файла в двумерный массив
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(Application.StartupPath + @"\all_reports.xls"); //название файла Excel                                             
            xlSht = xlWB.Worksheets[1]; //название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];
            int iLastRow = xlSht.Cells[xlSht.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;  //последняя заполненная строка в столбце А            
            var arrDataList = (object[,])xlSht.Range["A1:Q" + iLastRow].Value; //берём данные с листа Excel
            //xlApp.Visible = true; //отображаем Excel     
            xlWB.Close(false); //закрываем книгу, изменения не сохраняем
            xlApp.Quit(); //закрываем Excel
            GC.Collect(); // убрать за собой -- в том числе не используемые явно объекты !

            return (arrDataList);
        }

        public static void CompactAccessDB(string original, string copy) //сжатие БД
        {
            //http://www.codeproject.com/KB/database/mdbcompact_latebind.aspx
            //подключен COM-ОБЪЕКТ - microsoft jet and replication objects 2.6 library
            //сжаие базы данных compact MS ACCESS

            //object[] oParams;
            JRO.JetEngine je = new JRO.JetEngine();
            //je.CompactDatabase(connectionString, mdwfilename);
            je.CompactDatabase("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + original + ";Jet OLEDB:Database Password=parolDlya_BD;Jet OLEDB:Engine Type=5",
                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + copy + ";Jet OLEDB:Database Password=parolDlya_BD;Jet OLEDB:Engine Type=5");
            return;
        }

        public void FillDatabase()
        {
            //-------------запуск таймера-------------------//
            Stopwatch sw_total = new Stopwatch();
            sw_total.Start();

            //Код Магазин Статус Тип Филиал Код Филиала Дата открытия Дата закрытия Email   Телефон объекта Растояние Центр поддержки Код центра поддержки    ФИО системотехника  Телефон системотехника  Адрес Полный адрес
            //230039  Мишутка Закрыт  МД Краснодар Восток    235800  1998 - 11 - 17 00:00:00 2008 - 03 - 01 00:00:00                     Григорян Андрей Аванесович


            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DB.mdb;Jet OLEDB:Database Password=parolDlya_BD;");

            //Очистка таблиц MD_MK и HM
            connection.Open();
            OleDbCommand commandDelMd_Mk = new OleDbCommand("DELETE FROM MD_MK", connection);
            OleDbCommand commandDel_Hm = new OleDbCommand("DELETE FROM HM", connection);

            commandDelMd_Mk.ExecuteNonQuery();
            commandDel_Hm.ExecuteNonQuery();
            connection.Close();

            //сжатие БД
            CompactAccessDB("DB.mdb", "DB_compact.mdb");
            File.Delete("DB.mdb");
            File.Move("DB_compact.mdb", "DB.mdb");

            connection.Open();
            //Получить список ТО
            object[,] arrDataList = GetObjectList();

            int RowsCount = arrDataList.GetUpperBound(0);
            int ColumnsCount = arrDataList.GetUpperBound(1);

            //заполняем таблицы из xls
            for (int i = 2; i <= RowsCount; i++)
            {
                if (arrDataList[i, 4].ToString() == "МД" || arrDataList[i, 4].ToString() == "МК")
                {
                    string query = "INSERT INTO MD_MK (code, mm_name, status, type, filial, date_open, date_close) " +
                                    "VALUES (@code, @hm_name, @status, @type, @filial, @date_open, @date_close)";
                    OleDbCommand command = new OleDbCommand(query, connection);

                    if (arrDataList[i, 1] != null) 
                    { 
                        command.Parameters.AddWithValue("@code", arrDataList[i, 1].ToString()); 
                    }else command.Parameters.AddWithValue("@code", DBNull.Value);
                    if (arrDataList[i, 2] != null)
                    {
                        command.Parameters.AddWithValue("@mm_name", arrDataList[i, 2].ToString());
                    }else command.Parameters.AddWithValue("@mm_name", DBNull.Value);
                    if (arrDataList[i, 3] != null)
                    {
                        command.Parameters.AddWithValue("@status", arrDataList[i, 3].ToString());
                    }else command.Parameters.AddWithValue("@status", DBNull.Value);
                    if (arrDataList[i, 4] != null)
                    {
                        command.Parameters.AddWithValue("@type", arrDataList[i, 4].ToString());
                    }else command.Parameters.AddWithValue("@type", DBNull.Value);
                    if (arrDataList[i, 5] != null)
                    {
                        command.Parameters.AddWithValue("@filial", arrDataList[i, 5].ToString());
                    }else command.Parameters.AddWithValue("@filial", DBNull.Value);
                    if (arrDataList[i, 7] != null)
                    {
                        command.Parameters.AddWithValue("@date_open", arrDataList[i, 7].ToString());
                    }
                    else command.Parameters.AddWithValue("@date_open", DBNull.Value);
                    if (arrDataList[i, 8] != null)
                    {
                        command.Parameters.AddWithValue("@date_close", arrDataList[i, 8].ToString());
                    }
                    else command.Parameters.AddWithValue("@date_close", DBNull.Value);

                    command.ExecuteNonQuery();
                }
                else if (arrDataList[i, 4].ToString() == "ГМ")
                {
                    string query = "INSERT INTO HM (code, hm_name, status, type, filial, date_open, date_close) " +
                                    "VALUES (@code, @hm_name, @status, @type, @filial, @date_open, @date_close)";
                    OleDbCommand command = new OleDbCommand(query, connection);

                    if (arrDataList[i, 1] != null)
                    {
                        command.Parameters.AddWithValue("@code", arrDataList[i, 1].ToString());
                    }
                    else command.Parameters.AddWithValue("@code", DBNull.Value);
                    if (arrDataList[i, 2] != null)
                    {
                        command.Parameters.AddWithValue("@hm_name", arrDataList[i, 2].ToString());
                    }
                    else command.Parameters.AddWithValue("@hm_name", DBNull.Value);
                    if (arrDataList[i, 3] != null)
                    {
                        command.Parameters.AddWithValue("@status", arrDataList[i, 3].ToString());
                    }
                    else command.Parameters.AddWithValue("@status", DBNull.Value);
                    if (arrDataList[i, 4] != null)
                    {
                        command.Parameters.AddWithValue("@type", arrDataList[i, 4].ToString());
                    }
                    else command.Parameters.AddWithValue("@type", DBNull.Value);
                    if (arrDataList[i, 5] != null)
                    {
                        command.Parameters.AddWithValue("@filial", arrDataList[i, 5].ToString());
                    }
                    else command.Parameters.AddWithValue("@filial", DBNull.Value);
                    if (arrDataList[i, 7] != null)
                    {
                        command.Parameters.AddWithValue("@date_open", arrDataList[i, 7].ToString());
                    }
                    else command.Parameters.AddWithValue("@date_open", DBNull.Value);
                    if (arrDataList[i, 8] != null)
                    {
                        command.Parameters.AddWithValue("@date_close", arrDataList[i, 8].ToString());
                    }
                    else command.Parameters.AddWithValue("@date_close", DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }


            //заполняем ip и часовой пояс ГМ
            //992320	Абинск 1 Колхозная	10.2.68.1	 Новороссийск	2.1.6.2.1	21.62.1	000	+0	1	1	17.09.2010	открыт	присутствует
            StreamReader file = new StreamReader("db.txt");
            while ((line = file.ReadLine()) != null)
            {
                string[] array = line.Split('\t');
                string query = "UPDATE HM set ip_hm = @ip_hm, timezone = @timezone where code = @code";
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ip_hm", array[2]);
                command.Parameters.AddWithValue("@timezone", array[7]);
                command.Parameters.AddWithValue("@code", array[0]);

                command.ExecuteNonQuery();
            }
            file.Close();

            connection.Close();

            sw_total.Stop();
            MessageBox.Show(sw_total.ElapsedMilliseconds + " ms", "Time");
        }

        public void SrvShowHide(string mode)
        {
            if (mode == "show") this.Width = 875;
            else if (mode == "hide") this.Width = 586;
        }

        private void CbCodeGM_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    GetLineAndFillBoxes("code");
            //}
        }

        private void CbCodeGM_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //GetLineAndFillBoxes("code");
        }

        private void CbIpGM_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    GetLineAndFillBoxes("ip");
            //}
        }

        private void BtnGetNamePassRobot_Click(object sender, EventArgs e)
        {
            if (IsGmSelected())
            {
                string url = "";
                try
                {                    
                    url = "file://" + toolSshIbgm.Tag.ToString() + "/whs/update/sendsmtp.xml";

                    StringBuilder txt = new StringBuilder();
                    //txt.Append("<Password FrameClassName=\"TFrmEditText\" Caption=\"������\" value=\"AAAAaaaa1111111\"/>");

                    WebRequest req = WebRequest.Create(url);
                    WebResponse resp = req.GetResponse();

                    System.IO.Stream stream = resp.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    txt.AppendLine(sr.ReadToEnd());
                    sr.Close();
                    resp.Close();

                    StreamWriter sw = new StreamWriter("robotpass", false, Encoding.Default);
                    sw.Write(txt);
                    sw.Close();

                    GetPassRobot();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка подключения к серверу");
                    return;
                }
            }
        }

        private void BtnChangePassLang_Click(object sender, EventArgs e)
        {
            string newpassword = "";
            string password = txbRobotPass.Text;
            char[] pass = password.ToCharArray();
            char[] listrus = new char[76];
            char[] listeng = new char[76];
            int i = 0;
            StreamReader str = new StreamReader("listchange");
            while ((line = str.ReadLine()) != null)
            {
                char[] array = line.ToCharArray();
                listrus[i] = array[0];
                listeng[i] = array[2];
                i++;
            }
            str.Close();
            for (int k = 0; k < password.Length; k++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (pass[k] == listrus[j])
                    {
                        newpassword = newpassword + listeng[j];
                        continue;
                    }
                    else if (pass[k] == listeng[j])
                    {
                        newpassword = newpassword + listrus[j];
                        continue;
                    }
                }
            }

            txbRobotPass.Text = newpassword;
        }

        private void ToolRmsTerm_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsGmSelected())
            {
                if (e.Button == MouseButtons.Left)
                {
                    SrvRmsConnect(((ToolStripMenuItem)sender).Tag.ToString());
                }
                else if (e.Button == MouseButtons.Right)
                {
                    PingT(((ToolStripMenuItem)sender).Tag.ToString());
                }
            }            
        }

        private void ToolSshIbgm_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsGmSelected())
            {
                if (e.Button == MouseButtons.Left)
                {
                    if ((Mode.mode) & (((ToolStripMenuItem)sender).Name.ToString() != "toolSshDp"))
                        ProcStart(pathPutty, " -ssh -l " + Constants.UserLogin + " -pw " + Constants.UserPass + " " + (((ToolStripMenuItem)sender).Tag.ToString()));
                    else if (((ToolStripMenuItem)sender).Name.ToString() == "toolSshDp")
                        ProcStart(pathPutty, " -ssh -l " + Constants.DigitpriceLogin + " -pw " + Constants.DigitpricePass + " " + (((ToolStripMenuItem)sender).Tag.ToString()));
                    else
                        ProcStart(pathPutty, " -ssh " + (((ToolStripMenuItem)sender).Tag.ToString()));
                }
                else if (e.Button == MouseButtons.Right)
                {
                    PingT(((ToolStripMenuItem)sender).Tag.ToString());
                }
            }
        }

        private void toolRdpTerm_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsGmSelected())
                SrvRdpConnect(((ToolStripMenuItem)sender).Tag.ToString());
        }

        private void toolIpmiIbgm_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsGmSelected())
            {
                if (e.Button == MouseButtons.Right)
                {
                    PingT(((ToolStripMenuItem)sender).Tag.ToString());
                }
                else if (e.Button == MouseButtons.Left)
                {
                    string url = "http://" + ((ToolStripMenuItem)sender).Tag.ToString();
                    Process.Start(url);
                }
            }
                
        }

        private void btnScalesPing_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsGmSelected())
            {
                if (e.Button == MouseButtons.Left)
                {
                    IPStatus status = Ping(cbIpScalesStart.Text + txbIpScalesEnd.Text);

                    if (status == IPStatus.Success)
                    {
                        ((Button)sender).BackColor = Color.Green;
                        ((Button)sender).ForeColor = Color.White;
                    }
                    else
                    {
                        ((Button)sender).BackColor = Color.Red;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    PingT(cbIpScalesStart.Text + txbIpScalesEnd.Text);
                }
            }
            
        }

        private void btnPuttyDigi_Click(object sender, EventArgs e)
        {
            if (Mode.mode)
                ProcStart(pathPutty, " -ssh -l " + Constants.DigiSshLogin + " -pw " + Constants.DigiSshPass + " " + cbIpScalesStart.Text + txbIpScalesEnd.Text);
            else
                ProcStart(pathPutty, " -ssh " + cbIpScalesStart.Text + txbIpScalesEnd.Text);
        }

        private void btnCashCashier_Click(object sender, EventArgs e)
        {
            if (Mode.mode)
                ProcStart(pathPutty, " -ssh -l " + Constants.SalepointLogin + " -pw " + Constants.SalepointPass + " " + cbIpScalesStart.Text + txbIpScalesEnd.Text);
            else
                ProcStart(pathPutty, " -ssh " + cbIpScalesStart.Text + txbIpScalesEnd.Text);
        }

        private void btnUvnc_Click(object sender, EventArgs e)
        {
            if (Mode.mode)
                ProcStart(pathUvnc, "/password " + Constants.UvnsDigiPass + " " + cbIpScalesStart.Text + txbIpScalesEnd.Text);
            else
                ProcStart(pathUvnc, cbIpScalesStart.Text + txbIpScalesEnd.Text);
        }

        private void btnBizerba_Click(object sender, EventArgs e)
        {
            ProcStart("C:\\Program Files\\Internet Explorer\\iexplore.exe", "http://" + cbIpScalesStart.Text + txbIpScalesEnd.Text);
        }

        private void btnRmsConnect_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = pathRms;
            string ip = txbOtherRmsSsh.Text;
            string arg = "/c start /d " + '\u0022' + pathRms + '\u0022' + " /create /host:" + ip + " /FULLCONTROL";
            psi.Arguments = arg;
            Process.Start(psi);
        }

        private void btnRmsSshPing_MouseUp(object sender, MouseEventArgs e)
        {
            if (txbOtherRmsSsh.Text != "")
            {
                if (e.Button == MouseButtons.Left)
                {
                    IPStatus status = Ping(txbOtherRmsSsh.Text);

                    if (status == IPStatus.Success)
                    {
                        btnRmsSshPing.BackColor = Color.Green;
                        btnRmsSshPing.ForeColor = Color.White;
                    }
                    else
                    {
                        btnRmsSshPing.BackColor = Color.Red;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                    PingT(txbOtherRmsSsh.Text);
            }
        }

        private void btnSshConnect_Click(object sender, EventArgs e)
        {
            ProcStart(pathPutty, " -ssh " + txbOtherRmsSsh.Text);
        }

        private void chbOver_CheckedChanged(object sender, EventArgs e)
        {
            bool statusTopMost;
            if (chbOver.CheckState == CheckState.Checked) statusTopMost = true;
            else statusTopMost = false;

            TopMost = statusTopMost;
            string line = string.Empty;

            using (StreamReader str = new StreamReader("config", Encoding.Default))
            {
                while (!str.EndOfStream)
                {
                    string cline = str.ReadLine();
                    if (cline.StartsWith("topmost")) line = cline;
                }
            }

            string strUpdate = string.Empty;
            using (StreamReader reader = File.OpenText("config")) strUpdate = reader.ReadToEnd();

            string[] arrStr = line.Split('|');
            strUpdate = strUpdate.Replace(line, arrStr[0] + '|' + statusTopMost.ToString().ToLower());

            using (StreamWriter file = new StreamWriter("config"))
            {
                file.Write(strUpdate);
            }

        }

        private void cbIpScalesStart_SelectionChangeCommitted(object sender, EventArgs e)
        {
            btnScalesPing.BackColor = Color.WhiteSmoke;

            if (cbIpScalesStart.SelectedIndex == 1)
            {
                txbIpScalesEnd.ForeColor = Color.White;
                txbIpScalesEnd.BackColor = Color.Black;
            }
            else
            {
                txbIpScalesEnd.ForeColor = Color.Black;
                txbIpScalesEnd.BackColor = Color.White;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            cbIpScalesStart.DrawItem += cbIpScalesStart_DrawItem;
            cbIpScalesStart.DrawMode = DrawMode.OwnerDrawFixed;
        }

        private void cbIpScalesStart_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == 0)
            {
                using (Brush br = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(br, e.Bounds);
                    e.Graphics.DrawString(subnetItems[e.Index], e.Font, Brushes.Black, e.Bounds);
                }
            }
            else if(e.Index == 1)
            {
                using (Brush br = new SolidBrush(Color.Black))
                {
                    e.Graphics.FillRectangle(br, e.Bounds);
                    e.Graphics.DrawString(subnetItems[e.Index], e.Font, Brushes.White, e.Bounds);
                }
            }
            
        }

        private void toolWinscpIbgm_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsGmSelected())
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (Mode.mode)
                        ProcStart(pathWinscp, Constants.UserLogin + ":" + Constants.UserPass + "@" + (((ToolStripMenuItem)sender).Tag.ToString()));
                    else 
                        ProcStart(pathWinscp, Constants.UserLogin + "@" + (((ToolStripMenuItem)sender).Tag.ToString()));
                }
                else if (e.Button == MouseButtons.Right)
                {
                    PingT(((ToolStripMenuItem)sender).Tag.ToString());
                }
            }
        }

        private void menuUpdateBd_Click(object sender, EventArgs e)
        {
            CreatDbGM();
            LoadComboBox();
        }

        private void menuSetPassword_Click(object sender, EventArgs e)
        {
            SetPasswords fsp = new SetPasswords();
            fsp.ShowDialog();
        }

        private void menuSettigs_Click(object sender, EventArgs e)
        {
            Settings frmS = new Settings();
            frmS.ShowDialog();
        }

        private void menuRMSTerminal_Click(object sender, EventArgs e)
        {
            if (IsGmSelected())
            {
                if (((ToolStripMenuItem)sender).Tag.ToString() != "10.5.44.225")
                {
                    SrvRmsConnect(subnetIp.Remove(subnetIp.Length - 1, 1) + ((ToolStripMenuItem)sender).Tag.ToString());
                }
                else
                    SrvRmsConnect(((ToolStripMenuItem)sender).Tag.ToString());
            }
        }

        private void menuSshIbgm_Click(object sender, EventArgs e)
        {
            if (IsGmSelected())
            {
                string ip = subnetIp.Remove(subnetIp.Length - 1, 1) + ((ToolStripMenuItem)sender).Tag.ToString();
                if (Mode.mode)
                    ProcStart(pathPutty, " -ssh -l " + Constants.UserLogin + " -pw " + Constants.UserPass + " " + ip);
                else
                    ProcStart(pathPutty, " -ssh " + ip);
            }            
        }

        private void menuDP_Click(object sender, EventArgs e)
        {
            if (IsGmSelected())
                Process.Start("https://" + toolSshDp.Tag.ToString() + "/jeegy/");
        }

        private void menuManBD_Click(object sender, EventArgs e)
        {
            if (IsGmSelected())
                Process.Start("http://" + toolSshIbgm.Tag.ToString() + ":8091/");
        }

        private void menuStockControl_Click(object sender, EventArgs e)
        {
            if (IsGmSelected())
                Process.Start("http://" + toolSshIbgm.Tag.ToString());
        }

        private void menuMorganizer_Click(object sender, EventArgs e)
        {
            ProcStart(Application.StartupPath + @"\modules\morganizer\morganizer.exe", null);
        }

        private void menuThemeDark_Click(object sender, EventArgs e)
        {
            SetColor(ActiveForm, Color.Gray, Color.Black);
            //foreach (Control ctrl in mainMenu.Controls["menuFile"].Controls)
            //{
            //    if (ctrl != null)
            //    {
            //        ctrl.BackColor = Color.Red;
            //        ctrl.ForeColor = Color.White;
            //    }
            //}
        }

        private void menuThemeStandart_Click(object sender, EventArgs e)
        {
            SetColor(ActiveForm, SystemColors.Control, SystemColors.ControlText);
        }

        private void menuPingSrvs_Click(object sender, EventArgs e)
        {
            SrvShowHide("show");
        }

        private void menuVersion_Click(object sender, EventArgs e)
        {
            XmlDocument xDoc = new XmlDocument();
            string version = "";
            try
            {
                xDoc.Load("version.xml");
            }
            catch
            {
                MessageBox.Show("Не удалось открыть файл с версией", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Name == "version")
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("ver");
                    version = attr.Value;
                }
            }
            MessageBox.Show("SDTander v" + version, "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuUpHistory_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("notepad", @"uphistory");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка открытия файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void menuCheckUpdate_Click(object sender, EventArgs e)
        {
            CheckUpdate();
            if (!update) MessageBox.Show("Установлена актуальная версия ПО", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txbFindUseful_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                findUseful(rtbUseful.SelectionStart);
            }
        }

        private void btnFindUseful_Click(object sender, EventArgs e)
        {
            findUseful(rtbUseful.SelectionStart);
        }

        private void btnFindNextUseful_Click(object sender, EventArgs e)
        {
            findUseful(rtbUseful.SelectionStart + txbFindUseful.Text.Length);
        }

        private void btnEditUseful_Click(object sender, EventArgs e)
        {
            rtbUseful.ReadOnly = false;
            rtbUseful.BackColor = Color.LightGray;
        }

        private void btnSaveUseful_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("useful.txt", false, new UTF8Encoding(false));
            sw.Write(rtbUseful.Text);
            sw.Close();
            rtbUseful.ReadOnly = true;
            rtbUseful.BackColor = Color.White;
        }

        private void rtbUseful_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                findUseful(rtbUseful.SelectionStart + txbFindUseful.Text.Length);
            }
        }

        private void rtbUseful_MouseClick(object sender, MouseEventArgs e)
        {
            cmsCopy.Tag = "rtbUseful";
        }

        private void cmsCopy_Click(object sender, EventArgs e)
        {
            string copyText = string.Empty;
            //string a = Convert.ToString(cmsCopy.Tag);

            //if ((Convert.ToString(cmsCopy.Tag)) == "rtbAnswers")
            //{
            //    if (rtbAnswers.SelectionLength == 0)
            //        copyText = rtbAnswers.Text.Replace("\n", Environment.NewLine);

            //    else
            //        copyText = rtbAnswers.SelectedText.Replace("\n", Environment.NewLine);
            //}

            //if ((Convert.ToString(cmsCopy.Tag)) == "rtbDecision")
            //{
            //    if (rtbDecision.SelectionLength == 0)
            //        copyText = rtbDecision.Text.Replace("\n", Environment.NewLine);

            //    else
            //        copyText = rtbDecision.SelectedText.Replace("\n", Environment.NewLine);
            //}

            if ((Convert.ToString(cmsCopy.Tag)) == "rtbUseful")
            {
                if (rtbUseful.SelectionLength == 0)
                    copyText = rtbUseful.Text.Replace("\n", Environment.NewLine);

                else
                    copyText = rtbUseful.SelectedText.Replace("\n", Environment.NewLine);
            }


            try
            {
                Clipboard.SetDataObject(copyText, true, 3, 400);
            }

            catch (System.Runtime.InteropServices.ExternalException)
            {
                MessageBox.Show(this, "Не удалось очистить буфер обмена. Возможно буфер обмена используется другим процессом.",
                    "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRmsKso_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = pathRms;
            string ip = cbIpScalesStart.Text + txbIpScalesEnd.Text;
            string arg = "/c start /d " + '\u0022' + pathRms + '\u0022' + " /create /host:" + ip + " /FULLCONTROL";
            psi.Arguments = arg;
            Process.Start(psi);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (Constants.UserLogin != "superuser") GetMode();
            CheckUpdate();
        }

        private void btnSrvHide_Click(object sender, EventArgs e)
        {
            SrvShowHide("hide");
        }

        private void btnPingSrvs_Click(object sender, EventArgs e)
        {
            if (IsGmSelected())
            {
                progressBar.Value = 0;
                progressBar.Maximum = SrvArr.Length;
                //проверяем FW
                IPStatus status = new Ping().Send(lblPing253.Tag.ToString(), 3000).Status;

                if (status != IPStatus.Success)
                {
                    DialogResult result = MessageBox.Show("Firewall не доступен.\nРекомендуется проверить доступность\nостальных серверов в ручную.\nПродолжить автоматическую проверку?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        for (int i = 0; i < SrvArr.Length; i++)
                        {
                            Ping(SrvArr[i].ToString(), txbIpScalesStart.Text + SrvArr[i].ToString());
                            progressBar.PerformStep();
                        }
                    }
                    else
                        return;
                }
                else
                {
                    for (int i = 0; i < SrvArr.Length; i++)
                    {
                        Ping(SrvArr[i].ToString(), txbIpScalesStart.Text + SrvArr[i].ToString());
                        progressBar.PerformStep();
                    }
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void CbIpGM_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //GetLineAndFillBoxes("ip");
        }

        private void CbNameGM_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    GetLineAndFillBoxes("name");
            //}
        }

        private void CbNameGM_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //GetLineAndFillBoxes("name");
            DataTable dtHm = dsHm.Tables[0];
            foreach (DataRow row in dtHm.Rows)
            {
                var items = row.ItemArray;
                if (items[2].ToString() == cbNameGM.SelectedValue.ToString())
                {
                    int start = items[6].ToString().Length - 8;
                    int count = 8;
                    txbDateOpen.Text = items[6].ToString().Remove(start, count);
                    txbFilial.Text = items[5].ToString();
                    txbTimeGM.Text = items[9].ToString();
                    lblStatusGM.Text = items[3].ToString();
                    FillIp(items[8].ToString());

                    cbIpScalesStart.Items.Clear();
                    string[] arrip = items[8].ToString().Split('.');
                    string ipPlus = (Convert.ToInt32(arrip[2]) + 1).ToString();
                    cbIpScalesStart.Items.Add(arrip[0] + '.' + arrip[1] + '.' + arrip[2] + '.');
                    cbIpScalesStart.Items.Add(arrip[0] + '.' + arrip[1] + '.' + ipPlus + '.');
                    cbIpScalesStart.SelectedIndex = 0;

                    subnetItems = cbIpScalesStart.Items.Cast<string>().ToArray();

                    progressBar.Value = 0;
                    cbNameGM.SelectionLength = 0;
                    cbNameGM.SelectionStart = cbNameGM.Text.Length;

                    return;
                }
            }

            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FillDatabase();
        }

        

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
