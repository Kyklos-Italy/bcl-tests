using Kyklos.Kernel.Data.Async;
using Kyklos.Kernel.Data.Async.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Kyklos.Kernel.Core.KLinq;

namespace Film.OldFramework
{
    public class ActorDAL : BaseDAL, IActorDAL
    {
        public ActorDAL(IAsyncDao dao)
           : base(dao)
        {
        }

        public async Task<Actor[]> GetActors()
        {
            const string aliasActor = "actor";

            var builder =
                 _dao
                  .NewQueryBuilder()
                  .Select()
                  .AllFields<Actor>(aliasActor)
                  .From<Actor>(aliasActor);


            Actor[] result = await _dao.GetItemsArrayAsync<Actor>(builder);

            return result.ToEmptyArrayIfNull();
        }
    }
}
