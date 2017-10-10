using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameShow.Models
{
    public class GamesDTO
    {
        //public virtual List<Game> Games { get; set; }
        public virtual IPagedList<Game> PagedGames { get; internal set; }
        public MultiSelectList Genres { get; set; }
        public Game SelectedGame { get; set; }
        public string ErrorMessage { get; set; }
        public int Page { get; set; }
        public string GameNameFilter { get; set; }
        public int[] GenresListFilter { get; set; }
    }
}