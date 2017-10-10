using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameShow.Models
{
    public class Genre
    {
        public Genre()
        {
            Games = new List<Game>();
        }

        [Key]
        public int IDGenre { get; set; }

        [Required]
        [StringLength(32, ErrorMessage = "Gateunek gry ma maksymalnie 32 znaki.")]
        [DisplayName("Gantunek gry")]
        public string GenreName { get; set; }

        [ForeignKey("IDGame")]
        public virtual ICollection<Game> Games { get; set; }

        [MaxLength(8)]
        [Timestamp]
        public byte[] Stamp { get; set; }

        public override string ToString()
        {
            return GenreName;
        }

        public override bool Equals(object obj)
        {
            Genre genre = obj as Genre;
            if (genre == null)
            {
                return false;
            }
            return GenreName == genre.GenreName &&
                IDGenre == genre.IDGenre;
        }
    }
}