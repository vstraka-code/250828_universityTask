using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _250828_universityTask.Migrations.TryOutDb
{
    /// <inheritdoc />
    public partial class Student2ManyToManyFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Students_Student2Id",
                table: "Professors");

            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Universities_UniversityId",
                table: "Professors");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Universities_UniversityId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Professors_Student2Id",
                table: "Professors");

            migrationBuilder.DropColumn(
                name: "Student2Id",
                table: "Professors");

            migrationBuilder.CreateTable(
                name: "ProfessorStudent2",
                columns: table => new
                {
                    ProfessorId = table.Column<int>(type: "int", nullable: false),
                    Student2Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessorStudent2", x => new { x.ProfessorId, x.Student2Id });
                    table.ForeignKey(
                        name: "FK_ProfessorStudent2_Professors_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessorStudent2_Students_Student2Id",
                        column: x => x.Student2Id,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                columns: new[] { "Id", "Email", "Name", "UniversityId" },
                values: new object[,]
                {
                    { 1, "klaus@example.com", "Klaus Mikaelson", 1 },
                    { 2, "tim@example.com", "Tim McGraw", 3 },
                    { 3, "sophia@example.com", "Sophia Goldberg", 1 },
                    { 4, "clara@example.com", "Clara Bow", 2 }
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

            migrationBuilder.InsertData(
                table: "ProfessorStudent2",
                columns: new[] { "ProfessorId", "Student2Id" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 1 },
                    { 4, 2 },
                    { 4, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessorStudent2_Student2Id",
                table: "ProfessorStudent2",
                column: "Student2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Universities_UniversityId",
                table: "Professors",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Universities_UniversityId",
                table: "Student",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Universities_UniversityId",
                table: "Professors");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Universities_UniversityId",
                table: "Student");

            migrationBuilder.DropTable(
                name: "ProfessorStudent2");

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

            migrationBuilder.AddColumn<int>(
                name: "Student2Id",
                table: "Professors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_Student2Id",
                table: "Professors",
                column: "Student2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Students_Student2Id",
                table: "Professors",
                column: "Student2Id",
                principalTable: "Students",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Universities_UniversityId",
                table: "Professors",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Universities_UniversityId",
                table: "Student",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id");
        }
    }
}
