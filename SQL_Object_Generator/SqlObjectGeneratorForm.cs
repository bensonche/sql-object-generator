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

namespace SQL_Object_Generator
{
    public partial class SqlObjectGeneratorForm : Form
    {
        private ScriptGenerator _generator;

        private const string settingsFileName = "settings.xml";

        public SqlObjectGeneratorForm()
        {
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
            try
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(settingsFileName, FileMode.OpenOrCreate, FileAccess.Write, isoStore))
                {
                    XElement element =
                        new XElement("config",
                            new XElement("appSettings",
                                new XElement("server", txtServerName.Text),
                                new XElement("database", txtDatabaseName.Text),
                                new XElement("directory", txtDirectory.Text),
                                new XElement("username", txtUsername.Text),
                                new XElement("password", txtPassword.Text),
                                new XElement("integrated", rdbIntegrated.Checked.ToString())
                            )
                        );

                    byte[] data = Encoding.UTF8.GetBytes(element.ToString());

                    byte[] protectedData = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);

                    isoStream.Write(protectedData, 0, protectedData.Length);
                }
            }
            catch { }
        }

        private void LoadSettings()
        {
            try
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

                if (!isoStore.FileExists(settingsFileName))
                    return;

                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(settingsFileName, FileMode.Open, FileAccess.Read, isoStore))
                {
                    byte[] protectedData = new byte[isoStream.Length + 10];
                    int numBytesToRead = (int)isoStream.Length;
                    int numBytesRead = 0;
                    do
                    {
                        // Read may return anything from 0 to 10. 
                        int n = isoStream.Read(protectedData, numBytesRead, 10);
                        numBytesRead += n;
                        numBytesToRead -= n;
                    } while (numBytesToRead > 0);

                    byte[] data = ProtectedData.Unprotect(protectedData, null, DataProtectionScope.CurrentUser);

                    XElement element = XElement.Parse(Encoding.UTF8.GetString(data));

                    txtServerName.Text = GetSettingValue(element, "server");
                    txtDatabaseName.Text = GetSettingValue(element, "database");
                    txtDirectory.Text = GetSettingValue(element, "directory");
                    txtUsername.Text = GetSettingValue(element, "username");
                    txtPassword.Text = GetSettingValue(element, "password");

                    if (bool.Parse(GetSettingValue(element, "integrated")))
                        rdbIntegrated.Checked = true;
                    else
                        rdbSql.Checked = true;
                }
            }
            catch { }
        }

        private string GetSettingValue(XElement element, string key)
        {
            return (from field in element.Elements("appSettings").Elements(key)
                    select field.Value).FirstOrDefault() ?? "";

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
