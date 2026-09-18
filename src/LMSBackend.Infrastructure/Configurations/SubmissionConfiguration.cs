using LMSBackend.Domain.Entities.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasKey(submission => submission.Id);
        builder.Property(submission => submission.Comment).HasMaxLength(5000);
        builder.HasOne(submission => submission.TaskStudent)
            .WithMany()
            .HasForeignKey(submission => submission.TaskStudentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(submission => submission.TaskGroup)
            .WithMany()
            .HasForeignKey(submission => submission.TaskGroupId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(submission => submission.SubmittedByUser)
            .WithMany()
            .HasForeignKey(submission => submission.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
