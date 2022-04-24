using JRO;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;


namespace SD
{
    public partial class MainForm : Form
    {
        public static MainForm SelfRef { get; set; }
        public MainForm()
        {
            SelfRef = this;
            InitializeComponent();
            SetRights();
            ReadConfig();
            LoadAnswers();
            LoadDecision();
            LoadUseful();
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("ru-RU"));
            GetAutoUpdate();
            LoadComboBox();
            SetTopMostForm();            
        }

        //Переменные
        string line;
        int prbarCount = 0;
        bool autoupdate;
        string pathUpdate, pathPutty, pathRms, pathUvnc, upHost, pathWinscp, subnetIp, pathSqlFile, sshuser;
        bool update = false;
        bool topMostForm;
        int[] SrvArr = new int[12] { 1, 2, 3, 5, 6, 7, 18, 250, 253, 254, 93, 141 };
        int[] IpmiArr = new int[7] { 217, 218, 12, 13, 14, 15, 16 };

        string[] subnetItems;
        Color[] colors = { Color.White, Color.Black };

        DataSet dsHm = new DataSet();
        DataSet dsMm = new DataSet();


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
            StreamReader str = new StreamReader("config", Encoding.UTF8);

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
                if (cline.StartsWith("sqlfile"))
                {
                    string[] i = cline.Split('|');
                    Constants.SqlFile = i[1];
                }
                if (cline.StartsWith("sshuser"))
                {
                    string[] i = cline.Split('|');
                    sshuser = i[1];
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
                MessageBox.Show(ex.Message, "Не доступен ресурс с со списком ГМ");
                return;
            }
        }

        public void GetAutoUpdate()
        {
            if (autoupdate)
            {
                if (File.Exists("DB_back.mdb")) File.Delete("DB_back.mdb");
                File.Copy("DB.mdb", "DB_back.mdb");
                if (File.Exists("all_reports_back.xls")) File.Delete("all_reports_back.xls");
                File.Copy("all_reports.xls", "all_reports_back.xls");

                CreatDbGM();
                if (GetFileForDb())
                {
                    FillDatabase();

                    cbCodeGM.DataSource = null;
                    cbNameGM.DataSource = null;
                    cbIpGM.DataSource = null;
                    cbNameMM.DataSource = null;
                    cbCodeMM.DataSource = null;

                    cbNameGM.Items.Clear();
                    cbCodeGM.Items.Clear();
                    cbIpGM.Items.Clear();
                    cbNameMM.Items.Clear();
                    cbCodeMM.Items.Clear();

                    LoadComboBox();
                    MessageBox.Show("БД обновлена", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                MessageBox.Show("Не удалось обновить БД", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }                
        }

        public void LoadComboBox()
        {            
            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DB.mdb;Jet OLEDB:Database Password=parolDlya_BD;");
            connection.Open();
            //Заполняем combobox'ы на вкладке "ГМ"
            OleDbDataAdapter ole_adapterHM = new OleDbDataAdapter("SELECT * FROM HM", connection);
            ole_adapterHM.Fill(dsHm);


            cbCodeGM.DataSource = dsHm.Tables[0];
            cbCodeGM.DisplayMember = "code";
            cbCodeGM.ValueMember = "code";

            cbNameGM.DataSource = dsHm.Tables[0];
            cbNameGM.DisplayMember = "hm_name";
            cbNameGM.ValueMember = "hm_name";

            cbIpGM.DataSource = dsHm.Tables[0];
            cbIpGM.DisplayMember = "ip_hm";
            cbIpGM.ValueMember = "ip_hm";


            //Заполняем combobox'ы на вкладке ММ/МК
            OleDbDataAdapter ole_adapterMM = new OleDbDataAdapter("SELECT * FROM MD_MK", connection);
            ole_adapterMM.Fill(dsMm);

            cbNameMM.DataSource = dsMm.Tables[0];
            cbNameMM.DisplayMember = "mm_name";
            cbNameMM.ValueMember = "mm_name";

            cbCodeMM.DataSource = dsMm.Tables[0];
            cbCodeMM.DisplayMember = "code";
            cbCodeMM.ValueMember = "code";

            connection.Close();
        }

        public void FillAllFieldsHM(string param)
        {
            DataTable dtHm = dsHm.Tables[0];
            foreach (DataRow row in dtHm.Rows)
            {
                var items = row.ItemArray;
                if (param == "name")
                {
                    if (items[2].ToString() == cbNameGM.SelectedValue.ToString())
                    {
                        if (items[6].ToString() != "") txbDateOpen.Text = items[6].ToString().Remove(items[6].ToString().Length - 8, 8);
                        txbFilial.Text = items[5].ToString();
                        txbTimeGM.Text = items[9].ToString();
                        lblStatusGM.Text = items[3].ToString();
                        FillIp(items[8].ToString());

                        cbIpScalesStart.Items.Clear();
                        if (items[8].ToString() != "")
                        {
                            string[] arrip = items[8].ToString().Split('.');
                            string ipPlus = (Convert.ToInt32(arrip[2]) + 1).ToString();
                            cbIpScalesStart.Items.Add(arrip[0] + '.' + arrip[1] + '.' + arrip[2] + '.');
                            cbIpScalesStart.Items.Add(arrip[0] + '.' + arrip[1] + '.' + ipPlus + '.');
                            cbIpScalesStart.SelectedIndex = 0;

                            subnetItems = cbIpScalesStart.Items.Cast<string>().ToArray();
                            subnetIp = arrip[0] + '.' + arrip[1] + '.' + arrip[2] + '.';
                        }
                        

                        for (int i = 0; i < SrvArr.Length; i++)
                        {
                            Label lbl = tpHM.Controls["tableLayoutPanel4"].Controls["gbServersAvailability"].Controls["lblPing" + SrvArr[i].ToString()] as Label;
                            lbl.BackColor = Color.WhiteSmoke;
                        }

                        for (int i = 0; i < IpmiArr.Length; i++)
                        {
                            Label lbl = tpHM.Controls["tableLayoutPanel4"].Controls["gbServersAvailability"].Controls["lblPing" + IpmiArr[i].ToString()] as Label;
                            lbl.BackColor = Color.WhiteSmoke;
                        }

                        btnScalesPing.BackColor = Color.WhiteSmoke;
                        btnScalesPing.ForeColor = Color.Black;
                        btnRmsSshPing.BackColor = Color.WhiteSmoke;
                        btnRmsSshPing.ForeColor = Color.Black;

                        txbIpScalesEnd.ForeColor = Color.Black;
                        txbIpScalesEnd.BackColor = Color.White;

                        progressBar.Value = 0;
                        cbNameGM.SelectionLength = 0;
                        cbNameGM.SelectionStart = cbNameGM.Text.Length;

                        return;
                    }
                }
                if (param == "code")
                {
                    if (items[1].ToString() == cbCodeGM.SelectedValue.ToString())
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
                        subnetIp = arrip[0] + '.' + arrip[1] + '.' + arrip[2] + '.';

                        for (int i = 0; i < SrvArr.Length; i++)
                        {
                            Label lbl = tpHM.Controls["tableLayoutPanel4"].Controls["gbServersAvailability"].Controls["lblPing" + SrvArr[i].ToString()] as Label;
                            lbl.BackColor = Color.WhiteSmoke;
                        }

                        for (int i = 0; i < IpmiArr.Length; i++)
                        {
                            Label lbl = tpHM.Controls["tableLayoutPanel4"].Controls["gbServersAvailability"].Controls["lblPing" + IpmiArr[i].ToString()] as Label;
                            lbl.BackColor = Color.WhiteSmoke;
                        }

                        btnScalesPing.BackColor = Color.WhiteSmoke;
                        btnScalesPing.ForeColor = Color.Black;
                        btnRmsSshPing.BackColor = Color.WhiteSmoke;
                        btnRmsSshPing.ForeColor = Color.Black;

                        txbIpScalesEnd.ForeColor = Color.Black;
                        txbIpScalesEnd.BackColor = Color.White;

                        progressBar.Value = 0;
                        cbNameGM.SelectionLength = 0;
                        cbNameGM.SelectionStart = cbNameGM.Text.Length;

                        return;
                    }
                }
                if (param == "ip")
                {
                    if (items[8].ToString() == cbIpGM.SelectedValue.ToString())
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
                        subnetIp = arrip[0] + '.' + arrip[1] + '.' + arrip[2] + '.';

                        for (int i = 0; i < SrvArr.Length; i++)
                        {
                            Label lbl = tpHM.Controls["tableLayoutPanel4"].Controls["gbServersAvailability"].Controls["lblPing" + SrvArr[i].ToString()] as Label;
                            lbl.BackColor = Color.WhiteSmoke;
                        }

                        for (int i = 0; i < IpmiArr.Length; i++)
                        {
                            Label lbl = tpHM.Controls["tableLayoutPanel4"].Controls["gbServersAvailability"].Controls["lblPing" + IpmiArr[i].ToString()] as Label;
                            lbl.BackColor = Color.WhiteSmoke;
                        }

                        btnScalesPing.BackColor = Color.WhiteSmoke;
                        btnScalesPing.ForeColor = Color.Black;
                        btnRmsSshPing.BackColor = Color.WhiteSmoke;
                        btnRmsSshPing.ForeColor = Color.Black;

                        txbIpScalesEnd.ForeColor = Color.Black;
                        txbIpScalesEnd.BackColor = Color.White;

                        progressBar.Value = 0;
                        cbNameGM.SelectionLength = 0;
                        cbNameGM.SelectionStart = cbNameGM.Text.Length;

                        return;
                    }
                }

            }            
        }

        public void FillAllFieldsMM(string param)
        {
            txbRmsPass1.Text = "";
            txbRmsPass2.Text = "";
            txbRmsPass3.Text = "";
            txbDbPass.Text = "";
            txbIbmdPass.Text = "";
            txbMailPass.Text = "";

            lblMainChanelMM.BackColor = Color.WhiteSmoke;
            lblReserveChanelMM.BackColor = Color.WhiteSmoke;
            btnPingMM.BackColor = Color.WhiteSmoke;
            lblMainChanelMM.Text = "";
            lblReserveChanelMM.Text = "";
            lblMainChanelMM.Tag = "";
            lblReserveChanelMM.Tag = "";

            btnPingMM.Tag = "";
            tsbtnRmsMM.Tag = "";
            tsbtnSshMM.Tag = "";
            tsbtnWinscpMM.Tag = "";

            cbNameMM.SelectionLength = 0;
            cbNameMM.SelectionStart = cbNameMM.Text.Length;


            DataTable dtMm = dsMm.Tables[0];
            foreach (DataRow row in dtMm.Rows)
            {
                var items = row.ItemArray;
                if (param == "name")
                {
                    if (items[2].ToString() == cbNameMM.SelectedValue.ToString())
                    {
                        if (items[6].ToString() != "") txbDateOpenMM.Text = items[6].ToString().Remove(items[6].ToString().Length - 8, 8);
                        else txbDateOpenMM.Text = "";
                        if (items[7].ToString() != "") txbDateCloseMM.Text = items[7].ToString().Remove(items[7].ToString().Length - 8, 8);
                        else txbDateCloseMM.Text = "";
                        txbFilialMM.Text = items[5].ToString();
                        //txbTimeMM.Text = items[8].ToString();
                        tslblStatusMM.Text = items[3].ToString();
                        tslblTypeTO.Text = items[4].ToString();
                        string codeMM = items[1].ToString();
                        if (codeMM.Length < 6)
                        {
                            for(int i = 0; i < 6 - codeMM.Length; i++)
                            {
                                codeMM = "0" + codeMM;
                            }
                        }
                        if (items[4].ToString() == "МД")
                        {
                            btnPingMM.Tag = "OMD_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            lblMainChanelMM.Tag = "OMD_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            lblReserveChanelMM.Tag = "OMD_" + codeMM + "_1.ONLINEMM.CORP.TANDER.RU";
                            tsbtnRmsMM.Tag = "OMD_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            tsbtnSshMM.Tag = "OMD_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            tsbtnWinscpMM.Tag = "OMD_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                        }
                        else if (items[4].ToString() == "МК")
                        {
                            btnPingMM.Tag = "OMK_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            lblMainChanelMM.Tag = "OMK_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            lblReserveChanelMM.Tag = "OMK_" + codeMM + "_1.ONLINEMM.CORP.TANDER.RU";
                            tsbtnRmsMM.Tag = "OMK_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            tsbtnSshMM.Tag = "OMK_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            tsbtnWinscpMM.Tag = "OMK_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                        }
                        

                        //Получаем пароли
                        OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DB.mdb;Jet OLEDB:Database Password=parolDlya_BD;");
                        string query = "SELECT * FROM PASS_MD_MK WHERE filial = @filial";
                        OleDbCommand command = new OleDbCommand(query, connection);
                        command.Parameters.AddWithValue("@filial", items[5].ToString());
                        connection.Open();
                        OleDbDataReader SelectReader = command.ExecuteReader();

                        while (SelectReader.Read())
                        {
                            //login = (dr["Название"]);
                            txbRmsPass1.Text = (SelectReader["rms_1"]).ToString();
                            txbRmsPass2.Text = (SelectReader["rms_2"]).ToString();
                            txbRmsPass3.Text = (SelectReader["rms_3"]).ToString();
                            txbDbPass.Text = (SelectReader["dbase"]).ToString();
                            txbIbmdPass.Text = (SelectReader["server"]).ToString();
                            txbMailPass.Text = (SelectReader["mail"]).ToString();
                        }

                        SelectReader.Close();
                        connection.Close();

                        return;
                    }
                }
                if (param == "code")
                {
                    if (items[1].ToString() == cbCodeMM.SelectedValue.ToString())
                    {
                        if (items[6].ToString() != "") txbDateOpenMM.Text = items[6].ToString().Remove(items[6].ToString().Length - 8, 8);
                        else txbDateOpenMM.Text = "";
                        if (items[7].ToString() != "") txbDateCloseMM.Text = items[7].ToString().Remove(items[7].ToString().Length - 8, 8);
                        else txbDateCloseMM.Text = "";
                        txbFilialMM.Text = items[5].ToString();
                        //txbTimeMM.Text = items[8].ToString();
                        tslblStatusMM.Text = items[3].ToString();
                        tslblTypeTO.Text = items[4].ToString();
                        string codeMM = items[1].ToString();
                        if (codeMM.Length < 6)
                        {
                            for (int i = 0; i < 6 - codeMM.Length; i++)
                            {
                                codeMM = "0" + codeMM;
                            }
                        }

                        if (items[4].ToString() == "МД")
                        {
                            btnPingMM.Tag = "OMD_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            lblMainChanelMM.Tag = "OMD_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            lblReserveChanelMM.Tag = "OMD_" + codeMM + "_1.ONLINEMM.CORP.TANDER.RU";
                            tsbtnRmsMM.Tag = "OMD_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            tsbtnSshMM.Tag = "OMD_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            tsbtnWinscpMM.Tag = "OMD_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                        }
                        else if (items[4].ToString() == "МК")
                        {
                            btnPingMM.Tag = "OMK_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            lblMainChanelMM.Tag = "OMK_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            lblReserveChanelMM.Tag = "OMK_" + codeMM + "_1.ONLINEMM.CORP.TANDER.RU";
                            tsbtnRmsMM.Tag = "OMK_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            tsbtnSshMM.Tag = "OMK_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                            tsbtnWinscpMM.Tag = "OMK_" + codeMM + ".ONLINEMM.CORP.TANDER.RU";
                        }

                        //Получаем пароли
                        OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DB.mdb;Jet OLEDB:Database Password=parolDlya_BD;");
                        string query = "SELECT * FROM PASS_MD_MK WHERE filial = @filial";
                        OleDbCommand command = new OleDbCommand(query, connection);
                        command.Parameters.AddWithValue("@filial", items[5].ToString());
                        connection.Open();
                        OleDbDataReader SelectReader = command.ExecuteReader();

                        while (SelectReader.Read())
                        {
                            //login = (dr["Название"]);
                            txbRmsPass1.Text = (SelectReader["rms_1"]).ToString();
                            txbRmsPass2.Text = (SelectReader["rms_2"]).ToString();
                            txbRmsPass3.Text = (SelectReader["rms_3"]).ToString();
                            txbDbPass.Text = (SelectReader["dbase"]).ToString();
                            txbIbmdPass.Text = (SelectReader["server"]).ToString();
                            txbMailPass.Text = (SelectReader["mail"]).ToString();
                        }

                        SelectReader.Close();
                        connection.Close();

                        return;
                    }
                }
            }
        }

        private void FillIp(string ipHm)
        {
            if (ipHm != "")
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
                toolSshWifi.Tag = ip + "7";
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

                lblPing217.Tag = ip + "217";
                lblPing218.Tag = ip + "218";
                lblPing12.Tag = ip + "12";
                lblPing15.Tag = ip + "15";
                lblPing16.Tag = ip + "16";
                lblPing13.Tag = ip + "13";
                lblPing14.Tag = ip + "14";

                menuRMSTerminal.Tag = ip + "3";
                menuRMSWinBackup.Tag = ip + "6";
                menuSshIbgm.Tag = ip + "1";
                menuSshIbgmBackup.Tag = ip + "2";
                menuSshZope.Tag = ip + "5";
            }
            else
            {
                txbRobotPass.Text = "";
                txbRobotName.Text = "";

                toolSshIbgm.Tag = "";
                toolWinscpIbgm.Tag = "";
                toolSshIbgmBack.Tag = "";
                toolWinscpIbgmBack.Tag = "";
                toolRmsTerm.Tag = "";
                toolRdpTerm.Tag = "";
                toolSshZope.Tag = "";
                toolWinscpZope.Tag = "";
                toolRmsWinBack.Tag = "";
                toolRdpWinBack.Tag = "";
                toolSshWifi.Tag = "";
                toolSshDp.Tag = "";
                toolRmsUtm.Tag = "";
                toolRmsScala.Tag = "";

                toolIpmiIbgm.Tag = "";
                toolIpmiIbgmBack.Tag = "";
                toolIpmiZope.Tag = "";
                toolIpmiTerm.Tag = "";
                toolIpmiWinBack.Tag = "";

                lblPing1.Tag = "";
                lblPing2.Tag = "";
                lblPing3.Tag = "";
                lblPing5.Tag = "";
                lblPing6.Tag = "";
                lblPing7.Tag = "";
                lblPing18.Tag = "";
                lblPing93.Tag = "";
                lblPing250.Tag = "";
                lblPing253.Tag = "";
                lblPing141.Tag = "";
                lblPing254.Tag = "";

                lblPing217.Tag = "";
                lblPing218.Tag = "";
                lblPing12.Tag = "";
                lblPing15.Tag = "";
                lblPing16.Tag = "";
                lblPing13.Tag = "";
                lblPing14.Tag = "";

                cbIpScalesStart.Text = "";

                menuRMSTerminal.Tag = "";
                menuRMSWinBackup.Tag = "";
                menuSshIbgm.Tag = "";
                menuSshIbgmBackup.Tag = "";
                menuSshZope.Tag = "";
            }
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

        public bool IsMmSelected()
        {
            if (cbNameMM.Text == "")
            {
                MessageBox.Show("ВротМнеТапки! Не позорь мою лысую голову!\nМагаз кто за тебя выберет?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                //MessageBox.Show(ex.ToString(), "Ошибка выполнения Ping", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return (status);
        }

        public void PingSrv(string server, string host)
        {
            Label lbl = tpHM.Controls["tableLayoutPanel4"].Controls["gbServersAvailability"].Controls["lblPing" + server] as Label;
            IPStatus status = IPStatus.Unknown;
            try
            {
                status = new Ping().Send(host, 3000).Status;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка выполнения Ping", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (status == IPStatus.Success) lbl.BackColor = Color.Green;
            else lbl.BackColor = Color.Red;
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
                if (xnode.Name == "SshPass")
                {
                    Constants.SshPass = xnode.InnerText;
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
            //TODO: Переделать под скачивание с GIT
            try
            {
                WebClient client = new WebClient() { Proxy = null };
                if (File.Exists(@"\\" + upHost + @"\Update$\Updater.exe"))
                    client.DownloadFile(@"\\" + upHost + @"\Update$\Updater.exe", "Updater.exe");

                if (File.Exists(@"\\" + upHost + @"\Update$\version.xml"))
                {
                    client.DownloadFile(@"\\" + upHost + @"\Update$\version.xml", "version_new.xml");
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

        public void LoadUseful()
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
            JetEngine je = new JetEngine();
            //je.CompactDatabase(connectionString, mdwfilename);
            je.CompactDatabase("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + original + ";Jet OLEDB:Database Password=parolDlya_BD;Jet OLEDB:Engine Type=5",
                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + copy + ";Jet OLEDB:Database Password=parolDlya_BD;Jet OLEDB:Engine Type=5");
            return;
        }

        public void FillDatabase()
        {
            //-------------запуск таймера-------------------//
            //Stopwatch sw_total = new Stopwatch();
            //sw_total.Start();

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
                if (i == 2)
                {
                    string query = "INSERT INTO MD_MK (code, mm_name, status, type, filial, date_open, date_close) " +
                                    "VALUES ('-', '-', '-', '-', '-', null, null)";
                    OleDbCommand command = new OleDbCommand(query, connection);

                    query = "INSERT INTO HM (code, hm_name, status, type, filial, date_open, date_close) " +
                                    "VALUES ('-', '-', '-', '-', '-', null, null)";
                    command = new OleDbCommand(query, connection);
                }
                if (arrDataList[i, 4].ToString() == "МД" || arrDataList[i, 4].ToString() == "МК")
                {
                    string query = "INSERT INTO MD_MK (code, mm_name, status, type, filial, date_open, date_close) " +
                                    "VALUES (@code, @hm_name, @status, @type, @filial, @date_open, @date_close)";
                    OleDbCommand command = new OleDbCommand(query, connection);

                    if (arrDataList[i, 1] != null) 
                    { 
                        if (arrDataList[i, 1].ToString().Length < 6)
                        {
                            string codeMM = arrDataList[i, 1].ToString();
                            for (int k = 0; k < 6 - arrDataList[i, 1].ToString().Length; k++)
                            {
                                codeMM = "0" + codeMM;
                            }
                            command.Parameters.AddWithValue("@code", codeMM);
                        }else command.Parameters.AddWithValue("@code", arrDataList[i, 1].ToString());

                    }
                    else command.Parameters.AddWithValue("@code", DBNull.Value);
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


            //заполняем ip и часовой пояс ГМ/MM
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

            //sw_total.Stop();
            //MessageBox.Show(sw_total.ElapsedMilliseconds + " ms", "Time");
        }

        public void SrvShowHide(string mode)
        {
            if (mode == "show") this.Width = 875;
            else if (mode == "hide") this.Width = 586;
        }

        public void LoadAnswers()
        {
            OleDbConnection MyConnection;
            DataSet DtSet;
            OleDbDataAdapter MyCommand;
            MyConnection = new OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0; Data Source=answers.xls; Extended Properties=\"Excel 8.0;IMEX=1\"");
            MyCommand = new OleDbDataAdapter("select * from [Ответы$]", MyConnection);
            DtSet = new DataSet();
            MyCommand.Fill(DtSet);
            //dgvAnswers.DataSource = DtSet.Tables[0];
            DataTable dt = DtSet.Tables[0];
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                var items = row.ItemArray;
                if (i == 1)
                {
                    dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = items[0].ToString(), Width = 120 });
                    dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = items[1].ToString(), Width = 300 });
                    dgvAnswers.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = items[2].ToString(), Width = 80 });
                }
                if (i > 1)
                {
                    dgvAnswers.Rows.Add(items);
                }
                i++;
            }
            MyConnection.Close();
        }

        public void LoadDecision()
        {
            OleDbConnection MyConnection;
            DataSet DtSet;
            OleDbDataAdapter MyCommand;
            MyConnection = new OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0; Data Source=answers.xls; Extended Properties=\"Excel 8.0;IMEX=1\"");
            MyCommand = new OleDbDataAdapter("select * from [Решения$]", MyConnection);
            DtSet = new DataSet();
            MyCommand.Fill(DtSet);
            //dgvDecision.DataSource = DtSet.Tables[0];

            DataTable dt = DtSet.Tables[0];
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                var items = row.ItemArray;
                if (i == 1)
                {
                    dgvDecision.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = items[0].ToString(), Width = 200 });
                    dgvDecision.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = items[1].ToString(), Width = 300 });
                }
                if (i > 1)
                {
                    dgvDecision.Rows.Add(items);
                }
                i++;
            }

            MyConnection.Close();
        }

        public string ChangePass(string password)
        {
            string newpassword = "";
            char[] pass = password.ToCharArray();
            char[] listrus = new char[78];
            char[] listeng = new char[78];
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
            return (newpassword);
        }

        public void saveDgvToExcel(string dgv)
        {
            Excel.Application xlApp = new Excel.Application(); //Excel
            Excel.Workbook xlWB; //рабочая книга              
            Excel.Worksheet xlSht; //лист Excel   
            xlWB = xlApp.Workbooks.Open(Application.StartupPath + @"\answers.xls"); //название файла Excel  

            if (dgv == "answers")
            {
                dgvAnswers.ReadOnly = true;
                //Очищаем лист перед сохранением
                Excel.Range rng;
                xlSht = xlWB.Worksheets["Ответы"];//название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];
                int iLastRow = xlSht.Cells[xlSht.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;//последняя заполненная строка в столбце А
                int iLastCol = xlSht.Cells[3, xlSht.Columns.Count].End[Excel.XlDirection.xlToLeft].Column; //последний заполненный столбец в 1-й строке
                                                                                                           //rng = xlSht.UsedRange;
                rng = (Excel.Range)xlSht.Range["A4", xlSht.Cells[iLastRow, iLastCol]]; //пример записи диапазона ячеек в переменную Rng
                rng.Clear();

                //Пишем из dgv в excel
                for (int i = 0; i < dgvAnswers.RowCount; i++)
                {
                    for (int j = 0; j < dgvAnswers.ColumnCount; j++)
                    {
                        if (dgvAnswers.Rows[i].Cells[j].Value != null)
                            xlSht.Rows[i + 4].Columns[j + 1] = dgvAnswers.Rows[i].Cells[j].Value.ToString();
                    }
                }
            }

            if (dgv == "decision")
            {
                dgvDecision.ReadOnly = true;
                //Очищаем лист перед сохранением
                Excel.Range rng;
                xlSht = xlWB.Worksheets["Решения"];//название листа или 1-й лист в книге xlSht = xlWB.Worksheets[1];
                int iLastRow = xlSht.Cells[xlSht.Rows.Count, "A"].End[Excel.XlDirection.xlUp].Row;//последняя заполненная строка в столбце А
                int iLastCol = xlSht.Cells[3, xlSht.Columns.Count].End[Excel.XlDirection.xlToLeft].Column; //последний заполненный столбец в 1-й строке
                rng = (Excel.Range)xlSht.Range["A4", xlSht.Cells[iLastRow, iLastCol]]; //пример записи диапазона ячеек в переменную Rng
                rng.Clear();

                //Пишем из dgv в excel
                for (int k = 0; k < dgvDecision.RowCount; k++)
                {
                    for (int l = 0; l < dgvDecision.ColumnCount; l++)
                    {
                        if (dgvDecision.Rows[k].Cells[l].Value != null)
                            xlSht.Rows[k + 4].Columns[l + 1] = dgvDecision.Rows[k].Cells[l].Value.ToString();
                    }
                }
            }            

            xlWB.Application.DisplayAlerts = false;
            xlWB.SaveAs(Application.StartupPath + @"\answers.xls");

            //xlApp.Visible = true; //отображаем Excel     
            //xlWB.Close(false); //закрываем книгу, изменения не сохраняем
            xlApp.Quit(); //закрываем Excel
            GC.Collect(); // убрать за собой -- в том числе не используемые явно объекты !
            MessageBox.Show("Выполнено", "!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public bool GetFileForDb()
        {
            bool result = false;
            if (File.Exists("all_reports.xls"))
            {
                if (File.Exists(Application.StartupPath + @"\all_reports_.xls")) File.Delete(Application.StartupPath + @"\all_reports_.xls");
                File.Move(Application.StartupPath + @"\all_reports.xls", Application.StartupPath + @"\all_reports_.xls");
            }
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile("http://skynet.corp.tander.ru/atlantis_reports/all_reports.xls", Application.StartupPath + @"\all_reports.xls");
                //webClient.DownloadFile("https://ru.files.fm/down.php?cf&i=6msv4bsw&n=all_reports.xls", Application.StartupPath + @"\all_reports.xls");
                if (File.Exists(Application.StartupPath + @"\all_reports_.xls")) File.Delete(Application.StartupPath + @"\all_reports_.xls");
                result = true;                
            }
            catch(Exception ex)
            {
                MessageBox.Show("Не доступен сетевой ресурс. Не удается обновить БД объектов.\n" +
                    "Попробуйте обновить БД позже из меню ПО,работа будет продолжена без обновления.\n" +
                    "Некоторые объекты могут не отображаться в ПО.\n\n\n" + ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!File.Exists(Application.StartupPath + @"\all_reports.xls"))
            {

                File.Move(Application.StartupPath + @"\all_reports_.xls", Application.StartupPath + @"\all_reports.xls");
                File.Delete(Application.StartupPath + @"\all_reports_.xls");
            }
            return result;
        }

        private void CbCodeGM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FillAllFieldsHM(((ComboBox)sender).Tag.ToString());
            }
        }

        private void CbCodeGM_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillAllFieldsHM(((ComboBox)sender).Tag.ToString());
        }

        private void CbIpGM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FillAllFieldsHM(((ComboBox)sender).Tag.ToString());
            }
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
            char[] listrus = new char[78];
            char[] listeng = new char[78];
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
            if (IsGmSelected() && (((ToolStripMenuItem)sender).Tag.ToString() != ""))
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
            if (IsGmSelected() && (((ToolStripMenuItem)sender).Tag.ToString() != ""))
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
            if (IsGmSelected() && (((ToolStripMenuItem)sender).Tag.ToString() != ""))
                SrvRdpConnect(((ToolStripMenuItem)sender).Tag.ToString());
        }

        private void toolIpmiIbgm_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsGmSelected() && (((ToolStripMenuItem)sender).Tag.ToString() != ""))
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
            btnScalesPing.ForeColor = Color.Black;

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

            tabControl.SelectedIndex = 1;
            tabControl.SelectedIndex = 0;

            btnTpHm.Font = new Font(btnTpHm.Font, FontStyle.Underline);
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
            if (File.Exists("DB_back.mdb")) File.Delete("DB_back.mdb");
            File.Copy("DB.mdb", "DB_back.mdb");
            if (File.Exists("all_reports_back.xls")) File.Delete("all_reports_back.xls");
            File.Copy("all_reports.xls", "all_reports_back.xls");

            CreatDbGM();
            if (GetFileForDb())
            {
                FillDatabase();

                cbCodeGM.DataSource = null;
                cbNameGM.DataSource = null;
                cbIpGM.DataSource = null;
                cbNameMM.DataSource = null;
                cbCodeMM.DataSource = null;

                cbNameGM.Items.Clear();
                cbCodeGM.Items.Clear();
                cbIpGM.Items.Clear();
                cbNameMM.Items.Clear();
                cbCodeMM.Items.Clear();

                LoadComboBox();
                MessageBox.Show("БД обновлена", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MessageBox.Show("Не удалось обновить БД", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                SrvRmsConnect(((ToolStripMenuItem)sender).Tag.ToString());
            }
        }

        private void menuSshIbgm_Click(object sender, EventArgs e)
        {
            if (IsGmSelected())
            {
                if (Mode.mode)
                    ProcStart(pathPutty, " -ssh -l " + Constants.UserLogin + " -pw " + Constants.UserPass + " " + ((ToolStripMenuItem)sender).Tag.ToString());
                else
                    ProcStart(pathPutty, " -ssh " + ((ToolStripMenuItem)sender).Tag.ToString());
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
            if (Constants.UserLogin != "superuser1") GetMode();
            CheckUpdate();
        }

        private void btnSrvHide_Click(object sender, EventArgs e)
        {
            SrvShowHide("hide");
            for (int i = 0; i < SrvArr.Length; i++)
            {
                Label lbl = tpHM.Controls["tableLayoutPanel4"].Controls["gbServersAvailability"].Controls["lblPing" + SrvArr[i].ToString()] as Label;
                lbl.BackColor = Color.WhiteSmoke;
            }

            for (int i = 0; i < IpmiArr.Length; i++)
            {
                Label lbl = tpHM.Controls["tableLayoutPanel4"].Controls["gbServersAvailability"].Controls["lblPing" + IpmiArr[i].ToString()] as Label;
                lbl.BackColor = Color.WhiteSmoke;
            }
        }

        private void btnPingSrvs_Click(object sender, EventArgs e)
        {
            if (IsGmSelected())
            {
                progressBar.Value = 0;
                progressBar.Maximum = SrvArr.Length;
                //проверяем FW
                try
                {
                    IPStatus status = new Ping().Send(lblPing253.Tag.ToString(), 3000).Status;

                    if (status != IPStatus.Success)
                    {
                        DialogResult result = MessageBox.Show("Firewall не доступен.\nРекомендуется проверить доступность\nостальных серверов в ручную.\nПродолжить автоматическую проверку?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (result == DialogResult.Yes)
                        {
                            for (int i = 0; i < SrvArr.Length; i++)
                            {
                                PingSrv(SrvArr[i].ToString(), subnetIp + SrvArr[i].ToString());
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
                            PingSrv(SrvArr[i].ToString(), subnetIp + SrvArr[i].ToString());
                            progressBar.PerformStep();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка выполнения Ping", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void lblPing1_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsGmSelected())
            {
                if (e.Button == MouseButtons.Left)
                {
                    IPStatus status = Ping(((Label)sender).Tag.ToString());
                    if (status == IPStatus.Success) ((Label)sender).BackColor = Color.Green;
                    else ((Label)sender).BackColor = Color.Red;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    PingT(((Label)sender).Tag.ToString());
                }
            }
        }

        private void btnPingMM_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsMmSelected())
            {
                if (e.Button == MouseButtons.Left)
                {
                    try
                    {
                        IPStatus status = Ping(btnPingMM.Tag.ToString());
                        if (status == IPStatus.Success)
                        {
                            btnPingMM.BackColor = Color.Green;
                        }
                        else
                        {
                            btnPingMM.BackColor = Color.Red;
                        }
                    }
                    catch
                    {
                        btnPingMM.BackColor = Color.Red;
                    }

                    try
                    {
                        IPStatus status = Ping(lblMainChanelMM.Tag.ToString());
                        lblMainChanelMM.Text = Dns.GetHostAddresses(lblMainChanelMM.Tag.ToString())[0].ToString();

                        if (status == IPStatus.Success)
                        {
                            lblMainChanelMM.BackColor = Color.Green;
                        }
                        else
                        {
                            lblMainChanelMM.BackColor = Color.Red;
                        }
                    }
                    catch
                    {
                        lblMainChanelMM.BackColor = Color.Red;
                    }

                    try
                    {
                        IPStatus status = Ping(lblReserveChanelMM.Tag.ToString());
                        lblReserveChanelMM.Text = Dns.GetHostAddresses(lblReserveChanelMM.Tag.ToString())[0].ToString();

                        if (status == IPStatus.Success)
                        {
                            lblReserveChanelMM.BackColor = Color.Green;
                        }
                        else
                        {
                            lblReserveChanelMM.BackColor = Color.Red;
                        }
                    }
                    catch
                    {
                        lblReserveChanelMM.BackColor = Color.Red;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    PingT(btnPingMM.Tag.ToString());
                }
            }
        }

        private void cbCodeMM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FillAllFieldsMM(((ComboBox)sender).Tag.ToString());
            }
        }

        private void cbCodeMM_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillAllFieldsMM(((ComboBox)sender).Tag.ToString());
        }

        private void tsbtnRmsMM_Click(object sender, EventArgs e)
        {
            if (IsMmSelected() && ((ToolStripButton)sender).Tag.ToString() != "")
            {
                if (chbxConnectToIPMM.Checked == false && chbxConnectToIPReservMM.Checked == false)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = pathRms;
                    string arg = " /d " + '\u0022' + pathRms + '\u0022' + " /name:" + '\u0022' + cbCodeMM.Text + "_" 
                        + cbNameMM.Text + "_" + tslblTypeTO.Text + " " + '\u0022' + " /create /host:" + ((ToolStripButton)sender).Tag.ToString() + " /FULLCONTROL";
                    psi.Arguments = arg;
                    Process.Start(psi);
                }
                if (chbxConnectToIPMM.Checked == true && chbxConnectToIPReservMM.Checked == false)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = pathRms;
                    string arg = " /d " + '\u0022' + pathRms + '\u0022' + " /name:" + '\u0022' + cbCodeMM.Text + "_"
                        + cbNameMM.Text + "_" + tslblTypeTO.Text + "_" + "IP_OSN" + " " + '\u0022' + " /create /host:" + lblMainChanelMM.Text + " /FULLCONTROL";
                    psi.Arguments = arg;
                    Process.Start(psi);
                }
                if (chbxConnectToIPMM.Checked == true && chbxConnectToIPReservMM.Checked == true)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = pathRms;
                    string arg = " /d " + '\u0022' + pathRms + '\u0022' + " /name:" + '\u0022' + cbCodeMM.Text + "_"
                        + cbNameMM.Text + "_" + tslblTypeTO.Text + "_" + "IP_RES" + " " + '\u0022' + " /create /host:" + lblReserveChanelMM.Text + " /FULLCONTROL";
                    psi.Arguments = arg;
                    Process.Start(psi);
                }
                if (chbxConnectToIPMM.Checked == false && chbxConnectToIPReservMM.Checked == true)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = pathRms;
                    string arg = " /d " + '\u0022' + pathRms + '\u0022' + " /name:" + '\u0022' + cbCodeMM.Text + "_"
                        + cbNameMM.Text + "_" + tslblTypeTO.Text + "_" + "RES" + " " + '\u0022' + " /create /host:" + lblReserveChanelMM.Tag.ToString() + " /FULLCONTROL";
                    psi.Arguments = arg;
                    Process.Start(psi);
                }

            }
        }

        private void tsbtnSshMM_Click(object sender, EventArgs e)
        {
            string ssh_username = "root", ssh_pass = txbIbmdPass.Text;

            //PuTTY.exe - P 2223 - l root - pw 123456789 OMD_340109.ONLINEMM.CORP.TANDER.RU
            if (IsMmSelected() && (((ToolStripButton)sender).Tag.ToString() != ""))
            {
                if (chbxUZSSHIBMD.Checked == true)
                {
                    ssh_username = sshuser;
                    if (Constants.SshPass != "")
                    {
                        ssh_pass = Constants.SshPass;
                    }                                                            
                }

                if (chbxConnectToIPMM.Checked == false && chbxConnectToIPReservMM.Checked == false)
                {
                    string args = " -ssh -P 2223 -l " + ssh_username + " -pw " + ssh_pass + " " + (((ToolStripButton)sender).Tag.ToString());
                    wright_log(pathPutty);
                    wright_log(args);
                    ProcStart(pathPutty, " -ssh -P 2223 -l " + ssh_username + " -pw " + ssh_pass + " " + (((ToolStripButton)sender).Tag.ToString()));
                }
                if (chbxConnectToIPMM.Checked == true && chbxConnectToIPReservMM.Checked == false)
                {
                    ProcStart(pathPutty, " -ssh -P 2223 -l " + ssh_username + " -pw " + ssh_pass + " " + lblMainChanelMM.Text);
                }
                if (chbxConnectToIPMM.Checked == true && chbxConnectToIPReservMM.Checked == true)
                {
                    ProcStart(pathPutty, " -ssh -P 2223 -l " + ssh_username + " -pw " + ssh_pass + " " + lblReserveChanelMM.Text);
                }
                if (chbxConnectToIPMM.Checked == false && chbxConnectToIPReservMM.Checked == true)
                {
                    ProcStart(pathPutty, " -ssh -P 2223 -l " + ssh_username + " -pw " + ssh_pass + " " + lblReserveChanelMM.Tag.ToString());
                }
            }
        }

        private void tsbtnWinscpMM_Click(object sender, EventArgs e)
        {
            string ssh_username = "root", ssh_pass = txbIbmdPass.Text;

            if (IsMmSelected() && ((ToolStripButton)sender).Tag.ToString() != "")
            {
                if (chbxUZSSHIBMD.Checked == true)
                {
                    ssh_username = sshuser;
                    ssh_pass = Constants.SshPass;
                }

                if (chbxConnectToIPMM.Checked == false && chbxConnectToIPReservMM.Checked == false)
                {
                    ProcStart(pathWinscp, ssh_username + ":" + ssh_pass + "@" + (((ToolStripButton)sender).Tag.ToString()) + ":2223");
                }
                if (chbxConnectToIPMM.Checked == true && chbxConnectToIPReservMM.Checked == false)
                {
                    ProcStart(pathWinscp, ssh_username + ":" + ssh_pass + "@" + lblMainChanelMM.Text + ":2223");
                }
                if (chbxConnectToIPMM.Checked == true && chbxConnectToIPReservMM.Checked == true)
                {
                    ProcStart(pathWinscp, ssh_username + ":" + ssh_pass + "@" + lblReserveChanelMM.Text + ":2223");
                }
                if (chbxConnectToIPMM.Checked == false && chbxConnectToIPReservMM.Checked == true)
                {
                    ProcStart(pathWinscp, ssh_username + ":" + ssh_pass + "@" + lblReserveChanelMM.Tag.ToString() + ":2223");
                }
                
            }
        }

        private void tpMM_Enter(object sender, EventArgs e)
        {
            this.Width = 586;
            menuPingSrvs.Enabled = false;
        }

        private void btnbtnChangePassLangMM_Click(object sender, EventArgs e)
        {
            txbRmsPass1.Text = ChangePass(txbRmsPass1.Text);
            txbRmsPass2.Text = ChangePass(txbRmsPass2.Text);
            txbRmsPass3.Text = ChangePass(txbRmsPass3.Text);

            txbDbPass.Text = ChangePass(txbDbPass.Text);
            txbIbmdPass.Text = ChangePass(txbIbmdPass.Text);
        }

        private void tsmEditAnswer_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag.ToString() == "answers")
            {
                dgvAnswers.ReadOnly = false;
                //dgvAnswers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dgvAnswers.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
            else if (((ToolStripMenuItem)sender).Tag.ToString() == "decision")
            {
                dgvDecision.ReadOnly = false;
                dgvDecision.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
        }

        private void tsmSaveAnswer_Click(object sender, EventArgs e)
        {
            saveDgvToExcel(((ToolStripMenuItem)sender).Tag.ToString());
            if (((ToolStripMenuItem)sender).Tag.ToString() == "answers")
            {
                dgvAnswers.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            }
            else if (((ToolStripMenuItem)sender).Tag.ToString() == "decision")
            {
                dgvDecision.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            }
        }

        private void btnHideFindAnswers_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Tag.ToString() == "answers") pFindAnswers.Visible = false;
            else if (((Button)sender).Tag.ToString() == "decision") pFindDecision.Visible = false;
        }

        private void tsmFindDecision_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag.ToString() == "answers")
            {
                pFindAnswers.Visible = true;
                txbFindAnswers.Focus();
                txbFindAnswers.Text = "";
            }
            else if (((ToolStripMenuItem)sender).Tag.ToString() == "decision")
            {
                pFindDecision.Visible = true;
                txbFindDecision.Focus();
                txbFindDecision.Text = "";
            }
        }

        private void txbFindAnswers_TextChanged(object sender, EventArgs e)
        {

            //table.DefaultView.RowFilter = string.Format("[имя столбца] LIKE '%{0}%'", txbFindAnswers.Text);
            if (((TextBox)sender).Tag.ToString() == "answers")
            {
                if (txbFindAnswers.TextLength == 0)
                {
                    dgvAnswers.FirstDisplayedScrollingRowIndex = 0;
                    dgvAnswers.ClearSelection();
                }
                else
                {
                    for (int i = 0; i < dgvAnswers.RowCount; i++)
                    {
                        dgvAnswers.Rows[i].Selected = false;
                        for (int j = 0; j < dgvAnswers.ColumnCount; j++)
                            if (dgvAnswers.Rows[i].Cells[j].Value != null)
                                if (dgvAnswers.Rows[i].Cells[j].Value.ToString().ToLower().Contains(txbFindAnswers.Text.ToLower()))
                                {
                                    dgvAnswers.Rows[i].Selected = true;
                                    dgvAnswers.FirstDisplayedScrollingRowIndex = i;
                                    //break;
                                }
                    }
                }                
            }
            else if (((TextBox)sender).Tag.ToString() == "decision")
            {
                if (txbFindDecision.TextLength == 0)
                {
                    dgvDecision.FirstDisplayedScrollingRowIndex = 0;
                    dgvDecision.ClearSelection();
                }
                else
                {
                    for (int i = 0; i < dgvDecision.RowCount; i++)
                    {
                        dgvDecision.Rows[i].Selected = false;
                        for (int j = 0; j < dgvDecision.ColumnCount; j++)
                            if (dgvDecision.Rows[i].Cells[j].Value != null)
                                if (dgvDecision.Rows[i].Cells[j].Value.ToString().ToLower().Contains(txbFindDecision.Text.ToLower()))
                                {
                                    dgvDecision.Rows[i].Selected = true;
                                    dgvDecision.FirstDisplayedScrollingRowIndex = i;
                                    //break;
                                }
                    }
                }                
            }            
        }

        private void tpAnswers_Enter(object sender, EventArgs e)
        {
            if (((TabPage)sender).Tag.ToString() == "answers") dgvAnswers.Focus();
            if (((TabPage)sender).Tag.ToString() == "decision") dgvDecision.Focus();

            this.Width = 586;
            menuPingSrvs.Enabled = false;
        }

        private void tpHM_Enter(object sender, EventArgs e)
        {
            menuPingSrvs.Enabled = true;
        }

        private void tpUseful_Enter(object sender, EventArgs e)
        {
            this.Width = 586;
            menuPingSrvs.Enabled = false;
        }

        private void btnEditSavePassMM_Click(object sender, EventArgs e)
        {
            if (IsMmSelected())
            {
                if (((Button)sender).Text == "Редактировать")
                {
                    txbRmsPass1.ReadOnly = false;
                    txbRmsPass2.ReadOnly = false;
                    txbRmsPass3.ReadOnly = false;
                    txbDbPass.ReadOnly = false;
                    txbIbmdPass.ReadOnly = false;
                    txbMailPass.ReadOnly = false;

                    ((Button)sender).Text = "Сохранить";
                }
                else if (((Button)sender).Text == "Сохранить")
                {
                    OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DB.mdb;Jet OLEDB:Database Password=parolDlya_BD;");

                    //Очистка таблиц MD_MK и HM
                    connection.Open();
                    string query = "UPDATE PASS_MD_MK set rms_1 = @rms_1, rms_2 = @rms_2, rms_3 = @rms_3, dbase = @dbase, server = @server, mail = @mail where filial = @filial";

                    OleDbCommand command = new OleDbCommand(query, connection);
                    if (txbRmsPass1.Text != "")
                    {
                        command.Parameters.AddWithValue("@rms_1", txbRmsPass1.Text);
                    }
                    else command.Parameters.AddWithValue("@rms_1", DBNull.Value);
                    if (txbRmsPass2.Text != "")
                    {
                        command.Parameters.AddWithValue("@rms_2", txbRmsPass2.Text);
                    }
                    else command.Parameters.AddWithValue("@rms_2", DBNull.Value);
                    if (txbRmsPass3.Text != "")
                    {
                        command.Parameters.AddWithValue("@rms_3", txbRmsPass3.Text);
                    }
                    else command.Parameters.AddWithValue("@rms_3", DBNull.Value);
                    if (txbDbPass.Text != "")
                    {
                        command.Parameters.AddWithValue("@dbase", txbDbPass.Text);
                    }
                    else command.Parameters.AddWithValue("@dbase", DBNull.Value);
                    if (txbIbmdPass.Text != "")
                    {
                        command.Parameters.AddWithValue("@server", txbIbmdPass.Text);
                    }
                    else command.Parameters.AddWithValue("@server", DBNull.Value);
                    if (txbMailPass.Text != "")
                    {
                        command.Parameters.AddWithValue("@mail", txbMailPass.Text);
                    }
                    else command.Parameters.AddWithValue("@mail", DBNull.Value);

                    command.Parameters.AddWithValue("@filial", txbFilialMM.Text);

                    command.ExecuteNonQuery();
                    connection.Close();

                    ((Button)sender).Text = "Редактировать";
                    txbRmsPass1.ReadOnly = true;
                    txbRmsPass2.ReadOnly = true;
                    txbRmsPass3.ReadOnly = true;
                    txbDbPass.ReadOnly = true;
                    txbIbmdPass.ReadOnly = true;
                    txbMailPass.ReadOnly = true;
                }
            }
            
            


        }

        private void btnTpHm_Click(object sender, EventArgs e)
        {
            TabPage tb = tabControl.Controls["tp" + ((Button)sender).Tag.ToString()] as TabPage;
            tabControl.SelectedTab = tb;
            if (((Button)sender).Text == "ГМ")
            {
                btnTpHm.Font = new Font(btnTpHm.Font, FontStyle.Underline);
                btnTpMMMK.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpAnswers.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpDecision.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpUseful.Font = new Font(btnTpHm.Font, FontStyle.Regular);
            }
            else if (((Button)sender).Text == "ММ/МК")
            {
                btnTpHm.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpMMMK.Font = new Font(btnTpHm.Font, FontStyle.Underline);
                btnTpAnswers.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpDecision.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpUseful.Font = new Font(btnTpHm.Font, FontStyle.Regular);
            }
            else if (((Button)sender).Text == "Ответы")
            {
                btnTpHm.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpMMMK.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpAnswers.Font = new Font(btnTpHm.Font, FontStyle.Underline);
                btnTpDecision.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpUseful.Font = new Font(btnTpHm.Font, FontStyle.Regular);
            }
            else if (((Button)sender).Text == "Решения")
            {
                btnTpHm.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpMMMK.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpAnswers.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpDecision.Font = new Font(btnTpHm.Font, FontStyle.Underline);
                btnTpUseful.Font = new Font(btnTpHm.Font, FontStyle.Regular);
            }
            else if (((Button)sender).Text == "Полезное")
            {
                btnTpHm.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpMMMK.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpAnswers.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpDecision.Font = new Font(btnTpHm.Font, FontStyle.Regular);
                btnTpUseful.Font = new Font(btnTpHm.Font, FontStyle.Underline);
            }
        }

        private void lblMainChanelMM_Click(object sender, EventArgs e)
        {
            PingT(((Label)sender).Text);
        }

        //private void menuSql_Click(object sender, EventArgs e)
        //{
        //    Sql sql_form = new Sql();
        //    sql_form.Show();
        //}

        private void CbIpGM_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillAllFieldsHM(((ComboBox)sender).Tag.ToString());
        }

        private void CbNameGM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FillAllFieldsHM(((ComboBox)sender).Tag.ToString());
            }
        }

        private void CbNameGM_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillAllFieldsHM(((ComboBox)sender).Tag.ToString());
        }

        private void cbNameMM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FillAllFieldsMM(((ComboBox)sender).Tag.ToString());
            }
        }

        private void cbNameMM_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillAllFieldsMM(((ComboBox)sender).Tag.ToString());
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        public void wright_log(string mess)
        {
            try
            {
                using (StreamWriter sw_log = new StreamWriter("log.log", true))
                {
                    sw_log.WriteLine(DateTime.Now.ToString() + ": " + mess);
                    sw_log.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "logging error");
                return;
            }

        }
    }
}
