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
        public static Dictionary<string, User> Users { get; }

        private static void AddUser(User user)
        {
            Users.Add(user.Username, user);
        }

        public const string Aurelia = "aurelia";
        public const string Azzurra = "azzurra";
        public const string Barbiero = "barbiero";
        public const string Barotti = "barotti";
        public const string Bello = "bello";
        public const string Bianco = "bianco";
        public const string Camelot = "camelot";
        public const string Carta = "carta";
        public const string Casagrande = "casagrande";
        public const string Castelli = "castelli";

        static MultipleUsers()
        {
            Users = new Dictionary<string, User>();
            AddUser(new User(Aurelia, "aurelia", "Moncler", "ScmX"));
            AddUser(new User(Azzurra, "ITVAZ0", "Moncler", "ScmX"));
            AddUser(new User(Barbiero, "barbiero", "Moncler", "ScmX"));
            AddUser(new User(Barotti, "12345678", "Moncler", "ScmX"));
            AddUser(new User(Bello, "WKOBE0", "Moncler", "ScmX"));
            AddUser(new User(Bianco, "bianco", "Moncler", "ScmX"));
            AddUser(new User(Camelot, "camelot", "Moncler", "ScmX"));
            AddUser(new User(Carta, "12345678", "Moncler", "ScmX"));
            AddUser(new User(Casagrande, "IPGCM0", "Moncler", "ScmX"));
            AddUser(new User(Castelli, "castelli", "Moncler", "ScmX"));
        }
    }
}