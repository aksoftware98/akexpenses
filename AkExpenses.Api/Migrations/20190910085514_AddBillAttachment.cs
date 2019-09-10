using Microsoft.EntityFrameworkCore.Migrations;

namespace AkExpenses.Api.Migrations
{
    public partial class AddBillAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attachment",
                table: "Bills",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachment",
                table: "Bills");
        }
    }
}
