﻿using System;
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

            con.Close();

            Task triggers = GetTriggers(OutputDir, con);
            Task functions = GetFunctions(OutputDir, con);

            await triggers;
            await functions;

        }

        private async Task GetFunctions(string outputDir, SqlConnection con)
        {
            string commandText = @"
                select a.name, b.definition, c.name as [schema]
                from sys.objects a
                    inner join sys.sql_modules b
                        on a.object_id = b.object_id
                    inner join sys.schemas c
                        on a.schema_id = c.schema_id
                where a.type in (N'FN', N'IF', N'TF', N'FS', N'FT')";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(
                "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{2}].[{0}]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))");
            sb.AppendLine("DROP FUNCTION [{2}].[{0}]");
            sb.AppendLine("GO");
            sb.AppendLine("SET ANSI_NULLS ON");
            sb.AppendLine("GO");
            sb.AppendLine("SET QUOTED_IDENTIFIER ON");
            sb.AppendLine("GO");
            sb.Append("{1}");
            sb.AppendLine("GO");

            GenerateObjectScript(outputDir, "functions", commandText, sb.ToString(), true);
        }

        private async Task GetTriggers(string outputDir, SqlConnection con)
        {
            string commandText = @"
                select a.name, b.definition, d.name as [schema]
                from sys.triggers a
                    inner join sys.sql_modules b
                        on a.object_id = b.object_id
                    inner join sys.tables c
                        on a.parent_id = c.object_id
                    inner join sys.schemas d
                        on c.schema_id = d.schema_id";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[{2}].[{0}]'))");
            sb.AppendLine("DROP TRIGGER [{2}].[{0}]");
            sb.AppendLine("GO");
            sb.AppendLine("SET ANSI_NULLS ON");
            sb.AppendLine("GO");
            sb.AppendLine("SET QUOTED_IDENTIFIER ON");
            sb.AppendLine("GO");
            sb.Append("{1}");
            sb.AppendLine("GO");

            GenerateObjectScript(outputDir, "triggers", commandText, sb.ToString());
        }

        private void GenerateObjectScript(string outputDir, string description, string commandText,
            string body, bool includePermissions = false)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

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
                    string schema = reader["schema"].ToString();

                    string filename = string.Format("{0}.{1}.sql", schema, name);

                    FileInfo f = new FileInfo(Path.Combine(d.FullName, filename));

                    using (Stream s = f.Create())
                    {
                        using (StreamWriter w = new StreamWriter(s))
                        {
                            w.Write(body, name, definition, schema);

                            if (includePermissions)
                            {
                                string permissions = GetPermissionsString(schema, name);
                                if (!string.IsNullOrWhiteSpace(permissions))
                                {
                                    w.Write(permissions);
                                    w.Write("GO");
                                }
                            }
                        }
                    }
                }
            }
        }

        private string GetPermissionsString(string schema, string name)
        {
            string commandText = @"
                select state_desc, permission_name, name
                from sys.database_permissions a
                    left join sys.database_principals b
                        on a.grantee_principal_id = b.principal_id
                where a.major_id = object_id('[dbo].[allusers]')
                order by state_desc, permission_name, name";

            StringBuilder sb = new StringBuilder();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = commandText;

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    sb.AppendLine(string.Format("{0} {1} on [{2}].[{3}] to [{4}]",
                        reader["state_desc"],
                        reader["permission_name"],
                        schema,
                        name,
                        reader["name"]));
                }
            }

            return sb.ToString();
        }
    }
}
