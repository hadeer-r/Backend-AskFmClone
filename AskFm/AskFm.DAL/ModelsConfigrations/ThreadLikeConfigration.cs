using AskFm.DAL.Moodels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AskFm.DAL.ModelsConfigrations;

public class ThreadLikeConfigration : IEntityTypeConfiguration<ThreadLike>
{
    public void Configure(EntityTypeBuilder<ThreadLike> builder)
    {
        builder.HasKey(tl => new { tl.ThreadId, tl.UserId });

        builder.Property(ql => ql.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(tl => tl.User)
            .WithMany(u => u.QuestionLikes)
            .HasForeignKey(tl => tl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tl => tl.Thread)
            .WithMany(t => t.ThreadLikes)
            .HasForeignKey(tl => tl.ThreadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}