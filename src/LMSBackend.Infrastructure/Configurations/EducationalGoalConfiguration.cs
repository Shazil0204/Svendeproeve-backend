using LMSBackend.Domain.Entities.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class EducationalGoalConfiguration : IEntityTypeConfiguration<EducationalGoal>
{
    public void Configure(EntityTypeBuilder<EducationalGoal> builder)
    {
        builder.HasKey(goal => goal.Id);
        builder.Property(goal => goal.Content).HasMaxLength(2000).IsRequired();
        builder.HasOne(goal => goal.Subject)
            .WithMany()
            .HasForeignKey(goal => goal.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
