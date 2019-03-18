using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAdj
{
    public class CompareTypeResult
    {
        public string OriginalAssemblyName { get; set; }
        public string OriginalNamespace { get; set; }
        public string OriginalType { get; set; }
        public string NewAssemblyName { get; set; }
        public string NewNamespace { get; set; }
        public string NewType { get; set; }

        public bool IsChanged()
        {
            return !string.IsNullOrEmpty(NewType) &&
                (OriginalAssemblyName != NewAssemblyName || OriginalNamespace != NewNamespace || OriginalType != NewType);
        }
    }
}
