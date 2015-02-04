using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Object_Generator
{
    public partial class Form1 : Form
    {
        private ScriptGenerator _generator;

        public Form1()
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

            txtServerName.Text = "sql-intranet2";
            txtDatabaseName.Text = "RDI_Development";
            txtDirectory.Text = "c:\\temp\\test";
        }

        private void Form1_Resize(object sender, EventArgs e)
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

        private void Form1_Load(object sender, EventArgs e)
        {
            ToggleAuthentication();
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            btnGenerate.Enabled = false;

            if (!ValidateForm())
            {
                btnGenerate.Enabled = true;
                return;
            }

            _generator = new ScriptGenerator
            {
                ServerName = txtServerName.Text,
                DbName = txtDatabaseName.Text,
                Username = txtUsername.Text,
                Password = txtPassword.Text,
                Integrated = rdbIntegrated.Checked,
                OutputDir = txtDirectory.Text
            };

            try
            {
                Task gen = _generator.GenerateAsync();

                System.Timers.Timer timer = new System.Timers.Timer(1000);
                timer.Elapsed += timer_Elapsed;
                timer.AutoReset = true;
                timer.Enabled = true;

                await gen;

                timer.Enabled = false;
                timer_Elapsed();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error connecting to database");
            }
            finally
            {
                btnGenerate.Enabled = true;
            }
        }

        private void timer_Elapsed(object sender = null, EventArgs e = null)
        {
            string procs = _generator.ProcsRemaining == -1 ? "done" : _generator.ProcsRemaining.ToString();
            string functions = _generator.FunctionsRemaining == -1 ? "done" : _generator.FunctionsRemaining.ToString();
            string triggers = _generator.TriggersRemaining == -1 ? "done" : _generator.TriggersRemaining.ToString();

            Invoke(new Action(() =>
            {
                lblStatus.Text = "Procs: " + procs + '\n';
                lblStatus.Text += "Functions: " + functions + '\n';
                lblStatus.Text += "Triggers: " + triggers + '\n';
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
