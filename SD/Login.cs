using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SD
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            GetLogin();
        }

        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "Software\\SDTander";
        const string keyName = userRoot + "\\" + subkey;

        public string GetLogin()
        {
            try
            {
                var result = Registry.GetValue(keyName, "login", false)?.ToString();
                string login = result == null ? "false" : result.ToString();

                if (login != "false")
                {
                    txbLogin.Text = login;
                    txbPassword.Select();
                    return (login);
                }
                else return (login);
            }
            catch { return ("false"); }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Constants.UserLogin = txbLogin.Text;
            Constants.UserPass = txbPassword.Text;

            if (txbLogin.Text == "superuser" && txbPassword.Text == "superpas0000")
            {
                Form mainForm = new MainForm();
                MessageBox.Show("Учетные данные не заданы. ПО будет работать\nв режиме ручного ввода учетных данных.\n", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Mode.mode = false;
                mainForm.Show();
                mainForm.Text += @"     Администратор";
                Hide();
            }
            else
            {
                if (GetLogin() == "false")
                {
                    try
                    {
                        Registry.SetValue(keyName, "login", txbLogin.Text, RegistryValueKind.String);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Ошибка сохранения логина - нет прав на запись.");
                    }
                }

                try
                {
                    if (AD.ValidateCredentials(txbLogin.Text, txbPassword.Text))
                    {
                        if (AD.IsUserGroupMember(txbLogin.Text, "GM_support") || AD.IsUserGroupMember(txbLogin.Text, "coderepo"))
                        {
                            AD.SetUserData(txbLogin.Text, txbPassword.Text);
                            Form mainForm = new MainForm();
                            mainForm.Text += @"     " + AD.GetUserName(txbLogin.Text);
                            mainForm.Show();
                            Hide();
                        }
                        else
                        {
                            MessageBox.Show("Доступ запрещен", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txbPassword.Text = "";
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"Логин\Пароль введен не верно", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txbPassword.Text = "";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    txbPassword.Text = "";
                    return;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
