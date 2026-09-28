using LMSBackend.Domain.Entities.Quizzes;
using LMSBackend.Domain.ValueObjects.Progression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class QuizStudentConfiguration : IEntityTypeConfiguration<QuizStudent>
{
    public void Configure(EntityTypeBuilder<QuizStudent> builder)
    {
        builder.HasKey(attempt => attempt.Id);
        builder.HasIndex(attempt => new { attempt.QuizId, attempt.StudentId }).IsUnique();
        builder.Property(attempt => attempt.ScorePercentage)
            .HasConversion(percentage => percentage == null ? (decimal?)null : percentage.Value,
                value => value.HasValue ? new Percentage(value.Value) : null)
            .HasPrecision(5, 2);
        builder.Property(attempt => attempt.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasOne(attempt => attempt.Quiz)
            .WithMany(quiz => quiz.QuizStudents)
            .HasForeignKey(attempt => attempt.QuizId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(attempt => attempt.Student)
            .WithMany()
            .HasForeignKey(attempt => attempt.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
