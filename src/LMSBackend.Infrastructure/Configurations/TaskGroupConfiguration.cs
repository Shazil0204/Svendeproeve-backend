using LMSBackend.Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class TaskGroupConfiguration : IEntityTypeConfiguration<TaskGroup>
{
    public void Configure(EntityTypeBuilder<TaskGroup> builder)
    {
        builder.HasKey(assignment => assignment.Id);
        builder.Property(assignment => assignment.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasIndex(assignment => new { assignment.TaskId, assignment.GroupId }).IsUnique();
        builder.HasOne(assignment => assignment.Task)
            .WithMany()
            .HasForeignKey(assignment => assignment.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(assignment => assignment.Group)
            .WithMany()
            .HasForeignKey(assignment => assignment.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
