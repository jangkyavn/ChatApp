namespace ChatApp3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageID = c.Guid(nullable: false),
                        Content = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        SenderName = c.String(),
                        ReceiverName = c.String(),
                        IsStartDate = c.Boolean(nullable: false),
                        ContentType = c.String(),
                        User_UserID = c.Int(),
                    })
                .PrimaryKey(t => t.MessageID)
                .ForeignKey("dbo.Users", t => t.User_UserID)
                .Index(t => t.User_UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Connected = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "User_UserID", "dbo.Users");
            DropIndex("dbo.Messages", new[] { "User_UserID" });
            DropTable("dbo.Users");
            DropTable("dbo.Messages");
        }
    }
}
