using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _250828_universityTask.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingBasicData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Professors_ProfessorAddedId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Universities_UniversityId",
                table: "Students");

            migrationBuilder.InsertData(
                table: "Universities",
                columns: new[] { "Id", "City", "Country", "Name" },
                values: new object[,]
                {
                    { 1, "Cambridge", "USA", "Massachusetts Institute of Technology (MIT)" },
                    { 2, "Stanford", "USA", "Stanford University" },
                    { 3, "London", "UK", "Imperial College London" }
                });

            migrationBuilder.InsertData(
                table: "Professors",
                columns: new[] { "Id", "Name", "UniversityId" },
                values: new object[,]
                {
                    { 1, "Klaus Mikaelson", 1 },
                    { 2, "Tim McGraw", 3 },
                    { 3, "Sophia Goldberg", 1 },
                    { 4, "Clara Bow", 2 }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Name", "ProfessorAddedId", "UniversityId" },
                values: new object[,]
                {
                    { 1, "Addison Montgomery", 4, 2 },
                    { 2, "Hanna Marin", 2, 3 },
                    { 3, "Mark Sloan", 1, 1 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Professors_ProfessorAddedId",
                table: "Students",
                column: "ProfessorAddedId",
                principalTable: "Professors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Universities_UniversityId",
                table: "Students",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Professors_ProfessorAddedId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Universities_UniversityId",
                table: "Students");

            migrationBuilder.DeleteData(
                table: "Professors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Professors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Professors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Professors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Universities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Universities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Universities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Professors_ProfessorAddedId",
                table: "Students",
                column: "ProfessorAddedId",
                principalTable: "Professors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Universities_UniversityId",
                table: "Students",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id");
        }
    }
}
