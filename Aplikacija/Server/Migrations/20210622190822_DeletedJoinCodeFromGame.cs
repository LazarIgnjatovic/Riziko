using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class DeletedJoinCodeFromGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinCode",
                table: "Game");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JoinCode",
                table: "Game",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
