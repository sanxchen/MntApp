namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEnum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Apparatus", "Status", c => c.String());
            AlterColumn("dbo.Apparatus", "CalResult", c => c.String());
            AlterColumn("dbo.Apparatus", "CalCircle", c => c.String());
            AlterColumn("dbo.Apparatus", "CalType", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Apparatus", "CalType", c => c.Int(nullable: false));
            AlterColumn("dbo.Apparatus", "CalCircle", c => c.Int(nullable: false));
            AlterColumn("dbo.Apparatus", "CalResult", c => c.Int(nullable: false));
            DropColumn("dbo.Apparatus", "Status");
        }
    }
}
