using NuGet;

namespace ProjectAdj
{
    internal class CacheResult
    {
        public IPackage Package { get; }
        public bool AlreadyInstalled { get; set; }

        public CacheResult(IPackage package, bool alreadyInstalled)
        {
            Package = package;
            AlreadyInstalled = alreadyInstalled;
        }
    }
}
