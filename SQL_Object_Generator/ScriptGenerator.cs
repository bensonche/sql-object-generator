using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Object_Generator
{
    public class ScriptGenerator
    {
        public string ServerName;
        public string DbName;
        public string Username;
        public string Password;

        public bool Integrated;

        public string OutputDir;

        private string ConnectionString
        {
            get
            {
                if (Integrated)
                    return string.Format(@"Server={0};Database={1};Trusted_Connection=True",
                        ServerName,
                        DbName);

                return string.Format(@"Server={0};Database={1};User Id={2};Password={3};",
                    ServerName,
                    DbName,
                    Username,
                    Password);
            }
        }

        public async Task GenerateAsync()
        {
            SqlConnection con = new SqlConnection(ConnectionString);

            await con.OpenAsync();

            if (con.State != ConnectionState.Open)
                throw new Exception("Cannot open database");

            Task triggers = GetTriggers(OutputDir, con);

            await triggers;

            con.Close();
        }

        private async Task GetTriggers(string outputDir, SqlConnection con)
        {
            string commandText = @"
                select a.name, b.definition
                from sys.triggers a
                    inner join sys.sql_modules b
                        on a.object_id = b.object_id";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[{0}]'))");
            sb.AppendLine("DROP TRIGGER [dbo].[{0}]");
            sb.AppendLine("GO");
            sb.AppendLine("SET ANSI_NULLS ON");
            sb.AppendLine("GO");
            sb.AppendLine("SET QUOTED_IDENTIFIER ON");
            sb.AppendLine("GO");
            sb.AppendLine("{1}");
            sb.AppendLine("GO");

            GenerateObjectScript(outputDir, "triggers", con, commandText, sb.ToString());
        }

        private void GenerateObjectScript(string outputDir, string description, SqlConnection con, string commandText,
            string body)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = commandText;

            var reader = cmd.ExecuteReader();

            DirectoryInfo d = new DirectoryInfo(Path.Combine(outputDir, description));

            if (!d.Exists)
                d.Create();

            foreach (var f in d.GetFiles())
                f.Delete();

            while (reader.Read())
            {
                string name = reader["name"].ToString();
                string definition = reader["definition"].ToString();
                FileInfo f = new FileInfo(Path.Combine(d.FullName, string.Format("{0}.sql", reader["name"])));

                using (Stream s = f.Create())
                {
                    using (StreamWriter w = new StreamWriter(s))
                    {
                        w.Write(body, name, definition);
                    }
                }
            }
        }
    }
}
