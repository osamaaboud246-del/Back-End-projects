using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortalConsole.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorCourseRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {1
            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Instructors_InstructorId",
                table: "Courses",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Instructors_InstructorId",
                table: "Courses");
        }
    }
}
