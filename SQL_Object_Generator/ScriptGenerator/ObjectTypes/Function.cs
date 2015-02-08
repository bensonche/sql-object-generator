using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.ScriptGenerator.ObjectTypes
{
    public class Function : ObjectType
    {
        public override string Name { get { return "functions"; } }

        public override string CountQuery
        {
            get
            {
                return @"
                    select count(*)
                    from sys.objects
                    where type in (N'FN', N'IF', N'TF', N'FS', N'FT')";
            }
        }

        public override string DefinitionQuery
        {
            get
            {
                return @"
                    select a.name, b.definition, c.name as [schema]
                    from sys.objects a
                        inner join sys.sql_modules b
                            on a.object_id = b.object_id
                        inner join sys.schemas c
                            on a.schema_id = c.schema_id
                    where a.type in (N'FN', N'IF', N'TF', N'FS', N'FT')";
            }
        }

        public override string FileBody
        {
            get
            {
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

                return sb.ToString();
            }
        }

        public override bool IncludePermissions { get { return true; } }
    }
}
