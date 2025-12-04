namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit_employee_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        SectionId = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.SectionId);
            
            AddColumn("dbo.Employees", "Email", c => c.String());
            AddColumn("dbo.Employees", "Phone", c => c.String());
            AddColumn("dbo.Employees", "IsIntern", c => c.Boolean(nullable: false));
            AddColumn("dbo.Employees", "WorkLocation", c => c.Int(nullable: false));
            AddColumn("dbo.Employees", "HealthInsuranceFile", c => c.String());
            AddColumn("dbo.Employees", "DepartmentId", c => c.Guid(nullable: false));
            AddColumn("dbo.Employees", "SectionId", c => c.Guid(nullable: false));
            AddColumn("dbo.Employees", "EmploymentDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Employees", "RetirementDate", c => c.DateTime());
            CreateIndex("dbo.Employees", "DepartmentId");
            CreateIndex("dbo.Employees", "SectionId");
            AddForeignKey("dbo.Employees", "DepartmentId", "dbo.Departments", "DepartmentId");
            AddForeignKey("dbo.Employees", "SectionId", "dbo.Sections", "SectionId");
            DropColumn("dbo.Employees", "UpdatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "UpdatedDate", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Employees", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.Employees", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Employees", new[] { "SectionId" });
            DropIndex("dbo.Employees", new[] { "DepartmentId" });
            DropColumn("dbo.Employees", "RetirementDate");
            DropColumn("dbo.Employees", "EmploymentDate");
            DropColumn("dbo.Employees", "SectionId");
            DropColumn("dbo.Employees", "DepartmentId");
            DropColumn("dbo.Employees", "HealthInsuranceFile");
            DropColumn("dbo.Employees", "WorkLocation");
            DropColumn("dbo.Employees", "IsIntern");
            DropColumn("dbo.Employees", "Phone");
            DropColumn("dbo.Employees", "Email");
            DropTable("dbo.Sections");
            DropTable("dbo.Departments");
        }
    }
}
