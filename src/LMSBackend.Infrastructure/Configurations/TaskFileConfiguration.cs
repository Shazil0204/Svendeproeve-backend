using LMSBackend.Domain.Entities.Tasks;
using LMSBackend.Domain.ValueObjects.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class TaskFileConfiguration : IEntityTypeConfiguration<TaskFile>
{
    public void Configure(EntityTypeBuilder<TaskFile> builder)
    {
        builder.HasKey(file => file.Id);
        builder.Property(file => file.FileName)
            .HasConversion(name => name.Value, value => new FileName(value))
            .HasMaxLength(255).IsRequired();
        builder.Property(file => file.FilePath)
            .HasConversion(path => path.Value, value => new FilePath(value))
            .HasMaxLength(1000).IsRequired();
        builder.HasOne(file => file.Task)
            .WithMany()
            .HasForeignKey(file => file.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
