namespace SQL_Object_Generator
{
    partial class SqlObjectGeneratorForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.txtDatabaseName = new System.Windows.Forms.TextBox();
            this.rdbIntegrated = new System.Windows.Forms.RadioButton();
            this.rdbSql = new System.Windows.Forms.RadioButton();
            this.gpAuthentication = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblDirectory = new System.Windows.Forms.Label();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.btnBrowseDir = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.gpAuthentication.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Database Name";
            // 
            // txtServerName
            // 
            this.txtServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerName.Location = new System.Drawing.Point(102, 12);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(170, 20);
            this.txtServerName.TabIndex = 3;
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabaseName.Location = new System.Drawing.Point(102, 38);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.Size = new System.Drawing.Size(170, 20);
            this.txtDatabaseName.TabIndex = 4;
            // 
            // rdbIntegrated
            // 
            this.rdbIntegrated.AutoSize = true;
            this.rdbIntegrated.Checked = true;
            this.rdbIntegrated.Location = new System.Drawing.Point(6, 19);
            this.rdbIntegrated.Name = "rdbIntegrated";
            this.rdbIntegrated.Size = new System.Drawing.Size(73, 17);
            this.rdbIntegrated.TabIndex = 5;
            this.rdbIntegrated.TabStop = true;
            this.rdbIntegrated.Text = "Integrated";
            this.rdbIntegrated.UseVisualStyleBackColor = true;
            this.rdbIntegrated.CheckedChanged += new System.EventHandler(this.ToggleAuthentication);
            // 
            // rdbSql
            // 
            this.rdbSql.AutoSize = true;
            this.rdbSql.Location = new System.Drawing.Point(85, 19);
            this.rdbSql.Name = "rdbSql";
            this.rdbSql.Size = new System.Drawing.Size(46, 17);
            this.rdbSql.TabIndex = 6;
            this.rdbSql.TabStop = true;
            this.rdbSql.Text = "SQL";
            this.rdbSql.UseVisualStyleBackColor = true;
            this.rdbSql.CheckedChanged += new System.EventHandler(this.ToggleAuthentication);
            // 
            // gpAuthentication
            // 
            this.gpAuthentication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpAuthentication.AutoSize = true;
            this.gpAuthentication.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gpAuthentication.Controls.Add(this.txtPassword);
            this.gpAuthentication.Controls.Add(this.txtUsername);
            this.gpAuthentication.Controls.Add(this.lblPassword);
            this.gpAuthentication.Controls.Add(this.lblUsername);
            this.gpAuthentication.Controls.Add(this.rdbIntegrated);
            this.gpAuthentication.Controls.Add(this.rdbSql);
            this.gpAuthentication.Location = new System.Drawing.Point(13, 64);
            this.gpAuthentication.MinimumSize = new System.Drawing.Size(259, 0);
            this.gpAuthentication.Name = "gpAuthentication";
            this.gpAuthentication.Size = new System.Drawing.Size(259, 113);
            this.gpAuthentication.TabIndex = 7;
            this.gpAuthentication.TabStop = false;
            this.gpAuthentication.Text = "Authentication";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(67, 74);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(186, 20);
            this.txtPassword.TabIndex = 10;
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(67, 42);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(186, 20);
            this.txtUsername.TabIndex = 9;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(8, 77);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 8;
            this.lblPassword.Text = "Password";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(6, 45);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 13);
            this.lblUsername.TabIndex = 7;
            this.lblUsername.Text = "Username";
            // 
            // lblDirectory
            // 
            this.lblDirectory.AutoSize = true;
            this.lblDirectory.Location = new System.Drawing.Point(14, 186);
            this.lblDirectory.Name = "lblDirectory";
            this.lblDirectory.Size = new System.Drawing.Size(93, 13);
            this.lblDirectory.TabIndex = 8;
            this.lblDirectory.Text = "Directory Location";
            // 
            // txtDirectory
            // 
            this.txtDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDirectory.Location = new System.Drawing.Point(113, 183);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(127, 20);
            this.txtDirectory.TabIndex = 9;
            // 
            // btnBrowseDir
            // 
            this.btnBrowseDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseDir.AutoSize = true;
            this.btnBrowseDir.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBrowseDir.Location = new System.Drawing.Point(246, 183);
            this.btnBrowseDir.Name = "btnBrowseDir";
            this.btnBrowseDir.Size = new System.Drawing.Size(26, 23);
            this.btnBrowseDir.TabIndex = 10;
            this.btnBrowseDir.Text = "...";
            this.btnBrowseDir.UseVisualStyleBackColor = true;
            this.btnBrowseDir.Click += new System.EventHandler(this.btnBrowseDir_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(197, 227);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 11;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 214);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(56, 39);
            this.lblStatus.TabIndex = 12;
            this.lblStatus.Text = "Procs:\r\nFunctions:\r\nTriggers:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.btnBrowseDir);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.lblDirectory);
            this.Controls.Add(this.gpAuthentication);
            this.Controls.Add(this.txtDatabaseName);
            this.Controls.Add(this.txtServerName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.SqlObjectGeneratorForm_Load);
            this.Resize += new System.EventHandler(this.SqlObjectGeneratorForm_Resize);
            this.gpAuthentication.ResumeLayout(false);
            this.gpAuthentication.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.TextBox txtDatabaseName;
        private System.Windows.Forms.RadioButton rdbIntegrated;
        private System.Windows.Forms.RadioButton rdbSql;
        private System.Windows.Forms.GroupBox gpAuthentication;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblDirectory;
        private System.Windows.Forms.TextBox txtDirectory;
        private System.Windows.Forms.Button btnBrowseDir;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblStatus;


    }
}

