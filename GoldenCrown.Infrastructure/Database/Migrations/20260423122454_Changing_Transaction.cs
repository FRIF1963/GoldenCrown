using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoldenCrown.Migrations
{
    /// <inheritdoc />
    public partial class Changing_Transaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "session_id",
                table: "transaction",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "transaction",
                newName: "session_id");
        }
    }
}
