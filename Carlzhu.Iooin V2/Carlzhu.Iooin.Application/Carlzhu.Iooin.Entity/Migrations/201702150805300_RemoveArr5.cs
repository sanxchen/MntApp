namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveArr5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Apparatus", "CalDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Apparatus", "CalResult", c => c.Int(nullable: false));
            AlterColumn("dbo.Apparatus", "CalCircle", c => c.Int(nullable: false));
            AlterColumn("dbo.Apparatus", "NextCalDate", c => c.DateTime());
            AlterColumn("dbo.Apparatus", "CalType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Apparatus", "CalType", c => c.String());
            AlterColumn("dbo.Apparatus", "NextCalDate", c => c.String());
            AlterColumn("dbo.Apparatus", "CalCircle", c => c.String());
            AlterColumn("dbo.Apparatus", "CalResult", c => c.String());
            AlterColumn("dbo.Apparatus", "CalDate", c => c.String());
        }
    }
}
