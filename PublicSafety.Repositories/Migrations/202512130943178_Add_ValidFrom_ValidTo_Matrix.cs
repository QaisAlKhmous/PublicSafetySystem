namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ValidFrom_ValidTo_Matrix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matrices", "ValidFrom", c => c.DateTime(nullable: false));
            AddColumn("dbo.Matrices", "ValidTo", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matrices", "ValidTo");
            DropColumn("dbo.Matrices", "ValidFrom");
        }
    }
}
