using GoldenCrown.Api.Database.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoldenCrown.Migrations
{
    /// <inheritdoc />
    public partial class AccountCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_account_user_id",
                table: "account");

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "RUB");

            migrationBuilder.CreateIndex(
                name: "IX_account_user_id",
                table: "account",
                column: "user_id");

            migrationBuilder.Sql($@"
                insert into account (user_id,currency,balance)
                select u.id, '{Currency.USD}', 0
                from users u;

");
            migrationBuilder.Sql($@"
                insert into account (user_id,currency,balance)
                select u.id, '{Currency.EUR}', 0
                from users u;

");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_account_user_id",
                table: "account");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "account");

            migrationBuilder.CreateIndex(
                name: "IX_account_user_id",
                table: "account",
                column: "user_id",
                unique: true);
        }
    }
}
