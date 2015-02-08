using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace BC.ScriptGenerator
{
    abstract public class ObjectType
    {
        abstract public string Name { get; }
        abstract public string CountQuery { get; }
        abstract public string DefinitionQuery { get; }
        abstract public string FileBody { get; }

        abstract public bool IncludePermissions { get; }

        public int Count;

        public string Remaining
        {
            get
            {
                return Count < 0 ? "Done" : Count.ToString();
            }
        }

        public int SetInitialCount(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = CountQuery;
            cmd.CommandType = CommandType.Text;

            Count = (int)cmd.ExecuteScalar();

            return Count;
        }
    }
}
