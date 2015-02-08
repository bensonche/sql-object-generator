using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using BC.ScriptGenerator.ObjectTypes;

namespace BC.ScriptGenerator
{
    public class ScriptGenerator
    {
        public string OutputDir;

        public List<ObjectType> ObjectList = new List<ObjectType>();

        private string ConnectionString { get { return data.ConnectionString; } }

        private ScriptGeneratorData data;

        public ScriptGenerator( string serverName, string dbName, string username, string password, bool integrated ,string outputDir)
        {
            OutputDir = outputDir;

            data = new ScriptGeneratorData
            {
                ServerName = serverName,
                DbName = dbName,
                Username = username,
                Password = password,
                Integrated = integrated
            };

            ObjectList.Add(new Trigger());
            ObjectList.Add(new Function());
            ObjectList.Add(new Proc());
        }

        public async Task GenerateAsync()
        {
            await data.VerifyConnectionAsync();

            SetCounts();

            List<Task> taskList = new List<Task>();

            foreach (var type in ObjectList)
            {
                taskList.Add(Task.Run(() => GenerateObjectScript(type)));
            }

            foreach (var task in taskList)
                await task;
        }

        private void SetCounts()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                foreach (var type in ObjectList)
                    type.SetInitialCount(con);
            }
        }

        private async Task GenerateObjectScript(ObjectType type)
        {
            var definitionTask = data.GetDbOjectAsync(type);

            DirectoryInfo d = new DirectoryInfo(Path.Combine(OutputDir, type.Name));

            await Task.Run(() =>
            {
                if (!d.Exists)
                    d.Create();

                foreach (var f in d.GetFiles())
                    f.Delete();
            });

            var result = await definitionTask;

            foreach (var obj in result)
            {
                type.Count--;

                string name = obj.name;
                string definition = obj.definition;
                string schema = obj.schema;

                string filename = string.Format("{0}.{1}.sql", schema, name);

                FileInfo f = new FileInfo(Path.Combine(d.FullName, filename));

                using (Stream s = f.Create())
                using (StreamWriter w = new StreamWriter(s))
                {
                    w.Write(type.FileBody, name, definition, schema);

                    if (type.IncludePermissions)
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

            type.Count = -1;
        }

        private string GetPermissionsString(string schema, string name)
        {
            string commandText = string.Format(@"
                select state_desc, permission_name, name
                from sys.database_permissions a
                    left join sys.database_principals b
                        on a.grantee_principal_id = b.principal_id
                where a.major_id = object_id('[{0}].[{1}]')
                order by state_desc, permission_name, name", schema, name);

            StringBuilder sb = new StringBuilder();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand
                {
                    Connection = con,
                    CommandType = CommandType.Text,
                    CommandText = commandText
                };

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
