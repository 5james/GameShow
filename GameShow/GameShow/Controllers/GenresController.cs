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
    public class GenresController : Controller
    {
        private Storage storage = new Storage();
        public virtual Storage Storage
        {
            get
            {
                return storage;
            }
            set
            {
                storage = value;
            }
        }
        private int groupsPerPage = Int32.Parse(ConfigurationManager.AppSettings["GenresPerPage"]);

        // GET: PagedGroups
        public ActionResult Index(int? page, string errorMessage)
        {
            try
            {
                int currentPage = (page ?? 1);

                var genresList = storage.GetGenres();

                return View(new GenresDTO() { ErrorMessage = errorMessage, PagedGenres = genresList.ToPagedList(currentPage == 0 ? 1 : currentPage, groupsPerPage) });
            }
            catch (Exception ex)
            {
                return View(new GenresDTO() { ErrorMessage = ex.Message, PagedGenres = storage.GetGenres().ToPagedList(1, groupsPerPage) });
            }
        }
     

        // GET: PagedGroups/Details/5
        public ActionResult GenreForms(GenresDTO genresList, string request)
        {
            try
            {
                var genre = genresList.SelectedGenre;

                switch (request)
                {
                    case ("Create"):
                        storage.CreateGenre(genre);
                        break;
                    case ("Edit"):
                        storage.UpdateGenre(genre);
                        break;
                    case ("Delete"):
                        storage.DeleteGenre(genre);
                        break;
                    default:
                        throw new Exception("Akcja przycisku niezdefiniowana!");
                }
                return RedirectToAction("Index", new { page = genresList.Page });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { page = 1, errorMessage = ex.Message });
            }
        }
    }
}
