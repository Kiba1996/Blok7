namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payPal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayPals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PayementId = c.String(),
                        CreateTime = c.DateTime(),
                        PayerEmail = c.String(),
                        PayerName = c.String(),
                        PayerSurname = c.String(),
                        CurrencyCode = c.String(),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Tickets", "PayPalId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tickets", "PayPalId");
            AddForeignKey("dbo.Tickets", "PayPalId", "dbo.PayPals", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "PayPalId", "dbo.PayPals");
            DropIndex("dbo.Tickets", new[] { "PayPalId" });
            DropColumn("dbo.Tickets", "PayPalId");
            DropTable("dbo.PayPals");
        }
    }
}
