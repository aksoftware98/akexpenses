using Microsoft.EntityFrameworkCore.Migrations;

namespace AkExpenses.Api.Migrations
{
    public partial class addDebts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debt_Accounts_AccountId",
                table: "Debt");

            migrationBuilder.DropForeignKey(
                name: "FK_DebtPayment_Debt_DebtId",
                table: "DebtPayment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Debt",
                table: "Debt");

            migrationBuilder.RenameTable(
                name: "Debt",
                newName: "Debts");

            migrationBuilder.RenameIndex(
                name: "IX_Debt_AccountId",
                table: "Debts",
                newName: "IX_Debts_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Debts",
                table: "Debts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DebtPayment_Debts_DebtId",
                table: "DebtPayment",
                column: "DebtId",
                principalTable: "Debts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Accounts_AccountId",
                table: "Debts",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DebtPayment_Debts_DebtId",
                table: "DebtPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Accounts_AccountId",
                table: "Debts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Debts",
                table: "Debts");

            migrationBuilder.RenameTable(
                name: "Debts",
                newName: "Debt");

            migrationBuilder.RenameIndex(
                name: "IX_Debts_AccountId",
                table: "Debt",
                newName: "IX_Debt_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Debt",
                table: "Debt",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Debt_Accounts_AccountId",
                table: "Debt",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DebtPayment_Debt_DebtId",
                table: "DebtPayment",
                column: "DebtId",
                principalTable: "Debt",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
