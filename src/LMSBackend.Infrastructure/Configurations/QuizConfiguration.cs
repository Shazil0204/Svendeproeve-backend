using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Domain.ValueObjects.Progression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.HasKey(quiz => quiz.Id);
        builder.Property(quiz => quiz.Title).HasMaxLength(200).IsRequired();
        builder.Property(quiz => quiz.Description).HasMaxLength(5000).IsRequired();
        builder.Property(quiz => quiz.PassingPercentage)
            .HasConversion(percentage => percentage.Value, value => new Percentage(value))
            .HasPrecision(5, 2).IsRequired();
        builder.HasOne(quiz => quiz.Subject)
            .WithMany()
            .HasForeignKey(quiz => quiz.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(quiz => quiz.CreatedByUser)
            .WithMany()
            .HasForeignKey(quiz => quiz.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
