using AskFm.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AskFm.DAL.ModelsConfigrations;

public class CommentLikeConfigration : IEntityTypeConfiguration<CommentLike>
{
    public void Configure(EntityTypeBuilder<CommentLike> builder)
    {
        builder.HasKey(cl => new { cl.UserId, cl.CommentId });

        builder.Property(cl => cl.CreatedAt)
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(cl => cl.User)
            .WithMany(u => u.CommentLikes)
            .HasForeignKey(cl => cl.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(cl => cl.Comment)
            .WithMany(c => c.CommentLikes)
            .HasForeignKey(cl => cl.CommentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}