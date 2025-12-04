namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class make_dates_null : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Matrices", "EffectiveFrom", c => c.DateTime());
            AlterColumn("dbo.Matrices", "EffectiveTo", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Matrices", "EffectiveTo", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Matrices", "EffectiveFrom", c => c.DateTime(nullable: false));
        }
    }
}
