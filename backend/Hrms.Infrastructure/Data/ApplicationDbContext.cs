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
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<Interviewer> Interviewers => Set<Interviewer>();
    public DbSet<InterviewInterviewer> InterviewInterviewers => Set<InterviewInterviewer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Vacancy>(e =>
        {
            e.Property(v => v.Title).HasMaxLength(200).IsRequired();
            e.Property(v => v.Department).HasMaxLength(100).IsRequired();
            e.Property(v => v.SalaryMin).HasPrecision(12, 2);
            e.Property(v => v.SalaryMax).HasPrecision(12, 2);
            e.ToTable(t => t.HasCheckConstraint("CK_Vacancy_SalaryRange", "`SalaryMax` IS NULL OR `SalaryMax` >= `SalaryMin`"));
        });

        builder.Entity<Candidate>(e =>
        {
            e.Property(c => c.LastName).HasMaxLength(100).IsRequired();
            e.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
            e.Property(c => c.Email).HasMaxLength(256).IsRequired();
            e.HasIndex(c => c.Email).IsUnique();
        });

        builder.Entity<Resume>(e =>
        {
            e.Property(r => r.Summary).HasMaxLength(2000).IsRequired();
            e.HasOne(r => r.Candidate)
                .WithMany(c => c.Resumes)
                .HasForeignKey(r => r.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(r => r.Vacancy)
                .WithMany()
                .HasForeignKey(r => r.VacancyId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(r => r.Status);
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

        builder.Entity<Interview>(e =>
        {
            e.Property(i => i.ScheduledAt);
            e.HasOne(i => i.Resume)
                .WithMany(r => r.Interviews)
                .HasForeignKey(i => i.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(i => i.Vacancy)
                .WithMany(v => v.Interviews)
                .HasForeignKey(i => i.VacancyId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(i => i.ScheduledAt);
        });

        builder.Entity<Interviewer>(e =>
        {
            e.Property(i => i.FullName).HasMaxLength(200).IsRequired();
            e.Property(i => i.Email).HasMaxLength(256);
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
