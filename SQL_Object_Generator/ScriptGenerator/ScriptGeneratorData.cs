using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.ScriptGenerator.Model;
using System.Data;
using System.Data.SqlClient;

namespace BC.ScriptGenerator
{
    public class ScriptGeneratorData
    {
        public string ServerName;
        public string DbName;
        public string Username;
        public string Password;

        public bool Integrated;

        public string ConnectionString
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

        public async Task VerifyConnectionAsync()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                await con.OpenAsync();

                if (con.State != ConnectionState.Open)
                    throw new Exception("Cannot open database");
            }
        }

        public List<DbObjectResult> GetDbOject(string query)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand
                {
                    Connection = con,
                    CommandType = CommandType.Text,
                    CommandText = query
                };

                adapter.SelectCommand = cmd;

                var table = new DataTable();

                adapter.Fill(table);

                return (from t in table.AsEnumerable()
                        select new DbObjectResult
                        {
                            name = t.Field<string>("name"),
                            definition = t.Field<string>("definition"),
                            schema = t.Field<string>("schema"),
                        }).ToList<DbObjectResult>();

            }
        }

        private int GetScalar(SqlConnection con, string commandText)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.Text;

            return (int)cmd.ExecuteScalar();
        }
    }
}
