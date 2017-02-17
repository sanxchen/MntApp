namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTaskSch : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaskSch",
                c => new
                    {
                        TaskID = c.Int(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        OwnerID = c.Int(),
                        IsAllDay = c.Boolean(nullable: false),
                        RecurrenceRule = c.String(),
                        RecurrenceID = c.Int(),
                        RecurrenceException = c.String(),
                        StartTimezone = c.String(),
                        EndTimezone = c.String(),
                        Task1_TaskID = c.Int(),
                    })
                .PrimaryKey(t => t.TaskID)
                .ForeignKey("dbo.TaskSch", t => t.Task1_TaskID)
                .Index(t => t.Task1_TaskID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TaskSch", "Task1_TaskID", "dbo.TaskSch");
            DropIndex("dbo.TaskSch", new[] { "Task1_TaskID" });
            DropTable("dbo.TaskSch");
        }
    }
}
