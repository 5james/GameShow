using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameShow.Models
{
    public class GenresDTO
    {
        public IPagedList<Genre> PagedGenres { get; internal set; }
        public Genre SelectedGenre { get; set; }
        public string ErrorMessage { get; set; }
        public int Page { get; set; }
    }
}