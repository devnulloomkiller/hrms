using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Hrms.Infrastructure.Migrations;

/// <inheritdoc />
public partial class _20261116182558_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new
            {
                Id = table.Column<string>(type: "varchar(255)", nullable: false),
                Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new
            {
                Id = table.Column<string>(type: "varchar(255)", nullable: false),
                DisplayName = table.Column<string>(type: "longtext", nullable: true),
                UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUsers", x => x.Id);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Candidates",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                Phone = table.Column<string>(type: "longtext", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Candidates", x => x.Id);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Interviewers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                FullName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                Position = table.Column<string>(type: "longtext", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Interviewers", x => x.Id);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Vacancies",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                Department = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "longtext", nullable: true),
                SalaryMin = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                SalaryMax = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
                PublishedAt = table.Column<DateOnly>(type: "date", nullable: false),
                AcceptingResumesUntil = table.Column<DateOnly>(type: "date", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vacancies", x => x.Id);
                table.CheckConstraint("CK_Vacancy_SalaryRange", "`SalaryMax` IS NULL OR `SalaryMax` >= `SalaryMin`");
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                RoleId = table.Column<string>(type: "varchar(255)", nullable: false),
                ClaimType = table.Column<string>(type: "longtext", nullable: true),
                ClaimValue = table.Column<string>(type: "longtext", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "AspNetUserClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                ClaimType = table.Column<string>(type: "longtext", nullable: true),
                ClaimValue = table.Column<string>(type: "longtext", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "AspNetUserLogins",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                UserId = table.Column<string>(type: "varchar(255)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "AspNetUserRoles",
            columns: table => new
            {
                UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "AspNetUserTokens",
            columns: table => new
            {
                UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                Name = table.Column<string>(type: "varchar(255)", nullable: false),
                Value = table.Column<string>(type: "longtext", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Resumes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                CandidateId = table.Column<int>(type: "int", nullable: false),
                VacancyId = table.Column<int>(type: "int", nullable: true),
                Summary = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                ExperienceYears = table.Column<int>(type: "int", nullable: false),
                SubmittedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Resumes", x => x.Id);
                table.ForeignKey(
                    name: "FK_Resumes_Candidates_CandidateId",
                    column: x => x.CandidateId,
                    principalTable: "Candidates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Resumes_Vacancies_VacancyId",
                    column: x => x.VacancyId,
                    principalTable: "Vacancies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Interviews",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                ResumeId = table.Column<int>(type: "int", nullable: false),
                VacancyId = table.Column<int>(type: "int", nullable: false),
                ScheduledAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Mode = table.Column<int>(type: "int", nullable: false),
                Result = table.Column<int>(type: "int", nullable: false),
                Notes = table.Column<string>(type: "longtext", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Interviews", x => x.Id);
                table.ForeignKey(
                    name: "FK_Interviews_Resumes_ResumeId",
                    column: x => x.ResumeId,
                    principalTable: "Resumes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Interviews_Vacancies_VacancyId",
                    column: x => x.VacancyId,
                    principalTable: "Vacancies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "ResumeDetails",
            columns: table => new
            {
                ResumeId = table.Column<int>(type: "int", nullable: false),
                Education = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                Skills = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                FilePath = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
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
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "InterviewInterviewers",
            columns: table => new
            {
                InterviewId = table.Column<int>(type: "int", nullable: false),
                InterviewerId = table.Column<int>(type: "int", nullable: false),
                Role = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
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
            })
            .Annotation("MySQL:Charset", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetRoleClaims_RoleId",
            table: "AspNetRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            table: "AspNetRoles",
            column: "NormalizedName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserClaims_UserId",
            table: "AspNetUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserLogins_UserId",
            table: "AspNetUserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserRoles_RoleId",
            table: "AspNetUserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "AspNetUsers",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Candidates_Email",
            table: "Candidates",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Interviewers_Email",
            table: "Interviewers",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_InterviewInterviewers_InterviewerId",
            table: "InterviewInterviewers",
            column: "InterviewerId");

        migrationBuilder.CreateIndex(
            name: "IX_Interviews_ResumeId",
            table: "Interviews",
            column: "ResumeId");

        migrationBuilder.CreateIndex(
            name: "IX_Interviews_ScheduledAt",
            table: "Interviews",
            column: "ScheduledAt");

        migrationBuilder.CreateIndex(
            name: "IX_Interviews_VacancyId",
            table: "Interviews",
            column: "VacancyId");

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
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AspNetRoleClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserLogins");

        migrationBuilder.DropTable(
            name: "AspNetUserRoles");

        migrationBuilder.DropTable(
            name: "AspNetUserTokens");

        migrationBuilder.DropTable(
            name: "InterviewInterviewers");

        migrationBuilder.DropTable(
            name: "ResumeDetails");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "AspNetUsers");

        migrationBuilder.DropTable(
            name: "Interviewers");

        migrationBuilder.DropTable(
            name: "Interviews");

        migrationBuilder.DropTable(
            name: "Resumes");

        migrationBuilder.DropTable(
            name: "Candidates");

        migrationBuilder.DropTable(
            name: "Vacancies");
    }
}
