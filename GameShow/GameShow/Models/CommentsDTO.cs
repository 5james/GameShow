using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameShow.Models
{
    public class CommentsDTO
    {
        public string ErrorMessage { get; set; }
        public Comment SelectedComment { get; set; }
        public Game CommentedGame { get; set; }
    }
}