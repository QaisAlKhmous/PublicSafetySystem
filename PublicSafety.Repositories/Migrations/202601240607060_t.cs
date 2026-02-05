namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class t : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DepartmentJobTitles");
            DropPrimaryKey("dbo.SectionJobTitles");
            AddPrimaryKey("dbo.DepartmentJobTitles", "DepartmentJobTitleId");
            AddPrimaryKey("dbo.SectionJobTitles", "SectionJobTitleId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.SectionJobTitles");
            DropPrimaryKey("dbo.DepartmentJobTitles");
            AddPrimaryKey("dbo.SectionJobTitles", new[] { "SectionId", "JobTitleId" });
            AddPrimaryKey("dbo.DepartmentJobTitles", new[] { "DepartmentId", "JobTitleId" });
        }
    }
}
