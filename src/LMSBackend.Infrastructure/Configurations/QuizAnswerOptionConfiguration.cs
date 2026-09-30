using LMSBackend.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class QuizAnswerOptionConfiguration : IEntityTypeConfiguration<QuizAnswerOption>
{
    public void Configure(EntityTypeBuilder<QuizAnswerOption> builder)
    {
        builder.HasKey(option => option.Id);
        builder.Property(option => option.AnswerText).HasMaxLength(1000).IsRequired();
        builder.HasOne(option => option.QuizQuestion)
            .WithMany()
            .HasForeignKey(option => option.QuizQuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
