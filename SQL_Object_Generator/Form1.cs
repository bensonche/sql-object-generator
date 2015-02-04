using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SQL_Object_Generator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            gpAuthentication.MinimumSize = new Size(Size.Width - 41, 0);
        }

        private void toggleAuthentication(object sender = null, EventArgs e = null)
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
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select the directory to output the scripts.";
            dialog.ShowNewFolderButton = true;
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtDirectory.Text = dialog.SelectedPath;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toggleAuthentication();
        }
    }
}
