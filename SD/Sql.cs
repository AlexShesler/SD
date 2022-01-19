//using FirebirdSql.Data.FirebirdClient;
//using Renci.SshNet;
//using System;
//using System.Data;
//using System.Data.Common;
//using System.IO;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Windows.Forms;

//namespace SD
//{
//    public partial class Sql : Form
//    {
//        private FbTransactionOptions fbto;
//        private FbTransaction fbt;
//        private string key;
//        public string Key_mm = "-----BEGIN RSA PRIVATE KEY-----\r\nMIIEoQIBAAKCAQEA4A4VjZDoUvqqV7oVOY2oCfMMix32SWvCo6aY7zXr9GlJXGCq\r\nUDRJRa2qA/+w9WsggS2CThx9iC+AbMo++rZsIvcsPHVBJsbql4oHPPwQZDpjXN1d\r\nrBoTlnS/Gs6Oaiiyptq4hl8rYQMYQxorZhszsI7gCVz1wISmgBGCJfa5ewiuUp+t\r\nFgoL34vXZgAZX4MLqZ0pCGLvzPK2Z/0axlu/EFbzoOFcb0vdDHMbgI/ier7Eg301\r\nwlOgO7wKJioC1xCkXX1hoQqWywJOhssZAIkwB7kcuLE7p6qjT4cJYgn3dOJaD3bN\r\nh72KstlKMI4e1FcGhiQmAgODKNR0wKTRWY9PkQIBIwKCAQEAkzx0kDqnTHjWVuf/\r\nUbTcI8uah040IZ6V2UGXsyNvK5WlPLSNLWQwJnlvuXwj1HmZBG5c8YBvv+SzerDK\r\nTPuI48b/z/VIEirx7o3nf9jXklJBS6dpcRhzRZ0tID6YGeA64qz86pZPtMeE+OVP\r\nuCCBDZ+3y6N87Dnpwd+eqzu7tz78+Ci7lFKcvqWaVI8D41FgvQFWiUEbrCEdrpPU\r\nvGEyFt3Dwp5QBm2d8t4a3TXZfyTXWG+ZIIKkjAgRj8vmKyJ4P97VMdoR35QZOjfo\r\nhJ5R/oAjQEn4iS70E5P7yi8RpqdpbWAz4Y009aKeVRZNiJn+G1Pb4axd4eNAcf/l\r\nLlaIOwKBgQD6SaoypQdUSKjIC93XCE65WN5uyVm8hf54vPLp4J60oJRvNnPbZHAX\r\nJuZaKWR0JQ5+mC8TmMhx8c4CKQC939guWpg2waNbbvXjU8Uys/LqjEU14nJn0dPC\r\nQV8m64hn5vrT9YJO2bBnVs2uVcLMZ5fjfTAqj72cg1/Ta3jAWHe23QKBgQDlKybM\r\nTIUAt44DHD1/NPAjR2Ol3XmZ3NgboWDiCds4pgG+sZLavks5tsBVrYBxRkuwTnfJ\r\n1q4doVwwDBRhJOKdobN3JDzHHhTKX/68Hpo4A0y8UwXqUv67jDBub1tTTYSXwW06\r\n3GLHo3hR3uEI9esPp7cnR2D+vQSNLUJ8mS++RQKBgEBcFdKCNRWsSKh4FHkfZLNR\r\nXcS3bti0vcdGh5s5wmj2F4pP1KYhJCM143ZFKHWjIPv7P04umfFqEGb0mcpswo+T\r\no3vLZIU5yjMkK2TPL9XpjiPMg9GNuhwCLmkX/oEeIz3RaqaPv6WLWXX4y7DncDMu\r\n0d8PBN8ad7oFsVYIHsifAoGAVR6oAr1V+PO4Z44leGQergSSufMtKoU6U2fThyg7\r\ne3DcE6EKqQTaHMA4z12zXUX+2xXUuK7S9RAMTFxCFXQZqENf6myLmmpCPIoOKJ2m\r\n/tyneSYmxL+9s1/0u03uuIqBtLWO+JsAHkQIHmi5/AOR0p1hSRqDHMndHn6GaMsu\r\n/YcCgYACvFIFLSRXhrbthA7zwaY0UeTzi/Xm3VYtdTkHcjxVLgq67bzgFvii0vAm\r\nGNUVEak5eRuzTn8vUp94fef/GB/RcuA6kzEPo219oeFQcSaTr6RD6icrrK4Cpfpg\r\nWOEIIxgUPMSLoKqkSECEpuRU4F6ubDqPrmaDs1k+5wnuRo5dwA==\r\n-----END RSA PRIVATE KEY-----";
//        public Sql()
//        {
//            InitializeComponent();
//        }

