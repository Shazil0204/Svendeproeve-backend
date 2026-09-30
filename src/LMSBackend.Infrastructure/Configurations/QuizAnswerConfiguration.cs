using LMSBackend.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class QuizAnswerConfiguration : IEntityTypeConfiguration<QuizAnswer>
{
    public void Configure(EntityTypeBuilder<QuizAnswer> builder)
    {
        builder.HasKey(answer => answer.Id);
        builder.HasIndex(answer => new { answer.QuizStudentId, answer.QuizQuestionId }).IsUnique();
        builder.HasOne(answer => answer.QuizStudent)
            .WithMany()
            .HasForeignKey(answer => answer.QuizStudentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(answer => new
            {
                answer.QuizStudentId,
                answer.QuizQuestionId
            }).IsUnique();
        builder.HasOne(answer => answer.QuizQuestion)
            .WithMany()
            .HasForeignKey(answer => answer.QuizQuestionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(answer => answer.SelectedAnswerOption)
            .WithMany()
            .HasForeignKey(answer => answer.SelectedAnswerOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
