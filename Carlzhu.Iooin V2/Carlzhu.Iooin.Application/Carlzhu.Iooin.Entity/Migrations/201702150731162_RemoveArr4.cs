namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveArr4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Apparatus", "NextCalDate", c => c.String());
            AlterColumn("dbo.Apparatus", "CalType", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Apparatus", "CalType", c => c.Int(nullable: false));
            AlterColumn("dbo.Apparatus", "NextCalDate", c => c.DateTime(nullable: false));
        }
    }
}
