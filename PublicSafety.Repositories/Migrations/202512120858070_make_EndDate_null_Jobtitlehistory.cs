namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class make_EndDate_null_Jobtitlehistory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmployeeJobTitleHistories", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmployeeJobTitleHistories", "EndDate", c => c.DateTime(nullable: false));
        }
    }
}
