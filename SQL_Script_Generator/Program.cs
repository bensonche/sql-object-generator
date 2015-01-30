using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SQL_Script_Generator
{
    internal class Program
    {
        private static string username
        {
            get { return ConfigurationManager.AppSettings["username"]; }
        }

        private static string password
        {
            get { return ConfigurationManager.AppSettings["password"]; }
        }

        private static string serverName
        {
            get { return ConfigurationManager.AppSettings["serverName"]; }
        }

        private static string dbName
        {
            get { return ConfigurationManager.AppSettings["dbName"]; }
        }

        private static bool integrated
        {
            get { return ConfigurationManager.AppSettings["integrated"].ToLower() == "true"; }
        }

        private const string outputDir = @"c:\temp\repeatable\";

        private static string connectionString
        {
            get
            {
                if(integrated)
                    return string.Format(@"Server={0};Database={1};Trusted_Connection=True",
                        serverName,
                        dbName);

                return string.Format(@"Server={0};Database={1};User Id={2};Password={3};",
                    serverName,
                    dbName,
                    username,
                    password);
            }
        }

        private static void Main(string[] args)
        {
            SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            GetTriggers(outputDir, con);

            con.Close();
        }

        private static void GetTriggers(string outputDir, SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                select a.name, b.definition
                from sys.triggers a
                    inner join sys.sql_modules b
                        on a.object_id = b.object_id";

            var reader = cmd.ExecuteReader();

            DirectoryInfo d = new DirectoryInfo(Path.Combine( outputDir, "triggers"));

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
                        w.WriteLine(@"IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[{0}]'))", name);
                        w.WriteLine(@"DROP TRIGGER [dbo].[{0}]", name);
                        w.WriteLine("GO");
                        w.WriteLine("SET ANSI_NULLS ON");
                        w.WriteLine("GO");
                        w.WriteLine("SET QUOTED_IDENTIFIER ON");
                        w.WriteLine("GO");
                        w.WriteLine(definition);
                        w.WriteLine("GO");
                    }
                }
            }
        }
    }
}
