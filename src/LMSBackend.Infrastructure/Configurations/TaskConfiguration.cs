using LMSBackend.Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class TaskConfiguration : IEntityTypeConfiguration<LMSBackend.Domain.Entities.Tasks.Task>
{
    public void Configure(EntityTypeBuilder<LMSBackend.Domain.Entities.Tasks.Task> builder)
    {
        builder.HasKey(task => task.Id);
        builder.Property(task => task.Title).HasMaxLength(200).IsRequired();
        builder.Property(task => task.Description).HasMaxLength(5000).IsRequired();
        builder.HasOne(task => task.Subject)
            .WithMany()
            .HasForeignKey(task => task.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(task => task.CreatedByUser)
            .WithMany()
            .HasForeignKey(task => task.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