//        //string sqlFile = "";
        

//        private void btnSqlOpen_Click(object sender, EventArgs e)
//        {
//            OpenFileDialog ofd = new OpenFileDialog();
//            ofd.InitialDirectory = Application.StartupPath;

//            if (ofd.ShowDialog() == DialogResult.OK && ofd.FileName.Length > 0)
//            {
//                Constants.SqlFile = ofd.FileName;
//                tecSql.LoadFile(ofd.FileName);
//                tecSql.Highlighting = "SQL";
//            }
//        }

//        private void btnSqlSave_Click(object sender, EventArgs e)
//        {
//            SaveFileDialog sfd = new SaveFileDialog();
//            sfd.InitialDirectory = Application.StartupPath;

//            if (sfd.ShowDialog() == DialogResult.OK && sfd.FileName.Length > 0)
//            {
//                tecSql.SaveFile(sfd.FileName);
//                MessageBox.Show("Выполнено", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//        }

//        private void Sql_Load(object sender, EventArgs e)
//        {
//            tecSql.LoadFile(Constants.SqlFile);
//            tecSql.Highlighting = "SQL";
//        }

        

//        key = Key_mm;
//        fbto = (FbTransactionOptions)null;
//        ((FbTransactionOptions)ref this.fbto).set_TransactionBehavior((FbTransactionBehavior)10272);

//        public DataTable execute_request(string sql_text, string connect)
//        {
//            DataTable dataTable = new DataTable();
//            string oldValue = "";
//            Regex regex1 = new Regex("om\\w*.\\w*.\\w*.\\w*.ru");
//            if (regex1.IsMatch(connect))
//                oldValue = regex1.Match(connect).Value;
//            Regex regex2 = new Regex("([0-9]{1,3}[\\.]){3}[0-9]{1,3}");
//            if (regex2.IsMatch(connect))
//                oldValue = regex2.Match(connect).Value;
//            connect = connect.Replace("3050", "44000").Replace(oldValue, "127.0.0.1");
//            PrivateKeyConnectionInfo keyConnectionInfo = new PrivateKeyConnectionInfo(oldValue, 2223, "root", new PrivateKeyFile[1]
//            {
//                new PrivateKeyFile((Stream) new MemoryStream(Encoding.ASCII.GetBytes(this.key)))
//            });
//            try
//            {
//                using (SshClient sshClient = new SshClient((ConnectionInfo)keyConnectionInfo))
//                {
//                    ((BaseClient)sshClient).get_ConnectionInfo().set_Encoding(Encoding.GetEncoding(1251));
//                    ((BaseClient)sshClient).Connect();
//                    ForwardedPortLocal forwardedPortLocal = new ForwardedPortLocal("127.0.0.1", 44000U, "ibmd", 3050U);
//                    sshClient.AddForwardedPort((ForwardedPort)forwardedPortLocal);
//                    ((ForwardedPort)forwardedPortLocal).Start();
//                    using (FbConnection fbConnection = new FbConnection(connect))
//                    {
//                        this.fbt = (FbTransaction)null;
//                        try
//                        {
//                            ((DbConnection)fbConnection).Open();
//                            this.fbt = fbConnection.BeginTransaction(this.fbto);
//                            using (FbCommand fbCommand = new FbCommand(sql_text, fbConnection, this.fbt))
//                                dataTable.Load((IDataReader)fbCommand.ExecuteReader());
//                            ((DbTransaction)this.fbt).Commit();
//                        }
//                        catch (Exception ex1)
//                        {
//                            try
//                            {
//                                if (this.fbt != null)
//                                    ((DbTransaction)this.fbt).Rollback();
//                                int num = (int)MessageBox.Show("Ошибка запроса данных. " + ex1.Message, "Внимание");
//                            }
//                            catch (Exception ex2)
//                            {
//                                int num = (int)MessageBox.Show("Ошибка отката транзакции. " + ex2.Message, "Внимание");
//                            }
//                            return (DataTable)null;
//                        }
//                    }
//                  ((ForwardedPort)forwardedPortLocal).Stop();
//                }
//            }
//            catch (Exception ex)
//            {
//                int num = (int)XtraMessageBox.Show("Неизвестная ошибка. " + ex.Message, "Внимание");
//                return (DataTable)null;
//            }
//            return dataTable;
//        }
//    }
//}
