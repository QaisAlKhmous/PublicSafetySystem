namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class make_sectionId_inEmployees_optional : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Employees", new[] { "SectionId" });
            AlterColumn("dbo.Employees", "SectionId", c => c.Guid());
            CreateIndex("dbo.Employees", "SectionId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Employees", new[] { "SectionId" });
            AlterColumn("dbo.Employees", "SectionId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Employees", "SectionId");
        }
    }
}
