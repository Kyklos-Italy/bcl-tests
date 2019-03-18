using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypeFollower
{
    public class CompareMethodResult : CompareTypeResult
    {
        public string OriginalMethodName { get; set; }
        public string NewMethodName { get; set; }
    }
}
