using LMSBackend.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class UserConsentConfiguration : IEntityTypeConfiguration<UserConsent>
{
    public void Configure(EntityTypeBuilder<UserConsent> builder)
    {
        builder.HasKey(consent => consent.Id);
        builder.Property(consent => consent.ConsentType).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(consent => consent.ConsentVersion).HasMaxLength(50).IsRequired();
        builder.HasIndex(consent => new { consent.UserId, consent.ConsentType }).IsUnique();
        builder.HasOne(consent => consent.User)
            .WithMany()
            .HasForeignKey(consent => consent.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
