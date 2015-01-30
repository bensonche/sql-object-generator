using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Script_Generator
{
    internal class Program
    {
        private const string username = "";
        private const string password = "";

        private const string serverName = "";
        private const string dbName = "";

        private const string outputDir = @"c:\temp\repeatable\triggers";

        private static string connectionString
        {
            get
            {
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

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
                select a.name, b.definition
                from sys.triggers a
                    inner join sys.sql_modules b
                        on a.object_id = b.object_id";

            var reader = cmd.ExecuteReader();

            DirectoryInfo d = new DirectoryInfo(outputDir);

            if (!d.Exists)
                d.Create();

            foreach(var f in d.GetFiles())
                f.Delete();

            while (reader.Read())
            {
                FileInfo f = new FileInfo(Path.Combine(d.FullName, string.Format("{0}.sql", reader["name"])));

                using (Stream s = f.Create())
                {
                    using (StreamWriter w = new StreamWriter(s))
                    {
                        w.Write(@"IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[{0}]'))
DROP TRIGGER [dbo].[{0}]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
{1}
GO", reader["name"], reader["definition"]);
                    }
                }
            }

            con.Close();
        }
    }
}
