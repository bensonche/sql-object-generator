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

        public async Task<List<DbObjectResult>> GetDbOjectAsync(ObjectType type)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter())
            {
                await con.OpenAsync();

                SqlCommand cmd = new SqlCommand
                {
                    Connection = con,
                    CommandType = CommandType.Text,
                    CommandText = type.DefinitionQuery
                };

                adapter.SelectCommand = cmd;

                var table = new DataTable();

                adapter.Fill(table);

                var raw = from t in table.AsEnumerable()
                          select new
                          {
                              Name = t.Field<string>("name"),
                              Definition = t.Field<string>("definition"),
                              Schema = t.Field<string>("schema"),
                              PermissionType = t.Field<string>("PermissionType"),
                              PermissionName = t.Field<string>("PermissionName"),
                              GranteeName = t.Field<string>("GranteeName")
                          };

                var grouped = from r in raw
                              group r by new { r.Name, r.Definition, r.Schema } into g
                              select g;

                List<DbObjectResult> resultList = new List<DbObjectResult>();

                foreach (var obj in grouped)
                {
                    DbObjectResult result = new DbObjectResult();
                    result.Name = obj.Key.Name;
                    result.Definition = obj.Key.Definition;
                    result.Schema = obj.Key.Schema;

                    bool hasPermissions = true;
                    StringBuilder sb = new StringBuilder();
                    foreach (var permission in obj)
                    {
                        if (permission.PermissionType == null)
                        {
                            hasPermissions = false;
                            break;
                        }

                        sb.AppendLine(string.Format("{0} {1} on [{2}].[{3}] to [{4}]",
                            permission.PermissionType,
                            permission.PermissionName,
                            obj.Key.Schema,
                            obj.Key.Name,
                            permission.Name));
                    }
                    sb.AppendLine("GO");

                    result.PermissionString = hasPermissions ? sb.ToString() : null;

                    resultList.Add(result);
                }

                return resultList;
            }
        }
    }
}
