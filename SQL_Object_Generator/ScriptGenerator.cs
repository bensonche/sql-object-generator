using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Object_Generator
{
    public class ScriptGenerator
    {
        public string serverName;
        public string dbName;
        public string username;
        public string password;

        public bool integrated;

        private string connectionString
        {
            get
            {
                if (integrated)
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

        public async Task GenerateAsync()
        {
            SqlConnection con = new SqlConnection(connectionString);

            await con.OpenAsync();

            if (con.State != ConnectionState.Open)
                throw new Exception("Cannot open database");

            con.Close();
        }
    }
}
