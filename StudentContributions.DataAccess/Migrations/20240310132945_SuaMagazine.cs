using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentContributions.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SuaMagazine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Magazines_Faculties_FacutyID",
                table: "Magazines");

            migrationBuilder.DropForeignKey(
                name: "FK_Magazines_Semesters_SemsterID",
                table: "Magazines");

            migrationBuilder.RenameColumn(
                name: "SemsterID",
                table: "Magazines",
                newName: "SemesterID");

            migrationBuilder.RenameColumn(
                name: "FacutyID",
                table: "Magazines",
                newName: "FacultyID");

            migrationBuilder.RenameIndex(
                name: "IX_Magazines_SemsterID",
                table: "Magazines",
                newName: "IX_Magazines_SemesterID");

            migrationBuilder.RenameIndex(
                name: "IX_Magazines_FacutyID",
                table: "Magazines",
                newName: "IX_Magazines_FacultyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Magazines_Faculties_FacultyID",
                table: "Magazines",
                column: "FacultyID",
                principalTable: "Faculties",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Magazines_Semesters_SemesterID",
                table: "Magazines",
                column: "SemesterID",
                principalTable: "Semesters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Magazines_Faculties_FacultyID",
                table: "Magazines");

            migrationBuilder.DropForeignKey(
                name: "FK_Magazines_Semesters_SemesterID",
                table: "Magazines");

            migrationBuilder.RenameColumn(
                name: "SemesterID",
                table: "Magazines",
                newName: "SemsterID");

            migrationBuilder.RenameColumn(
                name: "FacultyID",
                table: "Magazines",
                newName: "FacutyID");

            migrationBuilder.RenameIndex(
                name: "IX_Magazines_SemesterID",
                table: "Magazines",
                newName: "IX_Magazines_SemsterID");

            migrationBuilder.RenameIndex(
                name: "IX_Magazines_FacultyID",
                table: "Magazines",
                newName: "IX_Magazines_FacutyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Magazines_Faculties_FacutyID",
                table: "Magazines",
                column: "FacutyID",
                principalTable: "Faculties",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Magazines_Semesters_SemsterID",
                table: "Magazines",
                column: "SemsterID",
                principalTable: "Semesters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
