using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thread = AskFm.DAL.Models.Thread;

namespace AskFm.DAL.ModelsConfigrations;

public class ThreadConfigration : IEntityTypeConfiguration<Thread>
{
    public void Configure(EntityTypeBuilder<Thread> builder)
    {
        builder.ToTable("Thread");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.QuestionContent)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(t => t.AnswerContent)
            .HasMaxLength(4000);

        builder.Property(t => t.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(t => t.isAnonymous)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .IsRequired();
        builder.HasOne(t => t.Asked)
            .WithMany(u => u.ReceivedThreads)
            .HasForeignKey(t => t.AskedId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(t => t.Asker)
            .WithMany(u => u.AskedThreads)
            .HasForeignKey(t => t.AskerId)
            .OnDelete(DeleteBehavior.ClientSetNull);




    }
}