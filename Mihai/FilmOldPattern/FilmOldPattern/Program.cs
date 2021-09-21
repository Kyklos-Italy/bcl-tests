using Common.Logging;
using FilmOldPattern.DAL;
using Kyklos.Kernel.SpringSupport.Core;
using System;

namespace FilmOldPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                CustomAdditionalDbProvider.SetDefaultAdditionalResourceForCustomDbProvidersSqlServer();
                IFilmDAL filmDal = Instantiator.GetObject<IFilmDAL>("FilmDAL");
                var films = filmDal.GetFilms();
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetLogger("DEBUG");
                logger.Error("Error", ex);
            }            
        }
    }
}
