using System.Threading.Tasks;
using Kyklos.Kernel.Data.Test;
using XUnitTestSupport;

namespace Kyklos.Kernel.Data.MariaDB.Test.NetCore
{
    public abstract class MariaDBDatasyncTestCommon : BaseDatasyncTest
    {
        private static object lockObj = new object();
        private static bool isInitialized = false;

        protected override string Schema => "testdb";
        protected override string ProviderName => "MariaDB";

        protected MariaDBDatasyncTestCommon() : base(NetPlatformType.NETCORE)
        {
            if (!isInitialized)
            {
                lock (lockObj)
                {
                    if (!isInitialized)
                    {
                        SetupCoreAsync().Wait();
                        isInitialized = true;
                    }
                }
            }            
        }

        //private async Task SetupCore()
        //{
        //    await PrepareDB();
        //    await GenerateScriptsForCreateAndDropSequence().ConfigureAwait(false);
        //    await AddTeams();
        //    await AddDays();
        //    await AddResults();
        //    await AddMembers().ConfigureAwait(false);
        //    await AddJobs().ConfigureAwait(false);
        //    await AddReasons().ConfigureAwait(false);
        //    await AddJobTimes().ConfigureAwait(false);
        //}

        protected override async Task GenerateScriptsForCreateAndDropSequence()
        {
            string createSequenceScript = @"CREATE SEQUENCE my_sequence
                                           INCREMENT BY 1
                                           MINVALUE 1 MAXVALUE 1000
                                           START WITH 1";

            string dropSequenceScript = "DROP SEQUENCE my_sequence";

            await PrepareSequence(createSequenceScript, dropSequenceScript).ConfigureAwait(false);
        }
    }
}
