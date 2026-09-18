using LMSBackend.Domain.Entities.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class GroupMembershipConfiguration : IEntityTypeConfiguration<GroupMembership>
{
    public void Configure(EntityTypeBuilder<GroupMembership> builder)
    {
        builder.HasKey(membership => new { membership.GroupId, membership.StudentId });
        builder.HasOne(membership => membership.Group)
            .WithMany()
            .HasForeignKey(membership => membership.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(membership => membership.Student)
            .WithMany()
            .HasForeignKey(membership => membership.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
