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
                        SenderName = c.String(maxLength: 128),
                        ReceiverName = c.String(maxLength: 128),
                        IsStartDate = c.Boolean(nullable: false),
                        ContentType = c.String(),
                        User_UserName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MessageID)
                .ForeignKey("dbo.Users", t => t.User_UserName)
                .ForeignKey("dbo.Users", t => t.SenderName)
                .ForeignKey("dbo.Users", t => t.ReceiverName)
                .Index(t => t.SenderName)
                .Index(t => t.ReceiverName)
                .Index(t => t.User_UserName);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        Connected = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "ReceiverName", "dbo.Users");
            DropForeignKey("dbo.Messages", "SenderName", "dbo.Users");
            DropForeignKey("dbo.Messages", "User_UserName", "dbo.Users");
            DropIndex("dbo.Messages", new[] { "User_UserName" });
            DropIndex("dbo.Messages", new[] { "ReceiverName" });
            DropIndex("dbo.Messages", new[] { "SenderName" });
            DropTable("dbo.Users");
            DropTable("dbo.Messages");
        }
    }
}
