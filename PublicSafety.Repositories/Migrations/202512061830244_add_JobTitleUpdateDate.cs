namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_JobTitleUpdateDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "JobTitleUpdateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "JobTitleUpdateDate");
        }
    }
}
