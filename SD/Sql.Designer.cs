namespace SD
{
    partial class Sql
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Sql));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSqlSave = new System.Windows.Forms.Button();
            this.btnSqlOpen = new System.Windows.Forms.Button();
            this.txbCodeTO = new System.Windows.Forms.TextBox();
            this.btnSqlExec = new System.Windows.Forms.Button();
            this.tecSql = new ICSharpCode.TextEditor.TextEditorControl();
            this.dgvSql = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSql)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.btnSqlSave, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSqlOpen, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txbCodeTO, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSqlExec, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tecSql, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(883, 355);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnSqlSave
            // 
            this.btnSqlSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSqlSave.Location = new System.Drawing.Point(325, 1);
            this.btnSqlSave.Margin = new System.Windows.Forms.Padding(1, 1, 1, 0);
            this.btnSqlSave.Name = "btnSqlSave";
            this.btnSqlSave.Size = new System.Drawing.Size(106, 24);
            this.btnSqlSave.TabIndex = 4;
            this.btnSqlSave.Text = "Сохранить";
            this.btnSqlSave.UseVisualStyleBackColor = true;
            this.btnSqlSave.Click += new System.EventHandler(this.btnSqlSave_Click);
            // 
            // btnSqlOpen
            // 
            this.btnSqlOpen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSqlOpen.Location = new System.Drawing.Point(217, 1);
            this.btnSqlOpen.Margin = new System.Windows.Forms.Padding(1, 1, 1, 0);
            this.btnSqlOpen.Name = "btnSqlOpen";
            this.btnSqlOpen.Size = new System.Drawing.Size(106, 24);
            this.btnSqlOpen.TabIndex = 3;
            this.btnSqlOpen.Text = "Открыть";
            this.btnSqlOpen.UseVisualStyleBackColor = true;
            this.btnSqlOpen.Click += new System.EventHandler(this.btnSqlOpen_Click);
            // 
            // txbCodeTO
            // 
            this.txbCodeTO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbCodeTO.Location = new System.Drawing.Point(3, 3);
            this.txbCodeTO.Name = "txbCodeTO";
            this.txbCodeTO.Size = new System.Drawing.Size(102, 20);
            this.txbCodeTO.TabIndex = 1;
            // 
            // btnSqlExec
            // 
            this.btnSqlExec.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSqlExec.Location = new System.Drawing.Point(109, 1);
            this.btnSqlExec.Margin = new System.Windows.Forms.Padding(1, 1, 1, 0);
            this.btnSqlExec.Name = "btnSqlExec";
            this.btnSqlExec.Size = new System.Drawing.Size(106, 24);
            this.btnSqlExec.TabIndex = 2;
            this.btnSqlExec.Text = "Выполнить";
            this.btnSqlExec.UseVisualStyleBackColor = true;
            // 
            // tecSql
            // 
            this.tecSql.BackColor = System.Drawing.Color.DimGray;
            this.tableLayoutPanel1.SetColumnSpan(this.tecSql, 8);
            this.tecSql.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tecSql.Highlighting = "SQL";
            this.tecSql.Location = new System.Drawing.Point(1, 26);
            this.tecSql.Margin = new System.Windows.Forms.Padding(1);
            this.tecSql.Name = "tecSql";
            this.tecSql.ShowVRuler = false;
            this.tecSql.Size = new System.Drawing.Size(881, 328);
            this.tecSql.TabIndex = 0;
            // 
            // dgvSql
            // 
            this.dgvSql.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSql.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSql.Location = new System.Drawing.Point(0, 0);
            this.dgvSql.Name = "dgvSql";
            this.dgvSql.Size = new System.Drawing.Size(883, 164);
            this.dgvSql.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvSql);
            this.splitContainer1.Size = new System.Drawing.Size(883, 523);
            this.splitContainer1.SplitterDistance = 355;
            this.splitContainer1.TabIndex = 1;
            // 
            // Sql
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 523);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Sql";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sql_ТО";
            this.Load += new System.EventHandler(this.Sql_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSql)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ICSharpCode.TextEditor.TextEditorControl tecSql;
        private System.Windows.Forms.DataGridView dgvSql;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txbCodeTO;
        private System.Windows.Forms.Button btnSqlExec;
        private System.Windows.Forms.Button btnSqlSave;
        private System.Windows.Forms.Button btnSqlOpen;
    }
}