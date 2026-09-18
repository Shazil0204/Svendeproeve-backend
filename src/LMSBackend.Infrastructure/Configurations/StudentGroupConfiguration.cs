using LMSBackend.Domain.Entities.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class StudentGroupConfiguration : IEntityTypeConfiguration<StudentGroup>
{
    public void Configure(EntityTypeBuilder<StudentGroup> builder)
    {
        builder.HasKey(group => group.Id);
        builder.Property(group => group.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(group => group.Name).IsUnique();
    }
}
