using System;
using System.Collections.Generic;
using System.Text;

namespace KMicro.Auth.Tests.TestUsers
{
    public class AdminUser
    {
        public AdminUser(string username, string password)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public string Username { get; }
        public string Password { get; }
    }
}
