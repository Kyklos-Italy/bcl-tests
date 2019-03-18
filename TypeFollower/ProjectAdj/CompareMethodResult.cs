using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAdj
{
    public class CompareMethodResult : CompareTypeResult
    {
        public string OriginalMethodName { get; set; }
        public string NewMethodName { get; set; }

        public new bool IsChanged()
        {
            return !string.IsNullOrEmpty(NewMethodName) && OriginalMethodName != NewMethodName || base.IsChanged();
        }
    }
}
