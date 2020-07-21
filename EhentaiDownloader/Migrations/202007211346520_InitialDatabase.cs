namespace EhentaiDownloader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EbookPageModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(unicode: false),
                        Title = c.String(unicode: false),
                        SubTitle = c.String(unicode: false),
                        Author = c.String(unicode: false),
                        ISBN = c.String(unicode: false),
                        Year = c.Int(nullable: false),
                        Pages = c.Int(nullable: false),
                        Language = c.String(unicode: false),
                        FileSize = c.String(unicode: false),
                        FileFormat = c.String(unicode: false),
                        Category = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        FilePaths = c.String(unicode: false),
                        ImagePath = c.String(unicode: false),
                        FileCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EbookPageModel");
        }
    }
}
