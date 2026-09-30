using LMSBackend.Domain.Entities.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasKey(feedback => feedback.Id);
        builder.Property(feedback => feedback.Content).HasMaxLength(5000).IsRequired();
        builder.HasOne(feedback => feedback.Submission)
            .WithMany()
            .HasForeignKey(feedback => feedback.SubmissionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(feedback => feedback.Teacher)
            .WithMany()
            .HasForeignKey(feedback => feedback.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
