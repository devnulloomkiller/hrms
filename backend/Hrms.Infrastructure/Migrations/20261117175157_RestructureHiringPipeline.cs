using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Hrms.Infrastructure.Migrations;

/// <inheritdoc />
public partial class _20261117175157_RestructureHiringPipeline : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Interviews_Resumes_ResumeId",
            table: "Interviews");

        migrationBuilder.DropForeignKey(
            name: "FK_Interviews_Vacancies_VacancyId",
            table: "Interviews");

        migrationBuilder.DropForeignKey(
            name: "FK_Resumes_Vacancies_VacancyId",
            table: "Resumes");

        // MySQL не даёт заменить обычный индекс на уникальный, пока на нём висит FK.
        migrationBuilder.DropForeignKey(
            name: "FK_Resumes_Candidates_CandidateId",
            table: "Resumes");

        // Старые собеседования ссылались на резюме и вакансию напрямую; перенести их
        // в новую модель без отклика нельзя, поэтому очищаем таблицу перед пересборкой связи.
        migrationBuilder.Sql("DELETE FROM `Interviews`;");
        migrationBuilder.Sql("DELETE r1 FROM `Resumes` r1 INNER JOIN `Resumes` r2 ON r1.`CandidateId` = r2.`CandidateId` AND r1.`Id` < r2.`Id`;");

        migrationBuilder.DropIndex(
            name: "IX_Resumes_CandidateId",
            table: "Resumes");

        migrationBuilder.DropIndex(
            name: "IX_Resumes_Status",
            table: "Resumes");

        migrationBuilder.DropIndex(
            name: "IX_Resumes_VacancyId",
            table: "Resumes");

        migrationBuilder.DropIndex(
            name: "IX_Interviews_ResumeId",
            table: "Interviews");

        migrationBuilder.DropColumn(
            name: "Status",
            table: "Resumes");

        migrationBuilder.DropColumn(
            name: "VacancyId",
            table: "Resumes");

        migrationBuilder.DropColumn(
            name: "ResumeId",
            table: "Interviews");

        migrationBuilder.RenameColumn(
            name: "SubmittedAt",
            table: "Resumes",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "VacancyId",
            table: "Interviews",
            newName: "ApplicationId");

        migrationBuilder.RenameIndex(
            name: "IX_Interviews_VacancyId",
            table: "Interviews",
            newName: "IX_Interviews_ApplicationId");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Vacancies",
            type: "varchar(4000)",
            maxLength: 4000,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext",
            oldNullable: true);

        migrationBuilder.AddColumn<int>(
            name: "MinExperience",
            table: "Vacancies",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "RequiredSkills",
            table: "Vacancies",
            type: "varchar(1000)",
            maxLength: 1000,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Resumes",
            type: "datetime(6)",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.Sql("UPDATE `Resumes` SET `CreatedAt` = `UpdatedAt` WHERE `CreatedAt` < '2000-01-01';");

        migrationBuilder.AddColumn<decimal>(
            name: "DesiredSalary",
            table: "Resumes",
            type: "decimal(12,2)",
            precision: 12,
            scale: 2,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Headline",
            table: "Resumes",
            type: "varchar(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AlterColumn<string>(
            name: "Position",
            table: "Interviewers",
            type: "varchar(200)",
            maxLength: 200,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Phone",
            table: "Candidates",
            type: "varchar(40)",
            maxLength: 40,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "longtext",
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "JobApplications",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                CandidateId = table.Column<int>(type: "int", nullable: false),
                VacancyId = table.Column<int>(type: "int", nullable: false),
                Stage = table.Column<int>(type: "int", nullable: false),
                AppliedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Source = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                Notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_JobApplications", x => x.Id);
                table.ForeignKey(
                    name: "FK_JobApplications_Candidates_CandidateId",
                    column: x => x.CandidateId,
                    principalTable: "Candidates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_JobApplications_Vacancies_VacancyId",
                    column: x => x.VacancyId,
                    principalTable: "Vacancies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "ResumeVacancyMatches",
            columns: table => new
            {
                ResumeId = table.Column<int>(type: "int", nullable: false),
                VacancyId = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                MatchScore = table.Column<int>(type: "int", nullable: true),
                MatchedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ResumeVacancyMatches", x => new { x.ResumeId, x.VacancyId });
                table.CheckConstraint("CK_Match_Score", "`MatchScore` IS NULL OR (`MatchScore` >= 0 AND `MatchScore` <= 100)");
                table.ForeignKey(
                    name: "FK_ResumeVacancyMatches_Resumes_ResumeId",
                    column: x => x.ResumeId,
                    principalTable: "Resumes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ResumeVacancyMatches_Vacancies_VacancyId",
                    column: x => x.VacancyId,
                    principalTable: "Vacancies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_Vacancies_Status",
            table: "Vacancies",
            column: "Status");

        migrationBuilder.AddCheckConstraint(
            name: "CK_Vacancy_MinExperience",
            table: "Vacancies",
            sql: "`MinExperience` >= 0");

        migrationBuilder.CreateIndex(
            name: "IX_Resumes_CandidateId",
            table: "Resumes",
            column: "CandidateId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Resumes_ExperienceYears",
            table: "Resumes",
            column: "ExperienceYears");

        migrationBuilder.AddCheckConstraint(
            name: "CK_Resume_Experience",
            table: "Resumes",
            sql: "`ExperienceYears` >= 0");

        migrationBuilder.CreateIndex(
            name: "IX_JobApplications_AppliedAt",
            table: "JobApplications",
            column: "AppliedAt");

        migrationBuilder.CreateIndex(
            name: "IX_JobApplications_CandidateId_VacancyId",
            table: "JobApplications",
            columns: new[] { "CandidateId", "VacancyId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_JobApplications_Stage",
            table: "JobApplications",
            column: "Stage");

        migrationBuilder.CreateIndex(
            name: "IX_JobApplications_VacancyId",
            table: "JobApplications",
            column: "VacancyId");

        migrationBuilder.CreateIndex(
            name: "IX_ResumeVacancyMatches_Status",
            table: "ResumeVacancyMatches",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_ResumeVacancyMatches_VacancyId",
            table: "ResumeVacancyMatches",
            column: "VacancyId");

        migrationBuilder.AddForeignKey(
            name: "FK_Interviews_JobApplications_ApplicationId",
            table: "Interviews",
            column: "ApplicationId",
            principalTable: "JobApplications",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Resumes_Candidates_CandidateId",
            table: "Resumes",
            column: "CandidateId",
            principalTable: "Candidates",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Interviews_JobApplications_ApplicationId",
            table: "Interviews");

        migrationBuilder.DropTable(
            name: "JobApplications");

        migrationBuilder.DropTable(
            name: "ResumeVacancyMatches");

        migrationBuilder.DropIndex(
            name: "IX_Vacancies_Status",
            table: "Vacancies");

        migrationBuilder.DropCheckConstraint(
            name: "CK_Vacancy_MinExperience",
            table: "Vacancies");

        migrationBuilder.DropIndex(
            name: "IX_Resumes_CandidateId",
            table: "Resumes");

        migrationBuilder.DropIndex(
            name: "IX_Resumes_ExperienceYears",
            table: "Resumes");

        migrationBuilder.DropCheckConstraint(
            name: "CK_Resume_Experience",
            table: "Resumes");

        migrationBuilder.DropColumn(
            name: "MinExperience",
            table: "Vacancies");

        migrationBuilder.DropColumn(
            name: "RequiredSkills",
            table: "Vacancies");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Resumes");

        migrationBuilder.DropColumn(
            name: "DesiredSalary",
            table: "Resumes");

        migrationBuilder.DropColumn(
            name: "Headline",
            table: "Resumes");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Resumes",
            newName: "SubmittedAt");

        migrationBuilder.RenameColumn(
            name: "ApplicationId",
            table: "Interviews",
            newName: "VacancyId");

        migrationBuilder.RenameIndex(
            name: "IX_Interviews_ApplicationId",
            table: "Interviews",
            newName: "IX_Interviews_VacancyId");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Vacancies",
            type: "longtext",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(4000)",
            oldMaxLength: 4000,
            oldNullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Status",
            table: "Resumes",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "VacancyId",
            table: "Resumes",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ResumeId",
            table: "Interviews",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AlterColumn<string>(
            name: "Position",
            table: "Interviewers",
            type: "longtext",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(200)",
            oldMaxLength: 200,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Phone",
            table: "Candidates",
            type: "longtext",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(40)",
            oldMaxLength: 40,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Resumes_CandidateId",
            table: "Resumes",
            column: "CandidateId");

        migrationBuilder.CreateIndex(
            name: "IX_Resumes_Status",
            table: "Resumes",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_Resumes_VacancyId",
            table: "Resumes",
            column: "VacancyId");

        migrationBuilder.CreateIndex(
            name: "IX_Interviews_ResumeId",
            table: "Interviews",
            column: "ResumeId");

        migrationBuilder.AddForeignKey(
            name: "FK_Interviews_Resumes_ResumeId",
            table: "Interviews",
            column: "ResumeId",
            principalTable: "Resumes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Interviews_Vacancies_VacancyId",
            table: "Interviews",
            column: "VacancyId",
            principalTable: "Vacancies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Resumes_Vacancies_VacancyId",
            table: "Resumes",
            column: "VacancyId",
            principalTable: "Vacancies",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }
}
