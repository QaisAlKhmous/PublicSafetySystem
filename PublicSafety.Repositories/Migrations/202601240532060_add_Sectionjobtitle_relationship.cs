namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Sectionjobtitle_relationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SectionJobTitles",
                c => new
                    {
                        SectionId = c.Guid(nullable: false),
                        JobTitleId = c.Guid(nullable: false),
                        SectionJobTitleId = c.Guid(nullable: false),
                        Department_DepartmentId = c.Guid(),
                    })
                .PrimaryKey(t => new { t.SectionId, t.JobTitleId })
                .ForeignKey("dbo.JobTitles", t => t.JobTitleId)
                .ForeignKey("dbo.Sections", t => t.SectionId)
                .ForeignKey("dbo.Departments", t => t.Department_DepartmentId)
                .Index(t => t.SectionId)
                .Index(t => t.JobTitleId)
                .Index(t => t.Department_DepartmentId);
            
            AddColumn("dbo.DepartmentJobTitles", "DepartmentJobTitleId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionJobTitles", "Department_DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.SectionJobTitles", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.SectionJobTitles", "JobTitleId", "dbo.JobTitles");
            DropIndex("dbo.SectionJobTitles", new[] { "Department_DepartmentId" });
            DropIndex("dbo.SectionJobTitles", new[] { "JobTitleId" });
            DropIndex("dbo.SectionJobTitles", new[] { "SectionId" });
            DropColumn("dbo.DepartmentJobTitles", "DepartmentJobTitleId");
            DropTable("dbo.SectionJobTitles");
        }
    }
}
