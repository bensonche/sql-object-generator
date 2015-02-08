using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.ScriptGenerator.Model
{
    public class DbObjectResult
    {
        public string Name;
        public string Definition;
        public string Schema;

        public string PermissionType;
        public string PermissionName;
        public string GranteeName;
    }
}
