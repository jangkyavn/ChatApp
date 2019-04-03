namespace ChatApp3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Messages", "IsStartDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "IsStartDate", c => c.Boolean(nullable: false));
        }
    }
}
