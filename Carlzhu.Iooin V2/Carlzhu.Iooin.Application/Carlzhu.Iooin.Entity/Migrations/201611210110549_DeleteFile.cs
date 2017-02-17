namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteFile : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BaseEmail", "Category", "dbo.BaseEmailCategory");
            DropForeignKey("dbo.BaseEmailAccessory", "EmailId", "dbo.BaseEmail");
            DropForeignKey("dbo.BaseEmailAddressee", "EmailId", "dbo.BaseEmail");
            DropForeignKey("dbo.BaseNetworkFile", "FolderId", "dbo.BaseNetworkFolder");
            DropIndex("dbo.BaseEmailAccessory", new[] { "EmailId" });
            DropIndex("dbo.BaseEmail", new[] { "Category" });
            DropIndex("dbo.BaseEmailAddressee", new[] { "EmailId" });
            DropIndex("dbo.BaseNetworkFile", new[] { "FolderId" });
            DropTable("dbo.BaseEmailAccessory");
            DropTable("dbo.BaseEmail");
            DropTable("dbo.BaseEmailCategory");
            DropTable("dbo.BaseEmailAddressee");
            DropTable("dbo.BaseNetworkFile");
            DropTable("dbo.BaseNetworkFolder");
            DropTable("dbo.BasePhoneNote");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.BasePhoneNote",
                c => new
                    {
                        PhoneNoteId = c.String(nullable: false, maxLength: 128),
                        PhonenNumber = c.String(),
                        SendContent = c.String(),
                        SendTime = c.DateTime(),
                        SendStatus = c.String(),
                        DeviceName = c.String(),
                        Remark = c.String(),
                        Enabled = c.Int(),
                        SortCode = c.Int(),
                        DeleteMark = c.Int(),
                        CreateDate = c.DateTime(),
                        CreateUserId = c.String(),
                        CreateUserName = c.String(),
                    })
                .PrimaryKey(t => t.PhoneNoteId);
            
            CreateTable(
                "dbo.BaseNetworkFolder",
                c => new
                    {
                        FolderId = c.String(nullable: false, maxLength: 128),
                        ParentId = c.String(),
                        Category = c.String(),
                        FolderName = c.String(),
                        IsPublic = c.Int(),
                        Enabled = c.Int(),
                        Sharing = c.Int(),
                        SharingFolderId = c.String(),
                        SharingCreateDate = c.DateTime(),
                        SharingEndDate = c.DateTime(),
                        SortCode = c.Int(),
                        DeleteMark = c.Int(),
                        CreateDate = c.DateTime(),
                        CreateUserId = c.String(),
                        CreateUserName = c.String(),
                        ModifyDate = c.DateTime(),
                        ModifyUserId = c.String(),
                        ModifyUserName = c.String(),
                    })
                .PrimaryKey(t => t.FolderId);
            
            CreateTable(
                "dbo.BaseNetworkFile",
                c => new
                    {
                        NetworkFileId = c.String(nullable: false, maxLength: 128),
                        FolderId = c.String(maxLength: 128),
                        FileName = c.String(),
                        FilePath = c.String(),
                        FileSize = c.String(),
                        FileExtensions = c.String(),
                        FileType = c.String(),
                        Icon = c.String(),
                        Sharing = c.Int(),
                        SharingFolderId = c.String(),
                        SharingCreateDate = c.DateTime(),
                        SharingEndDate = c.DateTime(),
                        Enabled = c.Int(),
                        SortCode = c.Int(),
                        DeleteMark = c.Int(),
                        CreateDate = c.DateTime(),
                        CreateUserId = c.String(),
                        CreateUserName = c.String(),
                    })
                .PrimaryKey(t => t.NetworkFileId);
            
            CreateTable(
                "dbo.BaseEmailAddressee",
                c => new
                    {
                        EmailAddresseeId = c.String(nullable: false, maxLength: 128),
                        EmailId = c.String(maxLength: 128),
                        AddresseeId = c.String(),
                        AddresseeName = c.String(),
                        AddresseeIdState = c.Int(),
                        IsRead = c.Int(),
                        ReadCount = c.Int(),
                        ReadDate = c.DateTime(),
                        EndReadDate = c.DateTime(),
                        Highlight = c.Int(),
                        Backlog = c.Int(),
                        CreateDate = c.DateTime(),
                        DeleteMark = c.Int(),
                    })
                .PrimaryKey(t => t.EmailAddresseeId);
            
            CreateTable(
                "dbo.BaseEmailCategory",
                c => new
                    {
                        EmailCategoryId = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        Remark = c.String(),
                        Enabled = c.Int(),
                        SortCode = c.Int(),
                        DeleteMark = c.Int(),
                        CreateDate = c.DateTime(),
                        CreateUserId = c.String(),
                        CreateUserName = c.String(),
                        ModifyDate = c.DateTime(),
                        ModifyUserId = c.String(),
                        ModifyUserName = c.String(),
                    })
                .PrimaryKey(t => t.EmailCategoryId);
            
            CreateTable(
                "dbo.BaseEmail",
                c => new
                    {
                        EmailId = c.String(nullable: false, maxLength: 128),
                        ParentId = c.String(),
                        Category = c.String(maxLength: 128),
                        Theme = c.String(),
                        ThemeColour = c.String(),
                        Content = c.String(),
                        Addresser = c.String(),
                        SendDate = c.DateTime(),
                        IsAccessory = c.Int(),
                        Priority = c.Int(),
                        Receipt = c.Int(),
                        IsDelayed = c.Int(),
                        DelayedTime = c.DateTime(),
                        State = c.Int(),
                        DeleteMark = c.Int(),
                        CreateDate = c.DateTime(),
                        CreateUserId = c.String(),
                        CreateUserName = c.String(),
                    })
                .PrimaryKey(t => t.EmailId);
            
            CreateTable(
                "dbo.BaseEmailAccessory",
                c => new
                    {
                        EmailAccessoryId = c.String(nullable: false, maxLength: 128),
                        EmailId = c.String(maxLength: 128),
                        FileName = c.String(),
                        FilePath = c.String(),
                        FileSize = c.String(),
                        FileType = c.String(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmailAccessoryId);
            
            CreateIndex("dbo.BaseNetworkFile", "FolderId");
            CreateIndex("dbo.BaseEmailAddressee", "EmailId");
            CreateIndex("dbo.BaseEmail", "Category");
            CreateIndex("dbo.BaseEmailAccessory", "EmailId");
            AddForeignKey("dbo.BaseNetworkFile", "FolderId", "dbo.BaseNetworkFolder", "FolderId");
            AddForeignKey("dbo.BaseEmailAddressee", "EmailId", "dbo.BaseEmail", "EmailId");
            AddForeignKey("dbo.BaseEmailAccessory", "EmailId", "dbo.BaseEmail", "EmailId");
            AddForeignKey("dbo.BaseEmail", "Category", "dbo.BaseEmailCategory", "EmailCategoryId");
        }
    }
}
