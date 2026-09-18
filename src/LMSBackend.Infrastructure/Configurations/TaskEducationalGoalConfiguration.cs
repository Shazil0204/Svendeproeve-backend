using LMSBackend.Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class TaskEducationalGoalConfiguration : IEntityTypeConfiguration<TaskEducationalGoal>
{
    public void Configure(EntityTypeBuilder<TaskEducationalGoal> builder)
    {
        builder.HasKey(link => new { link.TaskId, link.EducationalGoalId });
        builder.HasOne(link => link.Task)
            .WithMany()
            .HasForeignKey(link => link.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(link => link.EducationalGoal)
            .WithMany()
            .HasForeignKey(link => link.EducationalGoalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
