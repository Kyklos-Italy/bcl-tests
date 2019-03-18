using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypeFollower
{
	[Serializable]
    public class AssemblyObject
    {
        public string AssemblyName { get; set; }

        public string TypeName { get; set; }
        public string TypeNamespace { get; set; }

        public string MethodName { get; set; }
    }
}
