using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kyklos.Kernel.Data.Test
{
    public static class ConnectionStringsProvider
    {
        private static readonly Dictionary<string, string> _connectionStringProviderList;

        static ConnectionStringsProvider()
        {
            _connectionStringProviderList =
                JsonConvert.DeserializeObject<Dictionary<string, string>>
                (
                    File
                    .ReadAllText
                    (
                        "C:\\Development\\dotnet\\bcl-tests\\Kyklos.Kernel.BCL.Test\\Kyklos.Kernel.Data.Test\\ConnString.txt"
                    )
                );
        }

        public static string  GetConnectionStringProviderList(string ProviderName)
        {
            return _connectionStringProviderList[ProviderName];
        }
    }
}
