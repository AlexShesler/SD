namespace SD
{
    partial class Settings
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
            this.chbAutoUpdate = new System.Windows.Forms.CheckBox();
            this.btnPathUvnc = new System.Windows.Forms.Button();
            this.btnPathRms = new System.Windows.Forms.Button();
            this.txbUpHost = new System.Windows.Forms.TextBox();
            this.txbPathUpdate = new System.Windows.Forms.TextBox();
            this.txbPathUvnc = new System.Windows.Forms.TextBox();
            this.txbPathRms = new System.Windows.Forms.TextBox();
            this.lblUpHost = new System.Windows.Forms.Label();
            this.lblPathUvnc = new System.Windows.Forms.Label();
            this.lblPathUpdate = new System.Windows.Forms.Label();
            this.lblPathRms = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnPathPutty = new System.Windows.Forms.Button();
            this.txbPathPutty = new System.Windows.Forms.TextBox();
            this.lblPathPutty = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBD = new System.Windows.Forms.FolderBrowserDialog();
            this.btnPathWinScp = new System.Windows.Forms.Button();
            this.txbPathWinSCP = new System.Windows.Forms.TextBox();
            this.lblPathWinSCP = new System.Windows.Forms.Label();
            this.txbSshUser = new System.Windows.Forms.TextBox();
            this.lblSSHIbmd = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chbAutoUpdate
            // 
            this.chbAutoUpdate.AutoSize = true;
            this.chbAutoUpdate.Location = new System.Drawing.Point(23, 293);
            this.chbAutoUpdate.Name = "chbAutoUpdate";
            this.chbAutoUpdate.Size = new System.Drawing.Size(210, 17);
            this.chbAutoUpdate.TabIndex = 42;
            this.chbAutoUpdate.Text = "Автоматическое обновление БД ГМ";
            this.chbAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // btnPathUvnc
            // 
            this.btnPathUvnc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnPathUvnc.Location = new System.Drawing.Point(361, 109);
            this.btnPathUvnc.Name = "btnPathUvnc";
            this.btnPathUvnc.Size = new System.Drawing.Size(29, 23);
            this.btnPathUvnc.TabIndex = 40;
            this.btnPathUvnc.Text = "...";
            this.btnPathUvnc.UseVisualStyleBackColor = true;
            this.btnPathUvnc.Click += new System.EventHandler(this.btnPathUvnc_Click);
            // 
            // btnPathRms
            // 
            this.btnPathRms.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnPathRms.Location = new System.Drawing.Point(361, 69);
            this.btnPathRms.Name = "btnPathRms";
            this.btnPathRms.Size = new System.Drawing.Size(29, 23);
            this.btnPathRms.TabIndex = 41;
            this.btnPathRms.Text = "...";
            this.btnPathRms.UseVisualStyleBackColor = true;
            this.btnPathRms.Click += new System.EventHandler(this.btnPathRms_Click);
            // 
            // txbUpHost
            // 
            this.txbUpHost.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txbUpHost.Location = new System.Drawing.Point(23, 189);
            this.txbUpHost.Name = "txbUpHost";
            this.txbUpHost.Size = new System.Drawing.Size(330, 21);
            this.txbUpHost.TabIndex = 36;
            // 
            // txbPathUpdate
            // 
            this.txbPathUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txbPathUpdate.Location = new System.Drawing.Point(23, 229);
            this.txbPathUpdate.Name = "txbPathUpdate";
            this.txbPathUpdate.Size = new System.Drawing.Size(330, 21);
            this.txbPathUpdate.TabIndex = 37;
            // 
            // txbPathUvnc
            // 
            this.txbPathUvnc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txbPathUvnc.Location = new System.Drawing.Point(23, 109);
            this.txbPathUvnc.Name = "txbPathUvnc";
            this.txbPathUvnc.Size = new System.Drawing.Size(330, 21);
            this.txbPathUvnc.TabIndex = 38;
            // 
            // txbPathRms
            // 
            this.txbPathRms.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txbPathRms.Location = new System.Drawing.Point(23, 69);
            this.txbPathRms.Name = "txbPathRms";
            this.txbPathRms.Size = new System.Drawing.Size(330, 21);
            this.txbPathRms.TabIndex = 39;
            // 
            // lblUpHost
            // 
            this.lblUpHost.AutoSize = true;
            this.lblUpHost.Location = new System.Drawing.Point(23, 173);
            this.lblUpHost.Name = "lblUpHost";
            this.lblUpHost.Size = new System.Drawing.Size(117, 13);
            this.lblUpHost.TabIndex = 32;
            this.lblUpHost.Text = "Хост автообновления";
            // 
            // lblPathUvnc
            // 
            this.lblPathUvnc.AutoSize = true;
            this.lblPathUvnc.Location = new System.Drawing.Point(23, 93);
            this.lblPathUvnc.Name = "lblPathUvnc";
            this.lblPathUvnc.Size = new System.Drawing.Size(73, 13);
            this.lblPathUvnc.TabIndex = 33;
            this.lblPathUvnc.Text = "Путь к UVNC";
            // 
            // lblPathUpdate
            // 
            this.lblPathUpdate.AutoSize = true;
            this.lblPathUpdate.Location = new System.Drawing.Point(23, 213);
            this.lblPathUpdate.Name = "lblPathUpdate";
            this.lblPathUpdate.Size = new System.Drawing.Size(118, 13);
            this.lblPathUpdate.TabIndex = 34;
            this.lblPathUpdate.Text = "Ссылка на список ГМ";
            // 
            // lblPathRms
            // 
            this.lblPathRms.AutoSize = true;
            this.lblPathRms.Location = new System.Drawing.Point(23, 53);
            this.lblPathRms.Name = "lblPathRms";
            this.lblPathRms.Size = new System.Drawing.Size(67, 13);
            this.lblPathRms.TabIndex = 35;
            this.lblPathRms.Text = "Путь к RMS";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(234, 324);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 30;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(315, 324);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 31;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnPathPutty
            // 
            this.btnPathPutty.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnPathPutty.Location = new System.Drawing.Point(361, 29);
            this.btnPathPutty.Name = "btnPathPutty";
            this.btnPathPutty.Size = new System.Drawing.Size(29, 23);
            this.btnPathPutty.TabIndex = 29;
            this.btnPathPutty.Text = "...";
            this.btnPathPutty.UseVisualStyleBackColor = true;
            this.btnPathPutty.Click += new System.EventHandler(this.btnPathPutty_Click);
            // 
            // txbPathPutty
            // 
            this.txbPathPutty.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txbPathPutty.Location = new System.Drawing.Point(23, 29);
            this.txbPathPutty.Name = "txbPathPutty";
            this.txbPathPutty.Size = new System.Drawing.Size(330, 21);
            this.txbPathPutty.TabIndex = 28;
            // 
            // lblPathPutty
            // 
            this.lblPathPutty.AutoSize = true;
            this.lblPathPutty.Location = new System.Drawing.Point(23, 13);
            this.lblPathPutty.Name = "lblPathPutty";
            this.lblPathPutty.Size = new System.Drawing.Size(67, 13);
            this.lblPathPutty.TabIndex = 27;
            this.lblPathPutty.Text = "Путь к Putty";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // btnPathWinScp
            // 
            this.btnPathWinScp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnPathWinScp.Location = new System.Drawing.Point(362, 149);
            this.btnPathWinScp.Name = "btnPathWinScp";
            this.btnPathWinScp.Size = new System.Drawing.Size(29, 23);
            this.btnPathWinScp.TabIndex = 45;
            this.btnPathWinScp.Text = "...";
            this.btnPathWinScp.UseVisualStyleBackColor = true;
            this.btnPathWinScp.Click += new System.EventHandler(this.btnPathWinScp_Click);
            // 
            // txbPathWinSCP
            // 
            this.txbPathWinSCP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txbPathWinSCP.Location = new System.Drawing.Point(24, 149);
            this.txbPathWinSCP.Name = "txbPathWinSCP";
            this.txbPathWinSCP.Size = new System.Drawing.Size(330, 21);
            this.txbPathWinSCP.TabIndex = 44;
            // 
            // lblPathWinSCP
            // 
            this.lblPathWinSCP.AutoSize = true;
            this.lblPathWinSCP.Location = new System.Drawing.Point(24, 133);
            this.lblPathWinSCP.Name = "lblPathWinSCP";
            this.lblPathWinSCP.Size = new System.Drawing.Size(83, 13);
            this.lblPathWinSCP.TabIndex = 43;
            this.lblPathWinSCP.Text = "Путь к WinSCP";
            // 
            // txbSshUser
            // 
            this.txbSshUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txbSshUser.Location = new System.Drawing.Point(23, 269);
            this.txbSshUser.Name = "txbSshUser";
            this.txbSshUser.Size = new System.Drawing.Size(330, 21);
            this.txbSshUser.TabIndex = 47;
            // 
            // lblSSHIbmd
            // 
            this.lblSSHIbmd.AutoSize = true;
            this.lblSSHIbmd.Location = new System.Drawing.Point(23, 254);
            this.lblSSHIbmd.Name = "lblSSHIbmd";
            this.lblSSHIbmd.Size = new System.Drawing.Size(77, 13);
            this.lblSSHIbmd.TabIndex = 46;
            this.lblSSHIbmd.Text = "УЗ SSH IBMD";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(403, 352);
            this.Controls.Add(this.txbSshUser);
            this.Controls.Add(this.lblSSHIbmd);
            this.Controls.Add(this.btnPathWinScp);
            this.Controls.Add(this.txbPathWinSCP);
            this.Controls.Add(this.lblPathWinSCP);
            this.Controls.Add(this.chbAutoUpdate);
            this.Controls.Add(this.btnPathUvnc);
            this.Controls.Add(this.btnPathRms);
            this.Controls.Add(this.txbUpHost);
            this.Controls.Add(this.txbPathUpdate);
            this.Controls.Add(this.txbPathUvnc);
            this.Controls.Add(this.txbPathRms);
            this.Controls.Add(this.lblUpHost);
            this.Controls.Add(this.lblPathUvnc);
            this.Controls.Add(this.lblPathUpdate);
            this.Controls.Add(this.lblPathRms);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnPathPutty);
            this.Controls.Add(this.txbPathPutty);
            this.Controls.Add(this.lblPathPutty);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chbAutoUpdate;
        private System.Windows.Forms.Button btnPathUvnc;
        private System.Windows.Forms.Button btnPathRms;
        private System.Windows.Forms.TextBox txbUpHost;
        private System.Windows.Forms.TextBox txbPathUpdate;
        private System.Windows.Forms.TextBox txbPathUvnc;
        private System.Windows.Forms.TextBox txbPathRms;
        private System.Windows.Forms.Label lblUpHost;
        private System.Windows.Forms.Label lblPathUvnc;
        private System.Windows.Forms.Label lblPathUpdate;
        private System.Windows.Forms.Label lblPathRms;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnPathPutty;
        private System.Windows.Forms.TextBox txbPathPutty;
        private System.Windows.Forms.Label lblPathPutty;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBD;
        private System.Windows.Forms.Button btnPathWinScp;
        private System.Windows.Forms.TextBox txbPathWinSCP;
        private System.Windows.Forms.Label lblPathWinSCP;
        private System.Windows.Forms.TextBox txbSshUser;
        private System.Windows.Forms.Label lblSSHIbmd;
    }
}