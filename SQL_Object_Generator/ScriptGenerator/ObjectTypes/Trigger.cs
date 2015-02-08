using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.ScriptGenerator.ObjectTypes
{
    public class Trigger : ObjectType
    {
        public override string Name { get { return "triggers"; } }

        public override string CountQuery
        {
            get
            {
                return @"
                    select count(*)
                    from sys.triggers";
            }
        }

        public override string DefinitionQuery
        {
            get
            {
                return @"
                    select
                        a.name,
                        b.definition,
                        c.name as [schema],
                        null as PermissionType,
                        null as PermissionName,
                        null as GranteeName
                    from sys.triggers a
                        inner join sys.sql_modules b
                            on a.object_id = b.object_id
                        inner join sys.tables c
                            on a.parent_id = c.object_id
                        inner join sys.schemas d
                            on c.schema_id = d.schema_id";
            }
        }

        public override string FileBody
        {
            get
            {
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

                return sb.ToString();
            }
        }
    }
}
