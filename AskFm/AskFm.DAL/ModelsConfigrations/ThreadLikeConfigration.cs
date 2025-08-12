using AskFm.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AskFm.DAL.ModelsConfigrations;

public class ThreadLikeConfigration : IEntityTypeConfiguration<ThreadLike>
{
    public void Configure(EntityTypeBuilder<ThreadLike> builder)
    {
        builder.HasKey(tl => new { tl.ThreadId, tl.UserId });
        
        builder.HasOne(tl => tl.User)
            .WithMany(u => u.ThreadLikes)
            .HasForeignKey(tl => tl.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(tl => tl.Thread)
            .WithMany(t => t.ThreadLikes)
            .HasForeignKey(tl => tl.ThreadId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}