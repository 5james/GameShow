using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameShow.Models;

namespace GameShow.Controllers
{
    public class CommentsController : Controller
    {
        private Storage storage = new Storage();
        public virtual Storage Storage
        {
            get { return storage; }
            set { storage = value; }
        }
        ILogger logger = new Logger(typeof(GamesController));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CommentForms(CommentsDTO commentsList, string request)
        {
            try
            {
                var comment = commentsList.SelectedComment;

                switch (request)
                {
                    case ("Create"):
                        storage.CreateComment(comment);
                        break;
                    case ("Edit"):
                        storage.UpdateComment(comment);
                        break;
                    case ("Delete"):
                        storage.DeleteComment(comment);
                        break;
                    default:
                        throw new Exception("Akcja przycisku niezdefiniowana!");
                }
                return RedirectToAction("Comments", "Games", new { id = commentsList.SelectedComment.CommentedGameRefID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Comments", "Games", new { id = commentsList.SelectedComment.CommentedGameRefID, errorMessage = ex.Message });
            }
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int gameId)
        {
            try
            {
                storage.DeleteComment(id);
                return RedirectToAction("Comments", "Games", new { id = gameId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Comments", "Games", new { id = gameId, errorMessage = ex.Message });
            }
        }

        //private StorageContext db = new StorageContext();

        //// GET: Comments
        //public ActionResult Index()
        //{
        //    return View(db.Comments.ToList());
        //}

        //// GET: Comments/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Comment comment = db.Comments.Find(id);
        //    if (comment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(comment);
        //}

        //// GET: Comments/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Comments/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "IDComment,Nickname,Email,PublishDate,CommentContent,Stamp")] Comment comment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Comments.Add(comment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(comment);
        //}

        //// GET: Comments/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Comment comment = db.Comments.Find(id);
        //    if (comment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(comment);
        //}

        //// POST: Comments/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "IDComment,Nickname,Email,PublishDate,CommentContent,Stamp")] Comment comment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(comment).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(comment);
        //}



        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
