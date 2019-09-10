using Microsoft.EntityFrameworkCore.Migrations;

namespace AkExpenses.Api.Migrations
{
    public partial class AddAccountUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Accounts",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Accounts");
        }
    }
}
