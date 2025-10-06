using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _250828_universityTask.Migrations.TryOutDb
{
    /// <inheritdoc />
    public partial class student2modeladded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Professors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniversityId = table.Column<int>(type: "int", nullable: false),
                    Student2Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Professors_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniversityId = table.Column<int>(type: "int", nullable: true),
                    ProfessorAddedId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_Professors_ProfessorAddedId",
                        column: x => x.ProfessorAddedId,
                        principalTable: "Professors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniversityId = table.Column<int>(type: "int", nullable: true),
                    ProfessorAddedId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Professors_ProfessorAddedId",
                        column: x => x.ProfessorAddedId,
                        principalTable: "Professors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Professors_Student2Id",
                table: "Professors",
                column: "Student2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Professors_UniversityId",
                table: "Professors",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_ProfessorAddedId",
                table: "Student",
                column: "ProfessorAddedId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_UniversityId",
                table: "Student",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ProfessorAddedId",
                table: "Students",
                column: "ProfessorAddedId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UniversityId",
                table: "Students",
                column: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Students_Student2Id",
                table: "Professors",
                column: "Student2Id",
                principalTable: "Students",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Students_Student2Id",
                table: "Professors");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Professors");

            migrationBuilder.DropTable(
                name: "Universities");
        }
    }
}
