using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Migrations
{
    /// <inheritdoc />
    public partial class tableChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Player2",
                table: "Games",
                newName: "PlayerX");

            migrationBuilder.RenameColumn(
                name: "Player1",
                table: "Games",
                newName: "PlayerO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayerX",
                table: "Games",
                newName: "Player2");

            migrationBuilder.RenameColumn(
                name: "PlayerO",
                table: "Games",
                newName: "Player1");
        }
    }
}
