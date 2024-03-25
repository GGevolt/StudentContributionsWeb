using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentContributions.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class sead4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Contributions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Contributions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_ApplicationUserId",
                table: "Contributions",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contributions_AspNetUsers_ApplicationUserId",
                table: "Contributions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contributions_AspNetUsers_ApplicationUserId",
                table: "Contributions");

            migrationBuilder.DropIndex(
                name: "IX_Contributions_ApplicationUserId",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Contributions");
        }
    }
}
