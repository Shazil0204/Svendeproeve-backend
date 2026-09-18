using LMSBackend.Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class TaskStudentConfiguration : IEntityTypeConfiguration<TaskStudent>
{
    public void Configure(EntityTypeBuilder<TaskStudent> builder)
    {
        builder.HasKey(assignment => assignment.Id);
        builder.Property(assignment => assignment.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasIndex(assignment => new { assignment.TaskId, assignment.StudentId }).IsUnique();
        builder.HasOne(assignment => assignment.Task)
            .WithMany()
            .HasForeignKey(assignment => assignment.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(assignment => assignment.Student)
            .WithMany()
            .HasForeignKey(assignment => assignment.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
