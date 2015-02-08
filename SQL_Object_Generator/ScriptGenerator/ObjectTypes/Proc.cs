using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.ScriptGenerator.ObjectTypes
{
    public class Proc : ObjectType
    {
        public override string Name { get { return "procs"; } }

        public override string CountQuery
        {
            get
            {
                return @"
                    select count(*)
                    from sys.procedures";
            }
        }

        public override string DefinitionQuery
        {
            get
            {
                return @"
                    select a.name, b.definition, c.name as [schema]
                    from sys.procedures a
                        inner join sys.sql_modules b
                            on a.object_id = b.object_id
                        inner join sys.schemas c
                            on a.schema_id = c.schema_id";
            }
        }

        public override string FileBody
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(
                    "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{2}].[{0}]') AND type in (N'P', N'PC'))");
                sb.AppendLine("DROP PROCEDURE [{2}].[{0}]");
                sb.AppendLine("GO");
                sb.AppendLine("SET ANSI_NULLS ON");
                sb.AppendLine("GO");
                sb.AppendLine("SET QUOTED_IDENTIFIER ON");
                sb.AppendLine("GO");
                sb.Append("{1}");
                sb.AppendLine("GO");

                return sb.ToString();
            }
        }

        public override bool IncludePermissions { get { return true; } }
    }
}
