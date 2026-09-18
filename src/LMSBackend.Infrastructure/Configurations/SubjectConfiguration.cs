using LMSBackend.Domain.Entities.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(subject => subject.Id);
        builder.Property(subject => subject.Name).HasMaxLength(200).IsRequired();
        builder.HasOne(subject => subject.User)
            .WithMany()
            .HasForeignKey(subject => subject.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
