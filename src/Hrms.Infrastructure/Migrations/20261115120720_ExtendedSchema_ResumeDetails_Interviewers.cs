using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrms.Infrastructure.Migrations;

/// <inheritdoc />
public partial class _20261115120720_ExtendedSchema_ResumeDetails_Interviewers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Education",
            table: "Resumes");

        migrationBuilder.DropColumn(
            name: "FilePath",
            table: "Resumes");

        migrationBuilder.DropColumn(
            name: "Skills",
            table: "Resumes");

        migrationBuilder.DropColumn(
            name: "InterviewerName",
            table: "Interviews");

        migrationBuilder.CreateTable(
            name: "Interviewers",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                Position = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Interviewers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ResumeDetails",
            columns: table => new
            {
                ResumeId = table.Column<int>(type: "INTEGER", nullable: false),
                Education = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                Skills = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                FilePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ResumeDetails", x => x.ResumeId);
                table.ForeignKey(
                    name: "FK_ResumeDetails_Resumes_ResumeId",
                    column: x => x.ResumeId,
                    principalTable: "Resumes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "InterviewInterviewers",
            columns: table => new
            {
                InterviewId = table.Column<int>(type: "INTEGER", nullable: false),
                InterviewerId = table.Column<int>(type: "INTEGER", nullable: false),
                Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InterviewInterviewers", x => new { x.InterviewId, x.InterviewerId });
                table.ForeignKey(
                    name: "FK_InterviewInterviewers_Interviewers_InterviewerId",
                    column: x => x.InterviewerId,
                    principalTable: "Interviewers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_InterviewInterviewers_Interviews_InterviewId",
                    column: x => x.InterviewId,
                    principalTable: "Interviews",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Interviewers_Email",
            table: "Interviewers",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_InterviewInterviewers_InterviewerId",
            table: "InterviewInterviewers",
            column: "InterviewerId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InterviewInterviewers");

        migrationBuilder.DropTable(
            name: "ResumeDetails");

        migrationBuilder.DropTable(
            name: "Interviewers");

        migrationBuilder.AddColumn<string>(
            name: "Education",
            table: "Resumes",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FilePath",
            table: "Resumes",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Skills",
            table: "Resumes",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InterviewerName",
            table: "Interviews",
            type: "TEXT",
            maxLength: 200,
            nullable: false,
            defaultValue: "");
    }
}
