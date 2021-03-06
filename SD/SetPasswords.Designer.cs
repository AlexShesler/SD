namespace SD
{
    partial class SetPasswords
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetPasswords));
            this.txbLoginBd = new System.Windows.Forms.TextBox();
            this.btnShowPass7 = new System.Windows.Forms.Button();
            this.txbPassBd = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txbLoginDigitprice = new System.Windows.Forms.TextBox();
            this.txbLoginSalepoint = new System.Windows.Forms.TextBox();
            this.txbLoginPuttyDigi = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnShowPass6 = new System.Windows.Forms.Button();
            this.txbPassDigitprice = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnShowPass3 = new System.Windows.Forms.Button();
            this.txbPassSalepoint = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnShowPass2 = new System.Windows.Forms.Button();
            this.txbPassPuttyDigi = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnShowPass1 = new System.Windows.Forms.Button();
            this.txbPassUvnc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txbSSHPass = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnShowPass8 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txbLoginBd
            // 
            this.txbLoginBd.Location = new System.Drawing.Point(13, 201);
            this.txbLoginBd.Name = "txbLoginBd";
            this.txbLoginBd.Size = new System.Drawing.Size(130, 20);
            this.txbLoginBd.TabIndex = 113;
            // 
            // btnShowPass7
            // 
            this.btnShowPass7.Image = ((System.Drawing.Image)(resources.GetObject("btnShowPass7.Image")));
            this.btnShowPass7.Location = new System.Drawing.Point(248, 202);
            this.btnShowPass7.Name = "btnShowPass7";
            this.btnShowPass7.Size = new System.Drawing.Size(30, 18);
            this.btnShowPass7.TabIndex = 128;
            this.btnShowPass7.Tag = "Bd";
            this.btnShowPass7.UseVisualStyleBackColor = true;
            this.btnShowPass7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseDown);
            this.btnShowPass7.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseUp);
            // 
            // txbPassBd
            // 
            this.txbPassBd.Location = new System.Drawing.Point(149, 201);
            this.txbPassBd.Name = "txbPassBd";
            this.txbPassBd.PasswordChar = '•';
            this.txbPassBd.Size = new System.Drawing.Size(130, 20);
            this.txbPassBd.TabIndex = 114;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 185);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(223, 13);
            this.label7.TabIndex = 125;
            this.label7.Text = "Логин и пароль для подключения к БД ГМ";
            // 
            // txbLoginDigitprice
            // 
            this.txbLoginDigitprice.Location = new System.Drawing.Point(13, 162);
            this.txbLoginDigitprice.Name = "txbLoginDigitprice";
            this.txbLoginDigitprice.Size = new System.Drawing.Size(130, 20);
            this.txbLoginDigitprice.TabIndex = 111;
            // 
            // txbLoginSalepoint
            // 
            this.txbLoginSalepoint.Location = new System.Drawing.Point(13, 122);
            this.txbLoginSalepoint.Name = "txbLoginSalepoint";
            this.txbLoginSalepoint.Size = new System.Drawing.Size(130, 20);
            this.txbLoginSalepoint.TabIndex = 108;
            // 
            // txbLoginPuttyDigi
            // 
            this.txbLoginPuttyDigi.Location = new System.Drawing.Point(13, 69);
            this.txbLoginPuttyDigi.Name = "txbLoginPuttyDigi";
            this.txbLoginPuttyDigi.Size = new System.Drawing.Size(130, 20);
            this.txbLoginPuttyDigi.TabIndex = 106;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(138, 276);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 115;
            this.btnOK.Text = "ОК";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(219, 276);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 116;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnShowPass6
            // 
            this.btnShowPass6.Image = ((System.Drawing.Image)(resources.GetObject("btnShowPass6.Image")));
            this.btnShowPass6.Location = new System.Drawing.Point(248, 163);
            this.btnShowPass6.Name = "btnShowPass6";
            this.btnShowPass6.Size = new System.Drawing.Size(30, 18);
            this.btnShowPass6.TabIndex = 124;
            this.btnShowPass6.Tag = "Digitprice";
            this.btnShowPass6.UseVisualStyleBackColor = true;
            this.btnShowPass6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseDown);
            this.btnShowPass6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseUp);
            // 
            // txbPassDigitprice
            // 
            this.txbPassDigitprice.Location = new System.Drawing.Point(149, 162);
            this.txbPassDigitprice.Name = "txbPassDigitprice";
            this.txbPassDigitprice.PasswordChar = '•';
            this.txbPassDigitprice.Size = new System.Drawing.Size(130, 20);
            this.txbPassDigitprice.TabIndex = 112;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 146);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(291, 13);
            this.label6.TabIndex = 121;
            this.label6.Text = "Логин и пароль для подключения к серверу ЭЦ по SSH:";
            // 
            // btnShowPass3
            // 
            this.btnShowPass3.Image = ((System.Drawing.Image)(resources.GetObject("btnShowPass3.Image")));
            this.btnShowPass3.Location = new System.Drawing.Point(248, 123);
            this.btnShowPass3.Name = "btnShowPass3";
            this.btnShowPass3.Size = new System.Drawing.Size(30, 18);
            this.btnShowPass3.TabIndex = 126;
            this.btnShowPass3.Tag = "Salepoint";
            this.btnShowPass3.UseVisualStyleBackColor = true;
            this.btnShowPass3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseDown);
            this.btnShowPass3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseUp);
            // 
            // txbPassSalepoint
            // 
            this.txbPassSalepoint.Location = new System.Drawing.Point(149, 122);
            this.txbPassSalepoint.Name = "txbPassSalepoint";
            this.txbPassSalepoint.PasswordChar = '•';
            this.txbPassSalepoint.Size = new System.Drawing.Size(130, 20);
            this.txbPassSalepoint.TabIndex = 109;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(259, 26);
            this.label3.TabIndex = 120;
            this.label3.Text = "Логин и пароль для подключении к кассе по SSH\r\n(рекомендуется использовать логин " +
    "cashier):";
            // 
            // btnShowPass2
            // 
            this.btnShowPass2.Image = ((System.Drawing.Image)(resources.GetObject("btnShowPass2.Image")));
            this.btnShowPass2.Location = new System.Drawing.Point(248, 70);
            this.btnShowPass2.Name = "btnShowPass2";
            this.btnShowPass2.Size = new System.Drawing.Size(30, 18);
            this.btnShowPass2.TabIndex = 119;
            this.btnShowPass2.Tag = "PuttyDigi";
            this.btnShowPass2.UseVisualStyleBackColor = true;
            this.btnShowPass2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseDown);
            this.btnShowPass2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseUp);
            // 
            // txbPassPuttyDigi
            // 
            this.txbPassPuttyDigi.Location = new System.Drawing.Point(149, 69);
            this.txbPassPuttyDigi.Name = "txbPassPuttyDigi";
            this.txbPassPuttyDigi.PasswordChar = '•';
            this.txbPassPuttyDigi.Size = new System.Drawing.Size(130, 20);
            this.txbPassPuttyDigi.TabIndex = 107;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(285, 13);
            this.label2.TabIndex = 118;
            this.label2.Text = "Логин и пароль для подключении к весам Digi по SSH:";
            // 
            // btnShowPass1
            // 
            this.btnShowPass1.Image = ((System.Drawing.Image)(resources.GetObject("btnShowPass1.Image")));
            this.btnShowPass1.Location = new System.Drawing.Point(112, 31);
            this.btnShowPass1.Margin = new System.Windows.Forms.Padding(0);
            this.btnShowPass1.Name = "btnShowPass1";
            this.btnShowPass1.Size = new System.Drawing.Size(30, 18);
            this.btnShowPass1.TabIndex = 117;
            this.btnShowPass1.Tag = "Uvnc";
            this.btnShowPass1.UseVisualStyleBackColor = true;
            this.btnShowPass1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseDown);
            this.btnShowPass1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseUp);
            // 
            // txbPassUvnc
            // 
            this.txbPassUvnc.Location = new System.Drawing.Point(13, 30);
            this.txbPassUvnc.Name = "txbPassUvnc";
            this.txbPassUvnc.PasswordChar = '•';
            this.txbPassUvnc.Size = new System.Drawing.Size(130, 20);
            this.txbPassUvnc.TabIndex = 105;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 13);
            this.label1.TabIndex = 127;
            this.label1.Text = "Пароль для UVnc при подключении к весам Digi:";
            // 
            // txbSSHPass
            // 
            this.txbSSHPass.Location = new System.Drawing.Point(13, 240);
            this.txbSSHPass.Name = "txbSSHPass";
            this.txbSSHPass.PasswordChar = '•';
            this.txbSSHPass.Size = new System.Drawing.Size(130, 20);
            this.txbSSHPass.TabIndex = 129;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 224);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 13);
            this.label4.TabIndex = 130;
            this.label4.Text = "Пароль для SSH IBMD";
            // 
            // btnShowPass8
            // 
            this.btnShowPass8.Image = ((System.Drawing.Image)(resources.GetObject("btnShowPass8.Image")));
            this.btnShowPass8.Location = new System.Drawing.Point(112, 241);
            this.btnShowPass8.Margin = new System.Windows.Forms.Padding(0);
            this.btnShowPass8.Name = "btnShowPass8";
            this.btnShowPass8.Size = new System.Drawing.Size(30, 18);
            this.btnShowPass8.TabIndex = 131;
            this.btnShowPass8.Tag = "Uvnc";
            this.btnShowPass8.UseVisualStyleBackColor = true;
            this.btnShowPass8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseDown);
            this.btnShowPass8.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnShowPass1_MouseUp);
            // 
            // SetPasswords
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 304);
            this.Controls.Add(this.btnShowPass8);
            this.Controls.Add(this.txbSSHPass);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txbLoginBd);
            this.Controls.Add(this.btnShowPass7);
            this.Controls.Add(this.txbPassBd);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txbLoginDigitprice);
            this.Controls.Add(this.txbLoginSalepoint);
            this.Controls.Add(this.txbLoginPuttyDigi);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnShowPass6);
            this.Controls.Add(this.txbPassDigitprice);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnShowPass3);
            this.Controls.Add(this.txbPassSalepoint);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnShowPass2);
            this.Controls.Add(this.txbPassPuttyDigi);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnShowPass1);
            this.Controls.Add(this.txbPassUvnc);
            this.Controls.Add(this.label1);
            this.Name = "SetPasswords";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SetPasswords";
            this.Load += new System.EventHandler(this.SetPasswords_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txbLoginBd;
        private System.Windows.Forms.Button btnShowPass7;
        private System.Windows.Forms.TextBox txbPassBd;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txbLoginDigitprice;
        private System.Windows.Forms.TextBox txbLoginSalepoint;
        private System.Windows.Forms.TextBox txbLoginPuttyDigi;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnShowPass6;
        private System.Windows.Forms.TextBox txbPassDigitprice;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnShowPass3;
        private System.Windows.Forms.TextBox txbPassSalepoint;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnShowPass2;
        private System.Windows.Forms.TextBox txbPassPuttyDigi;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnShowPass1;
        private System.Windows.Forms.TextBox txbPassUvnc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txbSSHPass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnShowPass8;
    }
}