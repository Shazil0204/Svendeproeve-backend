using LMSBackend.Domain.Entities.Submissions;
using LMSBackend.Domain.ValueObjects.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class SubmissionFileConfiguration : IEntityTypeConfiguration<SubmissionFile>
{
    public void Configure(EntityTypeBuilder<SubmissionFile> builder)
    {
        builder.HasKey(file => file.Id);
        builder.Property(file => file.FileName)
            .HasConversion(name => name.Value, value => new FileName(value))
            .HasMaxLength(255).IsRequired();
        builder.Property(file => file.FilePath)
            .HasConversion(path => path.Value, value => new FilePath(value))
            .HasMaxLength(1000).IsRequired();
        builder.HasOne(file => file.Submission)
            .WithMany()
            .HasForeignKey(file => file.SubmissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
