using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Kyklos.Kernel.Data.Test
{
    public static class ConnectionStringsProvider
    {
        private static readonly Dictionary<string, string> _connectionStringProviderList;

        static ConnectionStringsProvider()
        {
            var dict =
                JsonConvert.DeserializeObject<Dictionary<string, string>>
                (
                    File
                    .ReadAllText
                    (
                        "ConnString.txt"
                    )
                );

            _connectionStringProviderList = new Dictionary<string, string>(dict, StringComparer.InvariantCultureIgnoreCase);
        }

        public static string GetConnectionString(string providerName)
        {
            return _connectionStringProviderList[providerName];
        }
    }
}
