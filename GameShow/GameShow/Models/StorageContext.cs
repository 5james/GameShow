using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameShow.Models
{
    public class StorageContext : DbContext
    {
        public StorageContext() : base("GamesDB")
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Game>()
            //.HasMany(e => e.GenreList);

            //modelBuilder.Entity<Genre>()
            //.HasMany(e => e.Games);

            modelBuilder.Entity<Game>()
            .Property(e => e.Stamp)
            .IsFixedLength();

            modelBuilder.Entity<Comment>()
            .Property(e => e.Stamp)
            .IsFixedLength();

            modelBuilder.Entity<Genre>()
            .Property(e => e.Stamp)
            .IsFixedLength();

            base.OnModelCreating(modelBuilder);
        }

        //public System.Data.Entity.DbSet<GameShow.Models.Comment> Comments { get; set; }
    }
}