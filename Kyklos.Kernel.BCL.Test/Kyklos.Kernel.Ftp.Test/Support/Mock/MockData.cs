using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kyklos.Kernel.Ftp.Test.Support.Mock
{
    internal class MockData : Framework
    {
        public string RebexFolder { get; }
        public string RebexDataFolder { get; }
        public string KftpServerFolder { get; }
        public string KftpServerDataFolder { get; }
        public string KftpsServerFolder { get; }
        public string KftpsServerDataFolder { get; }
        public string FtpDataFolder { get; }
        public string FtpsDataFolder { get; }
        public string SftpDataFolder { get; }
        public string HostName { get; } = "192.168.0.109";
        public string Username { get; } = "tester";
        public string Password { get; } = "password";

        public MockData(FrameworkType frameworkType) : base(frameworkType)
        {
            DirectoryInfo BinFolderDirectoryInfo = new DirectoryInfo(BinFolder);
            string TestBaseFolder = (frameworkType == FrameworkType.NETCORE) 
                ? 
                BinFolderDirectoryInfo.Parent.Parent.Parent.Parent.FullName + "\\Kyklos.Kernel.Ftp.Test" 
                :
                BinFolderDirectoryInfo.Parent.Parent.Parent.FullName + "\\Kyklos.Kernel.Ftp.Test"
            ;
            KftpServerFolder = TestBaseFolder + "\\KftpServer";
            KftpServerDataFolder = KftpServerFolder + "\\data";
            KftpsServerFolder = TestBaseFolder + "\\KftpsServer";
            KftpsServerDataFolder = KftpsServerFolder + "\\data";
            RebexFolder = TestBaseFolder + "\\Rebex";
            RebexDataFolder = RebexFolder + "\\data";
            FtpDataFolder = TestBaseFolder + "\\Ftp\\data";
            FtpsDataFolder = TestBaseFolder + "\\Ftps\\data";
            SftpDataFolder = TestBaseFolder + "\\Sftp\\data";
        }
    }
}
