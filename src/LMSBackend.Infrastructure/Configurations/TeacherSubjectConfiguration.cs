using LMSBackend.Domain.Entities.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSBackend.Infrastructure.Configurations;

public sealed class TeacherSubjectConfiguration : IEntityTypeConfiguration<TeacherSubject>
{
    public void Configure(EntityTypeBuilder<TeacherSubject> builder)
    {
        builder.HasKey(assignment => new { assignment.TeacherId, assignment.SubjectId });
        builder.HasOne(assignment => assignment.Teacher)
            .WithMany()
            .HasForeignKey(assignment => assignment.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(assignment => assignment.Subject)
            .WithMany()
            .HasForeignKey(assignment => assignment.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
