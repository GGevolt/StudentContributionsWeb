using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentContributions.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Test234 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "SemesterName",
                table: "Semesters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Contributions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_UserID",
                table: "Contributions",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contributions_AspNetUsers_UserID",
                table: "Contributions",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contributions_AspNetUsers_UserID",
                table: "Contributions");

            migrationBuilder.DropIndex(
                name: "IX_Contributions_UserID",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "SemesterName",
                table: "Semesters");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Contributions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Contributions",
                type: "nvarchar(450)",
                nullable: true);

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
    }
}
