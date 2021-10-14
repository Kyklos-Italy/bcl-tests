using Common.Logging;
using FilmOldPattern.DAL;
using FilmOldPattern.Model;
using Kyklos.Kernel.SpringSupport.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmOldPattern
{
    public class Utility
    {
        private static readonly IActorDAL actorDal = Instantiator.GetObject<IActorDAL>("ActorDAL");
        private static readonly IFilmDAL filmDal = Instantiator.GetObject<IFilmDAL>("FilmDAL");
        private static readonly ICastDAL castDal = Instantiator.GetObject<ICastDAL>("CastDAL");

        private static ILog logger = LogManager.GetLogger("ERROR");


        public static int GetNextId(string entityType)
        {
            int index = 0;
            try
            {
                switch (entityType)
                {
                    case "Actor":
                        index = actorDal.GetNextId();
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error", ex);
            }

            return index;
        }

        public static int Count(string type)
        {
            try
            {
                int count = 0;

                switch (type)
                {
                    case "FILM":
                        count = filmDal.GetCountFilm();
                        break;

                    case "Actor":
                        count = actorDal.GetCountActors();
                        break;

                    default:
                        break;
                }

                return count;
            }
            catch (Exception ex)
            {
                logger.Error("Error", ex);
            }

            return 0;
        }

        public static List<Film> GetFilms()
        {
            try
            {
                return filmDal.GetFilms().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("Error", ex);
            }

            return null;
        }

        public static List<Actor> GetActors()
        {
            try
            {
                return actorDal.GetActors().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("Error", ex);
            }

            return null;
        }

        public static List<Cast> GetCasts()
        {
            try
            {
                return castDal.GetCasts().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("Error", ex);
            }

            return null;
        }

        public static void PrintTable(string entityType)
        {
            try
            {
                switch (entityType)
                {
                    case "FILMS":
                        filmDal.ViewTable();
                        break;

                    case "ACTORS":
                        actorDal.ViewTable();
                        break;

                    case "CASTS":
                        castDal.ViewTable();
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error", ex);
            }
        }

        public static void Insert(string entityType, Film film = null, Actor actor = null, Cast cast = null)
        {
            try
            {
                switch (entityType)
                {
                    case "FILM":
                        filmDal.InsertFilm(film);
                        break;

                    case "ACTOR":
                        actorDal.InsertActor(actor);
                        break;

                    case "CAST":
                        castDal.InsertCast(cast);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error", ex);
            }
        }

        public static void Delete(string entityType, Film film = null, Actor actor = null, Cast cast = null)
        {
            try
            {
                switch (entityType)
                {
                    case "FILM":
                        filmDal.DeleteFilm(film);
                        break;

                    case "ACTOR":
                        actorDal.DeleteActor(actor);
                        break;

                    case "CAST":
                        castDal.DeleteCast(cast);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error", ex);
            }
        }



        public static void DeleteByParameter(string entityType, string parameter)
        {
            try
            {
                switch (entityType)
                {
                    case "FILM":
                        filmDal.DeleteFilmByFilmTitle(parameter);
                        break;

                    case "ACTOR":
                        actorDal.DeleteActorByActorName(parameter);
                        break;
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error", ex);
            }
        }

        public static DateTime GetDatebyFilmTitle(string filmTitle)
        {
            return filmDal.GetDateFilmByTitle(filmTitle);
        }


        public static List<string> GetFilmsTitleByActorId(int actorId)
        {
            return filmDal.FilmTitleWhereWasPartecipateSpecificActor(actorId).ToList();
        }

        public static Tuple<IList<Film>, IList<Actor>, IList<Cast>> GetEntities() 
        {
            return filmDal.GetTuple();
        }
    }
}
