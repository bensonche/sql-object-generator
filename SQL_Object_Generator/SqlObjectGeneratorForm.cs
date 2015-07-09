using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using BC.ScriptGenerator;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using bche.SettingsManager;

namespace SQL_Object_Generator
{
    public partial class SqlObjectGeneratorForm : Form
    {
        private ScriptGenerator _generator;

        private const string settingsFileName = "settings.xml";

        private readonly SettingsManager Settings;

        public SqlObjectGeneratorForm()
        {
            Settings = new SettingsManager();

            InitializeComponent();

            txtServerName.Enter += Textbox_Enter;
            txtDatabaseName.Enter += Textbox_Enter;
            txtUsername.Enter += Textbox_Enter;
            txtPassword.Enter += Textbox_Enter;
            txtDirectory.Enter += Textbox_Enter;

            txtServerName.TextChanged += Textbox_Enter;
            txtDatabaseName.TextChanged += Textbox_Enter;
            txtUsername.TextChanged += Textbox_Enter;
            txtPassword.TextChanged += Textbox_Enter;
            txtDirectory.TextChanged += Textbox_Enter;

            LoadSettings();
        }

        private void SaveSettings()
        {
            Settings["server"] = txtServerName.Text;
            Settings["database"] = txtDatabaseName.Text;
            Settings["directory"] = txtDirectory.Text;
            Settings["username"] = txtUsername.Text;
            Settings["password"] = txtPassword.Text;
            Settings["integrated"] = rdbIntegrated.Checked.ToString();

            Settings.SaveSettings();
        }

        private void LoadSettings()
        {
            Settings.LoadSettings();

            txtServerName.Text = Settings["server"];
            txtDatabaseName.Text = Settings["database"];
            txtDirectory.Text = Settings["directory"];
            txtUsername.Text = Settings["username"];
            txtPassword.Text = Settings["password"];

            bool result;
            if (bool.TryParse(Settings["integrated"], out result))
            {
                if (result)
                    rdbIntegrated.Checked = true;
                else
                    rdbSql.Checked = true;
            }
        }

        private void SqlObjectGeneratorForm_Resize(object sender, EventArgs e)
        {
            gpAuthentication.MinimumSize = new Size(Size.Width - 41, 0);
        }

        private void ToggleAuthentication(object sender = null, EventArgs e = null)
        {
            bool visible = rdbSql.Checked;

            lblUsername.Visible = visible;
            lblPassword.Visible = visible;
            txtUsername.Visible = visible;
            txtPassword.Visible = visible;

            SetDirectoryInfoLayoutLocation();
        }

        private void SetDirectoryInfoLayoutLocation()
        {
            int y = gpAuthentication.Height;
            y += gpAuthentication.Location.Y;
            y += 9;

            lblDirectory.Location = new Point(lblDirectory.Location.X, y);
            txtDirectory.Location = new Point(txtDirectory.Location.X, y);
            btnBrowseDir.Location = new Point(btnBrowseDir.Location.X, y);
        }

        private void btnBrowseDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                Description = "Select the directory to output the scripts.",
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.MyComputer
            };

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtDirectory.Text = dialog.SelectedPath;
            }
        }

        private void SqlObjectGeneratorForm_Load(object sender, EventArgs e)
        {
            ToggleAuthentication();
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            SaveSettings();

            btnGenerate.Enabled = false;

            try
            {
                await GenerateAsync();
            }
            finally
            {
                btnGenerate.Enabled = true;
            }
        }

        private async Task GenerateAsync()
        {
            if (!ValidateForm())
                return;

            _generator = new ScriptGenerator(txtServerName.Text, txtDatabaseName.Text, txtUsername.Text, txtPassword.Text, rdbIntegrated.Checked, txtDirectory.Text);

            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = true;

            try
            {
                Task gen = _generator.GenerateAsync();

                timer.Enabled = true;

                await gen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error connecting to database");
            }
            finally
            {
                timer.Enabled = false;
                timer_Elapsed();
            }
        }

        private void timer_Elapsed(object sender = null, EventArgs e = null)
        {
            Invoke(new Action(() =>
            {
                lblStatus.Text = "";
                foreach(var type in _generator.ObjectList)
                {
                    lblStatus.Text += string.Format("{0}: {1}\n", type.Name, type.Remaining);
                }
            }));
        }

        private void Textbox_Enter(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (txt == null)
                return;

            txt.ResetBackColor();
        }

        private bool ValidateForm()
        {
            bool success = true;

            success &= ValidateControl(txtServerName);
            success &= ValidateControl(txtDatabaseName);
            success &= ValidateControl(txtDirectory);

            if (rdbSql.Checked)
            {
                success &= ValidateControl(txtUsername);
                success &= ValidateControl(txtPassword);
            }

            return success;
        }

        private bool ValidateControl(TextBox txt)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.BackColor = Color.Yellow;
                return false;
            }
            return true;
        }
    }
}
