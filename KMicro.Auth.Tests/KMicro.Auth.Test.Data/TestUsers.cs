
namespace KMicro.Auth.Tests.TestUsers
{
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

}
