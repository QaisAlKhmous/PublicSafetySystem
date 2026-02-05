namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_dates_jobtitles : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.JobTitles", "CreatedDate");
            DropColumn("dbo.JobTitles", "UpdatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobTitles", "UpdatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.JobTitles", "CreatedDate", c => c.DateTime(nullable: false));
        }
    }
}
