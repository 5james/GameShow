using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GameShow.Models
{
    public class Comment
    {
        [Key]
        public int IDComment { get; set; }

        [Required]
        [StringLength(32, ErrorMessage = "Nick może posiadać maksymalnie 32 znaki.")]
        [DisplayName("Nickname")]
        public string Nickname { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Adres Email jest wymagany!")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format email")]
        public string Email { get; set; }


        [Column(TypeName = "date")]
        [DisplayName("Data wystawienia")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime? PublishDate { get; set; }

        [StringLength(1024, ErrorMessage = "Komentarz może posiadać maksymalnie 32 znaki.")]
        [DisplayName("Komentarz")]
        [DataType(DataType.MultilineText)]
        public string CommentContent { get; set; }

        public int CommentedGameRefID { get; set; }

        [ForeignKey("CommentedGameRefID")]
        public virtual Game CommentedGame { get; set; }


        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] Stamp { get; set; }


        public override bool Equals(object obj)
        {
            Comment comment = obj as Comment;
            if (comment == null)
            {
                return false;
            }
            return Nickname == comment.Nickname &&
                Email == comment.Email &&
                CommentContent.Trim() == comment.CommentContent.Trim() &&
                CommentedGameRefID ==  comment.CommentedGameRefID &&
                PublishDate.Equals(PublishDate);
        }
    }
}