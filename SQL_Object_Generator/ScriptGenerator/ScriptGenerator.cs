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

            var definitionList = await definitionTask;

            foreach (var definition in definitionList)
            {
                type.Count--;

                string filename = string.Format("{0}.{1}.sql", definition.Schema, definition.Name);

                FileInfo f = new FileInfo(Path.Combine(d.FullName, filename));

                using (Stream s = f.Create())
                using (StreamWriter w = new StreamWriter(s))
                {
                    w.Write(type.FileBody, definition.Name, definition.Definition, definition.Schema);

                    if (definition.PermissionString != null)
                    {
                        w.Write(definition.PermissionString);
                        w.Write("GO");
                    }
                }
            }

            type.Count = -1;
        }
    }
}
