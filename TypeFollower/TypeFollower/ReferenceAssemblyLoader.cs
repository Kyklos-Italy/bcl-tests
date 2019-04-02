using System;
using System.IO;
using System.Reflection;
using TypeFollower.Properties;

namespace TypeFollower
{
    public static class ReferenceAssemblyLoader
    {
		public static string SourceAssemblyFolder { get; set; }
		public static string TargetAssemblyFolder { get; set; }

		public static Assembly ResolveAssemblyEventHandler(object sender, ResolveEventArgs args)
        {
            AppDomain domain = sender as AppDomain;
            string defaultPath = domain?.SetupInformation?.ApplicationName ?? string.Empty;
            return ResolveAssembly(args.Name, defaultPath);
        }

        public static Assembly ResolveAssembly(string assemblyFullName, string defaultSearchPath)
        {
            Console.WriteLine($"Resolving referenced assembly {assemblyFullName} ....");
            string[] assemblyInfo = assemblyFullName.Split(",".ToCharArray());
            string assemblyName = $"{assemblyInfo[0].Trim()}";

            string assPath = Path.Combine(defaultSearchPath ?? string.Empty, $"{assemblyName}.dll");
            if (File.Exists(assPath))
            {
                return AssemblyLoad(assPath);
            }

			assPath = Path.Combine(SourceAssemblyFolder ?? string.Empty, $"{assemblyName}.dll");
			if (File.Exists(assPath))
			{
				return AssemblyLoad(assPath);
			}

			assPath = Path.Combine(TargetAssemblyFolder ?? string.Empty, $"{assemblyName}.dll");
			if (File.Exists(assPath))
			{
				return AssemblyLoad(assPath);
			}

			assPath = Path.Combine(Settings.Default.ReferenceAssemblyPoolPath, assemblyName, assemblyInfo[1].Replace("Version=", "").Trim(), $"{assemblyName}.dll");
            if (File.Exists(assPath))
            {
                return AssemblyLoad(assPath);
            }

            assPath = Path.Combine(Settings.Default.ReferenceAssemblyPoolPath, $"{assemblyName}.dll");
            if (File.Exists(assPath))
            {
                return AssemblyLoad(assPath);
            }

            Console.WriteLine($"Failed!");
            return null;
        }

        public static Assembly AssemblyLoad(string path)
        {
            MachineType mt = GetMachineType(path);
            Console.WriteLine($"Loading {mt.ToString()} assembly from {path} ....");
            
            return Assembly.LoadFrom(path);
        }

        public static MachineType GetMachineType(string fileName)
        {
            const int PE_POINTER_OFFSET = 60;
            const int MACHINE_OFFSET = 4;
            byte[] data = new byte[4096];
            using (Stream s = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                s.Read(data, 0, 4096);
            }
            // dos header is 64 bytes, last element, long (4 bytes) is the address of the PE header
            int PE_HEADER_ADDR = BitConverter.ToInt32(data, PE_POINTER_OFFSET);
            int machineUint = BitConverter.ToUInt16(data, PE_HEADER_ADDR + MACHINE_OFFSET);
            return (MachineType)machineUint;
        }
    }

    public enum MachineType
    {
        Native = 0, I386 = 0x014c, Itanium = 0x0200, x64 = 0x8664
    }
}
