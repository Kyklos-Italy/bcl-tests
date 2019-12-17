
using System.Collections.Generic;

namespace KMicro.Auth.Tests.TestUsers
{
    public class User
    {
        public string Username { get; }
        public string Password { get; }
        public string Domain { get; }
        public string Application { get; }

        public User(string usr, string pwd, string domain, string app)
        {
            Username = usr;
            Password = pwd;
            Domain = domain;
            Application = app;
        }
    }

    public class IncorrectData
    {
        public const string Username = "MadeUpUserName";
        public const string Password = "MadeUpPassword";
        public const string Domain = "MadeUpUserName";
        public const string Application = "MadeUpAppliation";
    }

    public class NeverExpiresUser
    {
        public const string Username = "neverxpuser";
        public const string Password = "Maialino818!";
        public const string Domain = "KyklosTest";
        public const string Application = "ScmX";
    }

    public class NeverLocksUser
    {
        public const string Username = "neverlocks";
        public const string Password = "Porcellino313!";
        public const string Domain = "MonclerTest";
        public const string Application = "QualityX";
    }

    public class PasswordExpiredUser
    {
        public const string Username = "pwdexpireduser";
        public const string Password = "Prosciutto313!";
        public const string Domain = "KyklosTest";
        public const string Application = "QualityX";
    }

    public class AllowOldPasswordsUser
    {
        public const string Username = "neverxpuser";
        public const string Password = "Maialino818!";
        public const string Domain = "AllowOldPwdsDomain";
        public const string Application = "ScmX";
    }

    public class MultipleUsers
    {
        public static Dictionary<string, User> users = new Dictionary<string, User>
        {
            {"aurelia", new User("aurelia", "aurelia", "Moncler", "ScmX")},
            {"azzurra", new User("azzurra", "ITVAZ0", "Moncler", "SCMX")},
            {"barbiero", new User("barbiero", "barbiero", "Moncler", "SCMX")},
            {"barotti", new User("barotti", "12345678", "Moncler", "SCMX")},
            {"bello", new User("bello", "WKOBE0", "Moncler", "SCMX")},
            {"bianco", new User("bianco", "bianco", "Moncler", "SCMX")},
            {"camelot", new User("camelot", "camelot", "Moncler", "SCMX")},
            {"carta", new User("carta", "12345678", "Moncler", "SCMX")},
            {"casagrande", new User("casagrande", "IPGCM0", "Moncler", "SCMX")},
            {"castelli", new User("castelli", "castelli", "Moncler", "SCMX")}
        };
    }
}

