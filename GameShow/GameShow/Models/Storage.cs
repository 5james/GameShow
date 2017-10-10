using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace GameShow.Models
{
    public class Storage
    {
        ILogger logger = new Logger(typeof(Storage));
        public Storage()
        {
            Database.SetInitializer<StorageContext>(null);
        }

        #region Get from DB
        public virtual List<Game> GetGames()
        {
            var db = new StorageContext();
            return new List<Game>(db.Games.Include(s => s.GenreList).ToList());
        }

        public virtual List<Game> GetGames(int id)
        {
            var db = new StorageContext();
            return new List<Game>(db.Games.Include(s => s.GenreList).ToList().Where(s => s.IDGame.Equals(id)));
        }

        public virtual List<Game> GetGames(string gamename)
        {
            var db = new StorageContext();
            return new List<Game>(db.Games.Include(s => s.GenreList).ToList().Where(s => s.GameName.ToLower().Contains(gamename.ToLower())).ToList());
        }

        public virtual List<Game> GetGames(int[] genres)
        {
            var db = new StorageContext();
            if (genres == null)
            {
                genres = new int[] { };
            }
            List<Game> toreturn = new List<Game>();
            List<Game> games = new List<Game>(db.Games.Include(s => s.GenreList).ToList());
            foreach (var game in games)
            {
                List<int> genreIDs = new List<int>();
                for (int i = 0; i < game.GenreList.Count; i++)
                {
                    genreIDs.Add(game.GenreList.ElementAt(i).IDGenre);
                }
                var intersect = genreIDs.Intersect(genres).ToArray().OrderBy(i => i);
                if (intersect.Count() == genres.Count())
                {
                    toreturn.Add(game);
                }

            }

            return toreturn;
        }

        public virtual List<Game> GetGames(int[] genres, string gamename)
        {
            var db = new StorageContext();
            if (genres == null)
            {
                genres = new int[] { };
            }
            List<Game> toreturn = new List<Game>();
            List<Game> games = new List<Game>(db.Games.Include(s => s.GenreList).ToList().Where(s => s.GameName.ToLower().Contains(gamename.ToLower())).ToList());
            foreach (var game in games)
            {
                List<int> genreIDs = new List<int>();
                for (int i = 0; i < game.GenreList.Count; i++)
                {
                    genreIDs.Add(game.GenreList.ElementAt(i).IDGenre);
                }
                var intersect = genreIDs.Intersect(genres).ToArray().OrderBy(i =>i);
                if (intersect.Count() == genres.Count())
                {
                    toreturn.Add(game);
                }

            }
            
            return toreturn;
        }

        public virtual List<Genre> GetGenres()
        {
            var db = new StorageContext();
            return new List<Genre>(db.Genres.ToList());
        }

        public virtual List<Genre> GetGenres(int id)
        {
            var db = new StorageContext();
            return new List<Genre>(db.Genres.ToList().Where(g => g.IDGenre.Equals(id)));
        }

        public virtual List<Comment> GetComments(Game game)
        {
            var db = new StorageContext();
            if (game != null)
            {
                return new List<Comment>(game.CommentsList);
            }
            else
                return new List<Comment>();
        }
        #endregion

        #region Post to DB

        #region Games
        public virtual void CreateGame(Game _game, int[] genresID)
        {
            try
            {
                var db = new StorageContext();
                if (db.Games.Where(st => st.GameName.ToLower().Equals(_game.GameName.ToLower())).ToList().Count > 0)
                {
                    logger.LogError("Illegal attemp to add game with same name!");
                    throw new Exception("Istnieje już gra z taką nazwą!");
                }
                var game = new Game();
                game.GameName = _game.GameName;
                game.Country = _game.Country;
                game.PublishDate = _game.PublishDate;
                game.Publisher = _game.Publisher;
                //game.CommentsList = new List<Comment>();

                List<Genre> genreslist = new List<Genre>();
                foreach (int i in genresID)
                {
                    var genre = new List<Genre>(db.Genres.ToList().Where(g => g.IDGenre.Equals(i))).First();
                    if (genre != null)
                    {
                        genreslist.Add(genre);
                    }
                    else
                    {
                        throw new Exception("Nie istnieje wybrany gatunek gry! ");
                    }
                }
                game.GenreList = genreslist;

                db.Games.Add(game);
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                List<string> errorMessages = new List<string>();
                foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                {
                    string entityName = validationResult.Entry.Entity.GetType().Name;
                    foreach (DbValidationError error in validationResult.ValidationErrors)
                    {
                        errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                    }
                }
                throw new Exception("Niedozwolona próba utworzenia gry! " + errorMessages);
            }
            catch (Exception ex)
            {
                throw new Exception("Niedozwolona próba utworzenia gry! " + ex.Message);
            }
        }


        public virtual void UpdateGame(Game _game, int[] genresID)
        {
            var db = new StorageContext();
            var original = db.Games.Find(_game.IDGame);
            if (original != null)
            {
                if (!StructuralComparisons.StructuralEqualityComparer.Equals(_game.Stamp, original.Stamp))
                {
                    logger.LogError("Illegal attemp to update game! This record (game) was changed.");
                    throw new Exception("Rekord gry został zmieniony poza programem, który nie zdążył się zaktualizwać.");
                }
                if (db.Games.Where(s => s.GameName.Equals(_game.GameName) && !(s.IDGame.Equals(_game.IDGame))).ToList().Count != 0)
                {
                    logger.LogError("Illegal attemp to update game's name to existing one!");
                    throw new Exception("Już istnieje taka gra!");
                }

                original.Country = _game.Country;
                original.GameName = _game.GameName;
                original.GenreList = _game.GenreList;
                original.IDGame = _game.IDGame;
                original.PublishDate = _game.PublishDate;
                original.Publisher = _game.Publisher;
                original.Stamp = _game.Stamp;

                try
                {
                    db.Games.Find(_game.IDGame).GenreList.Clear();
                    foreach (int i in genresID)
                    {
                        var genre = new List<Genre>(db.Genres.ToList().Where(g => g.IDGenre.Equals(i))).First();
                        if (genre != null)
                        {
                            original.GenreList.Add(genre);
                        }
                        else
                        {
                            throw new Exception("Nie istnieje wybrany gatunek gry! ");
                        }
                    }
                    //original.GenreList.Clear();


                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    List<string> errorMessages = new List<string>();
                    foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                    {
                        string entityName = validationResult.Entry.Entity.GetType().Name;
                        foreach (DbValidationError error in validationResult.ValidationErrors)
                        {
                            errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                        }
                    }
                    throw new Exception("Niedozwolona próba zapisania gry! " + errorMessages);
                }
                catch (Exception ex)
                {
                    throw new Exception("Niedozwolona próba zapisania gry! " + ex.Message);
                }
            }
            else
            {
                throw new Exception("Niedozwolona próba zmiany gry, która już nie istnieje!");
            }
        }

        public virtual void DeleteGame(Game _game, int[] genresID)
        {
            if (_game == null)
            {
                logger.LogWarningMessage("No Game was picked to be removed!");
                throw new Exception("Żadna gra nie została wybrana!");
            }
            logger.LogInfoMessage("Game with id = " + _game.IDGame.ToString() + " is going to be removed.");
            try
            {
                var db = new StorageContext();
                var original = db.Games.Find(_game.IDGame);
                if (original != null)
                {
                    if (!StructuralComparisons.StructuralEqualityComparer.Equals(_game.Stamp, original.Stamp))
                    {
                        logger.LogError("Illegal attemp to delete game! This record (game) was changed.");
                        throw new Exception("Niedozwolona próba usunięcia gry! Rekord gry został zmieniony poza programem, który nie zdążył się zaktualizwać.");
                    }
                    List<Genre> genreslist = new List<Genre>();
                    foreach (int i in genresID)
                    {
                        var genre = new List<Genre>(db.Genres.ToList().Where(g => g.IDGenre.Equals(i))).First();
                        if (genre != null)
                        {
                            genreslist.Add(genre);
                        }
                        else
                        {
                            throw new Exception("Nie istnieje wybrany gatunek gry! ");
                        }
                    }
                    _game.GenreList = genreslist;
                    if (original.Equals(_game))
                    {
                        db.Games.Remove(original);
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Wpis został zmieniony!");
                    }
                }
                else
                {
                    logger.LogWarningMessage("Game (to be removed) was not found in database!");
                    throw new Exception("Gra nie została znaleziona w bazie danych.");
                }
            }
            catch (DbEntityValidationException ex)
            {
                List<string> errorMessages = new List<string>();
                foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                {
                    string entityName = validationResult.Entry.Entity.GetType().Name;
                    foreach (DbValidationError error in validationResult.ValidationErrors)
                    {
                        errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                    }
                }
                throw new Exception("Niedozwolona próba usunięcia gry! " + errorMessages);
            }
            catch (Exception ex)
            {
                throw new Exception("Niedozwolona próba usunięcia gry! " + ex.Message);
            }
        }
        #endregion

        #region Comments
        public virtual void CreateComment(Comment _comment)
        {
            try
            {
                var db = new StorageContext();
                var comment = new Comment()
                {
                    CommentContent = _comment.CommentContent.Trim(),
                    Nickname = _comment.Nickname,
                    PublishDate = DateTime.Now,
                    CommentedGameRefID = _comment.CommentedGameRefID,
                    Email = _comment.Email
                };

                //comment.CommentedGame = db.Games.Include(s => s.GenreList).ToList().Where(s => s.IDGame.Equals(comment.CommentedGameRefID)).First();



                db.Comments.Add(comment);
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                List<string> errorMessages = new List<string>();
                foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                {
                    string entityName = validationResult.Entry.Entity.GetType().Name;
                    foreach (DbValidationError error in validationResult.ValidationErrors)
                    {
                        errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                    }
                }
                throw new Exception("Niedozwolona próba utworzenia komentarza! " + errorMessages);
            }
            catch (Exception ex)
            {
                throw new Exception("Niedozwolona próba utworzenia komentarza! " + ex.Message);
            }
        }

        public virtual void UpdateComment(Comment _comment)
        {
            var db = new StorageContext();
            var original = db.Comments.Find(_comment.IDComment);
            if (original != null)
            {
                if (!StructuralComparisons.StructuralEqualityComparer.Equals(_comment.Stamp, original.Stamp))
                {
                    logger.LogError("Illegal attemp to update Comment! This record (comment) was changed.");
                    throw new Exception("Rekord komentarza został zmieniony poza programem, który nie zdążył się zaktualizwać.");
                }
                if (db.Comments.Where(s => s.Nickname.Equals(_comment.Nickname) && s.Email.Equals(_comment.Email) && s.CommentContent.Equals(_comment.CommentContent)).ToList().Count != 0)
                {
                    logger.LogError("Illegal attemp to update comment to existing one!");
                    throw new Exception("Już istnieje identyczny komentarz!");
                }

                original.Nickname = _comment.Nickname;
                original.Email = _comment.Email;
                original.CommentContent = _comment.CommentContent;
                original.IDComment = _comment.IDComment;
                original.PublishDate = DateTime.Now;
                original.Stamp = _comment.Stamp;

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    List<string> errorMessages = new List<string>();
                    foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                    {
                        string entityName = validationResult.Entry.Entity.GetType().Name;
                        foreach (DbValidationError error in validationResult.ValidationErrors)
                        {
                            errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                        }
                    }
                    throw new Exception("Niedozwolona próba zapisania komentarza! " + errorMessages);
                }
                catch (Exception ex)
                {
                    throw new Exception("Niedozwolona próba zapisania komentarza! " + ex.Message);
                }
            }
            else
            {
                throw new Exception("Niedozwolona próba zmiany komentarza, która już nie istnieje!");
            }
        }

        public virtual void DeleteComment(Comment _comment)
        {
            {
                if (_comment == null)
                {
                    logger.LogWarningMessage("No Comment was picked to be removed!");
                    throw new Exception("Żaden komentarz nie został wybrany!");
                }
                logger.LogInfoMessage("Comment with id = " + _comment.IDComment.ToString() + " is going to be removed.");
                try
                {
                    var db = new StorageContext();
                    var original = db.Comments.Find(_comment.IDComment);
                    if (original != null)
                    {
                        if (!StructuralComparisons.StructuralEqualityComparer.Equals(_comment.Stamp, original.Stamp))
                        {
                            logger.LogError("Illegal attemp to delete Comment! This record (comment) was changed.");
                            throw new Exception("Niedozwolona próba aktualizacji komentarza! Rekord komentarza został zmieniony poza programem, który nie zdążył się zaktualizwać.");
                        }

                        if (original.Equals(_comment))
                        {
                            db.Comments.Remove(original);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Wpis został zmieniony!");
                        }
                    }
                    else
                    {
                        logger.LogWarningMessage("Comment (to be removed) was not found in database!");
                        throw new Exception("Komentarz nie został znaleziony w bazie danych.");
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    List<string> errorMessages = new List<string>();
                    foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                    {
                        string entityName = validationResult.Entry.Entity.GetType().Name;
                        foreach (DbValidationError error in validationResult.ValidationErrors)
                        {
                            errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                        }
                    }
                    throw new Exception("Niedozwolona próba usunięcia komentarza! " + errorMessages);
                }
                catch (Exception ex)
                {
                    throw new Exception("Niedozwolona próba usunięcia komentarza! " + ex.Message);
                }
            }
        }

        public virtual void DeleteComment(int id)
        {
            var db = new StorageContext();
            DeleteComment(db.Comments.Find(id));
        }
        #endregion

        #region Genres

        public virtual void CreateGenre(Genre _genre)
        {
            try
            {
                var db = new StorageContext();
                if (db.Genres.Where(st => st.GenreName.ToLower().Equals(_genre.GenreName.ToLower())).ToList().Count > 0)
                {
                    logger.LogError("Illegal attemp to add genre with same name!");
                    throw new Exception("Istnieje już styl gry z taką nazwą!");
                }
                var genre = new Genre();
                genre.GenreName = _genre.GenreName;



                db.Genres.Add(genre);
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                List<string> errorMessages = new List<string>();
                foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                {
                    string entityName = validationResult.Entry.Entity.GetType().Name;
                    foreach (DbValidationError error in validationResult.ValidationErrors)
                    {
                        errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                    }
                }
                throw new Exception("Niedozwolona próba utworzenia stylu gry! " + errorMessages);
            }
            catch (Exception ex)
            {
                throw new Exception("Niedozwolona próba utworzenia stylu gry! " + ex.Message);
            }
        }

        public virtual void UpdateGenre(Genre _genre)
        {
            var db = new StorageContext();
            var original = db.Genres.Find(_genre.IDGenre);
            if (original != null)
            {
                if (!StructuralComparisons.StructuralEqualityComparer.Equals(_genre.Stamp, original.Stamp))
                {
                    logger.LogError("Illegal attemp to update genre! This record (genre) was changed.");
                    throw new Exception("Rekord gatunku gry został zmieniony poza programem, który nie zdążył się zaktualizwać.");
                }
                if (db.Genres.Where(s => s.GenreName.Equals(_genre.GenreName) && !(s.IDGenre.Equals(_genre.IDGenre))).ToList().Count != 0)
                {
                    logger.LogError("Illegal attemp to update genre's name to existing one!");
                    throw new Exception("Już istnieje taki gatunek gry!");
                }

                original.GenreName = _genre.GenreName;
                original.IDGenre = _genre.IDGenre;
                original.Stamp = _genre.Stamp;

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    List<string> errorMessages = new List<string>();
                    foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                    {
                        string entityName = validationResult.Entry.Entity.GetType().Name;
                        foreach (DbValidationError error in validationResult.ValidationErrors)
                        {
                            errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                        }
                    }
                    throw new Exception("Niedozwolona próba zapisania gatunku gry! " + errorMessages);
                }
                catch (Exception ex)
                {
                    throw new Exception("Niedozwolona próba zapisania gatunku gry! " + ex.Message);
                }
            }
            else
            {
                throw new Exception("Niedozwolona próba zmiany gatunku gry, który już nie istnieje!");
            }
        }


        public virtual void DeleteGenre(Genre _genre)
        {
            if (_genre == null)
            {
                logger.LogWarningMessage("No Genre was picked to be removed!");
                throw new Exception("Żaden gatunek gry nie został wybrany!");
            }
            logger.LogInfoMessage("Genre with id = " + _genre.IDGenre.ToString() + " is going to be removed.");
            try
            {
                var db = new StorageContext();
                var original = db.Genres.Find(_genre.IDGenre);
                if (original != null)
                {
                    if (!StructuralComparisons.StructuralEqualityComparer.Equals(_genre.Stamp, original.Stamp))
                    {
                        logger.LogError("Illegal attemp to delete genre! This record (genre) was changed.");
                        throw new Exception("Niedozwolona próba usunięcia gatunku! Rekord gatunku gry został zmieniony poza programem, który nie zdążył się zaktualizwać.");
                    }

                    if (original.Equals(_genre))
                    {
                        db.Genres.Remove(original);
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Wpis został zmieniony!");
                    }
                }
                else
                {
                    logger.LogWarningMessage("Genre (to be removed) was not found in database!");
                    throw new Exception("Gatunek gry nie został znaleziony w bazie danych.");
                }
            }
            catch (DbEntityValidationException ex)
            {
                List<string> errorMessages = new List<string>();
                foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
                {
                    string entityName = validationResult.Entry.Entity.GetType().Name;
                    foreach (DbValidationError error in validationResult.ValidationErrors)
                    {
                        errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                    }
                }
                throw new Exception("Niedozwolona próba usunięcia gatunku! " + errorMessages);
            }
            catch (Exception ex)
            {
                throw new Exception("Niedozwolona próba usunięcia gatunku! " + ex.Message);
            }
        }
    }



    #endregion
    #endregion


}