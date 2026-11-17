using Hrms.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<ResumeDetails> ResumeDetails => Set<ResumeDetails>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<ResumeVacancyMatch> ResumeVacancyMatches => Set<ResumeVacancyMatch>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<Interviewer> Interviewers => Set<Interviewer>();
    public DbSet<InterviewInterviewer> InterviewInterviewers => Set<InterviewInterviewer>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateOnly>()
            .HaveConversion<DateOnlyDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Vacancy>(e =>
        {
            e.Property(v => v.Title).HasMaxLength(200).IsRequired();
            e.Property(v => v.Department).HasMaxLength(100).IsRequired();
            e.Property(v => v.Description).HasMaxLength(4000);
            e.Property(v => v.RequiredSkills).HasMaxLength(1000);
            e.Property(v => v.PublishedAt).HasColumnType("date");
            e.Property(v => v.AcceptingResumesUntil).HasColumnType("date");
            e.Property(v => v.SalaryMin).HasPrecision(12, 2);
            e.Property(v => v.SalaryMax).HasPrecision(12, 2);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Vacancy_SalaryRange", "`SalaryMax` IS NULL OR `SalaryMax` >= `SalaryMin`");
                t.HasCheckConstraint("CK_Vacancy_MinExperience", "`MinExperience` >= 0");
            });
            e.HasIndex(v => v.Status);
        });

        builder.Entity<Candidate>(e =>
        {
            e.Property(c => c.LastName).HasMaxLength(100).IsRequired();
            e.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
            e.Property(c => c.Email).HasMaxLength(256).IsRequired();
            e.Property(c => c.Phone).HasMaxLength(40);
            e.HasIndex(c => c.Email).IsUnique();
        });

        builder.Entity<Resume>(e =>
        {
            e.Property(r => r.Headline).HasMaxLength(200).IsRequired();
            e.Property(r => r.Summary).HasMaxLength(2000).IsRequired();
            e.Property(r => r.DesiredSalary).HasPrecision(12, 2);
            e.HasOne(r => r.Candidate)
                .WithOne(c => c.Resume)
                .HasForeignKey<Resume>(r => r.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
            e.ToTable(t => t.HasCheckConstraint("CK_Resume_Experience", "`ExperienceYears` >= 0"));
            e.HasIndex(r => r.ExperienceYears);
        });

        builder.Entity<ResumeDetails>(e =>
        {
            e.HasKey(d => d.ResumeId);
            e.HasOne(d => d.Resume)
                .WithOne(r => r.Details)
                .HasForeignKey<ResumeDetails>(d => d.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(d => d.Education).HasMaxLength(500);
            e.Property(d => d.Skills).HasMaxLength(1000);
            e.Property(d => d.FilePath).HasMaxLength(1000);
        });

        builder.Entity<JobApplication>(e =>
        {
            e.HasIndex(a => new { a.CandidateId, a.VacancyId }).IsUnique();
            e.HasIndex(a => a.Stage);
            e.HasIndex(a => a.AppliedAt);
            e.Property(a => a.Source).HasMaxLength(100);
            e.Property(a => a.Notes).HasMaxLength(1000);
            e.HasOne(a => a.Candidate)
                .WithMany(c => c.Applications)
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Vacancy)
                .WithMany(v => v.Applications)
                .HasForeignKey(a => a.VacancyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ResumeVacancyMatch>(e =>
        {
            e.HasKey(m => new { m.ResumeId, m.VacancyId });
            e.HasIndex(m => m.Status);
            e.Property(m => m.Comment).HasMaxLength(500);
            e.HasOne(m => m.Resume)
                .WithMany(r => r.VacancyMatches)
                .HasForeignKey(m => m.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Vacancy)
                .WithMany(v => v.ResumeMatches)
                .HasForeignKey(m => m.VacancyId)
                .OnDelete(DeleteBehavior.Cascade);
            e.ToTable(t => t.HasCheckConstraint("CK_Match_Score", "`MatchScore` IS NULL OR (`MatchScore` >= 0 AND `MatchScore` <= 100)"));
        });

        builder.Entity<Interview>(e =>
        {
            e.HasIndex(i => i.ScheduledAt);
            e.HasOne(i => i.Application)
                .WithMany(a => a.Interviews)
                .HasForeignKey(i => i.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Interviewer>(e =>
        {
            e.Property(i => i.FullName).HasMaxLength(200).IsRequired();
            e.Property(i => i.Email).HasMaxLength(256);
            e.Property(i => i.Position).HasMaxLength(200);
            e.HasIndex(i => i.Email).IsUnique();
        });

        builder.Entity<InterviewInterviewer>(e =>
        {
            e.HasKey(ii => new { ii.InterviewId, ii.InterviewerId });
            e.Property(ii => ii.Role).HasMaxLength(50).IsRequired();
            e.HasOne(ii => ii.Interview)
                .WithMany(i => i.Participants)
                .HasForeignKey(ii => ii.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ii => ii.Interviewer)
                .WithMany(i => i.Interviews)
                .HasForeignKey(ii => ii.InterviewerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
