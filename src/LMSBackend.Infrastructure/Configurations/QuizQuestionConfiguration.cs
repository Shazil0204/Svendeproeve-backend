using LMSBackend.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class QuizQuestionConfiguration : IEntityTypeConfiguration<QuizQuestion>
{
    public void Configure(EntityTypeBuilder<QuizQuestion> builder)
    {
        builder.HasKey(question => question.Id);
        builder.Property(question => question.QuestionText).HasMaxLength(2000).IsRequired();
        builder.HasOne(question => question.Quiz)
            .WithMany()
            .HasForeignKey(question => question.QuizId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
