namespace GameShow.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        IDComment = c.Int(nullable: false, identity: true),
                        Nickname = c.String(nullable: false, maxLength: 32),
                        Email = c.String(nullable: false),
                        PublishDate = c.DateTime(storeType: "date"),
                        CommentContent = c.String(maxLength: 1024),
                        CommentedGameRefID = c.Int(nullable: false),
                        Stamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.IDComment)
                .ForeignKey("dbo.Games", t => t.CommentedGameRefID, cascadeDelete: true)
                .Index(t => t.CommentedGameRefID);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        IDGame = c.Int(nullable: false, identity: true),
                        GameName = c.String(nullable: false, maxLength: 32),
                        Publisher = c.String(maxLength: 32),
                        PublishDate = c.DateTime(storeType: "date"),
                        Country = c.String(maxLength: 32),
                        Stamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.IDGame);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        IDGenre = c.Int(nullable: false, identity: true),
                        GenreName = c.String(nullable: false, maxLength: 32),
                        Stamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.IDGenre);
            
            CreateTable(
                "dbo.GenreGames",
                c => new
                    {
                        Genre_IDGenre = c.Int(nullable: false),
                        Game_IDGame = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Genre_IDGenre, t.Game_IDGame })
                .ForeignKey("dbo.Genres", t => t.Genre_IDGenre, cascadeDelete: true)
                .ForeignKey("dbo.Games", t => t.Game_IDGame, cascadeDelete: true)
                .Index(t => t.Genre_IDGenre)
                .Index(t => t.Game_IDGame);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GenreGames", "Game_IDGame", "dbo.Games");
            DropForeignKey("dbo.GenreGames", "Genre_IDGenre", "dbo.Genres");
            DropForeignKey("dbo.Comments", "CommentedGameRefID", "dbo.Games");
            DropIndex("dbo.GenreGames", new[] { "Game_IDGame" });
            DropIndex("dbo.GenreGames", new[] { "Genre_IDGenre" });
            DropIndex("dbo.Comments", new[] { "CommentedGameRefID" });
            DropTable("dbo.GenreGames");
            DropTable("dbo.Genres");
            DropTable("dbo.Games");
            DropTable("dbo.Comments");
        }
    }
}
