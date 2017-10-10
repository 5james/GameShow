using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameShow.Models
{
    public class Game
    {
        public Game()
        {
            GenreList = new List<Genre>();
            CommentsList = new List<Comment>();
        }

        [Key]
        public int IDGame { get; set; }

        [Required]
        [StringLength(32, ErrorMessage = "Tytuł gry ma maksymalnie 32 znaki.")]
        [DisplayName("Tytuł gry")]
        public string GameName { get; set; }
        
        [StringLength(32, ErrorMessage = "Wydawca gry ma maksymalnie 32 znaki.")]
        [DisplayName("Wydawca")]
        public string Publisher { get; set; }


        [Column(TypeName = "date")]
        [DisplayName("Rok wydania")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PublishDate { get; set; }

        [StringLength(32, ErrorMessage = "Kraj pochodzenia gry ma maksymalnie 32 znaki.")]
        [DisplayName("Kraj")]
        public string Country { get; set; }

        [ForeignKey("IDGenre")]
        [DisplayName("Gatunek")]
        public virtual ICollection<Genre> GenreList { get; set; }

        [ForeignKey("IDComment")]
        [DisplayName("Komentarze")]
        public virtual ICollection<Comment> CommentsList { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] Stamp { get; set; }

        public override bool Equals(object obj)
        {
            Game game = obj as Game;
            if (game == null)
            {
                return false;
            }
            bool b = GameName == game.GameName &&
                Publisher == game.Publisher &&
                Country == game.Country &&
                PublishDate.Equals(game.PublishDate);
            if (game.GenreList == null || !GenreList.Count.Equals(game.GenreList.Count))
                return false;
            if (b)
            {
                List<Genre> list1 = new List<Genre>(GenreList.OrderBy(i => i.IDGenre));
                List<Genre> list2 = new List<Genre>(game.GenreList.OrderBy(i => i.IDGenre));
                for (int i = 0; i < list1.Count(); i++)
                {
                    b = list1[i].IDGenre.Equals(list2[i].IDGenre);
                }
            }
            else
            {
                return false;
            }
            return b;
        }

    }
}