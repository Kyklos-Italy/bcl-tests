using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Film.OldFramework
{
    interface IActorDAL
    {
        Task<Actor[]> GetActors();
    }
}
