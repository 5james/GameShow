using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameShow.Models;
using System.Configuration;
using PagedList;

namespace GameShow.Controllers
{
    public class GamesController : Controller
    {
        private Storage storage = new Storage();
        public virtual Storage Storage
        {
            get { return storage; }
            set { storage = value; }
        }
        private int gamesPerPage = Int32.Parse(ConfigurationManager.AppSettings["GamesPerPage"]);
        ILogger logger = new Logger(typeof(GamesController));

        // GET: Games
        public ActionResult Index(int? page, string errorMessage, int[] genresFilter, string gamenameFilter, string request)
        {
            try
            {
                int currentPage = (page ?? 1);
                var gamesList = storage.GetGames();
                MultiSelectList genresList = new MultiSelectList(storage.GetGenres().ToList().OrderBy(i => i.GenreName), "IDGenre", "GenreName");

                if (!String.IsNullOrEmpty(request))
                {
                    if (request.Equals("Clear"))
                    {
                        genresFilter = new int[] { };
                        gamenameFilter = "";
                    }
                    else if (request.Equals("Filter"))
                    {
                        if (genresFilter == null)
                        {
                            genresFilter = new int[] { };
                        }
                        return FilterGames(genresFilter, gamenameFilter, currentPage);
                    }
                }

                return View(new GamesDTO { PagedGames = gamesList.ToPagedList(currentPage == 0 ? 1 : currentPage, gamesPerPage), Genres = genresList, ErrorMessage = errorMessage });
            }
            catch (Exception ex)
            {
                MultiSelectList genresList = new MultiSelectList(storage.GetGenres().ToList().OrderBy(i => i.GenreName), "IDGenre", "GenreName");
                return View(new GamesDTO { PagedGames = storage.GetGames().ToPagedList(1, gamesPerPage), Genres = genresList, ErrorMessage = ex.Message + errorMessage });
            }

        }

        public virtual ActionResult FilterGames(int[] genresFilter, string gamenameFilter, int currentPage)
        {

            try
            {
                List<Game> games = storage.GetGames();

                if (!String.IsNullOrEmpty(gamenameFilter))
                {
                    if (genresFilter.Count() > 0)
                    {
                        games = storage.GetGames(genresFilter, gamenameFilter);
                    }
                    else
                    {
                        games = storage.GetGames(gamenameFilter);
                    }
                }
                else
                {
                    if (genresFilter.Count() > 0)
                    {
                        games = storage.GetGames(genresFilter);
                    }
                }
                MultiSelectList genresList = new MultiSelectList(storage.GetGenres().ToList().OrderBy(i => i.GenreName), "IDGenre", "GenreName");
                return View(new GamesDTO { PagedGames = games.ToPagedList(currentPage == 0 ? 1 : currentPage, gamesPerPage), Genres = genresList });
            }
            catch (Exception ex)
            {

                MultiSelectList genresList = new MultiSelectList(storage.GetGenres().ToList().OrderBy(i => i.GenreName), "IDGenre", "GenreName");
                return View(new GamesDTO { PagedGames = storage.GetGames().ToPagedList(1, gamesPerPage), Genres = genresList, ErrorMessage = ex.Message });
            }



        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GameForms(GamesDTO gamesList, string request, int[] formGenres)
        {
            try
            {
                var game = gamesList.SelectedGame;

                switch (request)
                {
                    case ("Create"):
                        storage.CreateGame(game, formGenres);
                        break;
                    case ("Edit"):
                        storage.UpdateGame(game, formGenres);
                        break;
                    case ("Delete"):
                        storage.DeleteGame(game, formGenres);
                        break;
                    default:
                        throw new Exception("Akcja przycisku niezdefiniowana!");
                }
                return RedirectToAction("Index", new { page = gamesList.Page });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { page = 1, errorMessage = ex.Message });
            }
        }

        // GET: Games/Comments/5
        public ActionResult Comments(int? id, string errorMessage)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = storage.GetGames((int)id).First();
            if (game == null)
            {
                return HttpNotFound();
            }


            CommentsDTO model = new CommentsDTO { CommentedGame = game, ErrorMessage = errorMessage };
            return View(model);
        }


    }
}
