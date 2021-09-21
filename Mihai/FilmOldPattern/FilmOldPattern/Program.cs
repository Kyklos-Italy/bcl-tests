using Kyklos.Kernel.SpringSupport.Core;
using Kyklos.Kernel.SpringSupport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmOldPattern
{
    class Program
    {
        private static IDaoHelper DaoHelper => Instantiator.GetObject<IDaoHelper>("KyklosKernelDaoHelper");
        static void Main(string[] args)
        {
            
        }
    }
}
