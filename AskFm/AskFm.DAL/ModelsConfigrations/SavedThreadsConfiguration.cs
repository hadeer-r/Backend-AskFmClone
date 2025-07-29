using AskFm.DAL.Moodels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AskFm.DAL.ModelsConfigrations;

public class SavedThreadsConfiguration : IEntityTypeConfiguration<SavedThreads>
{
    public void Configure(EntityTypeBuilder<SavedThreads> builder)
    {
        builder.HasKey(s=>new {s.SavedThreadId, s.UserId});
        
        builder.HasOne(s => s.Thread)
            .WithMany(t => t.SavedThreads)
            .HasForeignKey(s=>s.SavedThreadId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(s => s.User)
            .WithMany(u => u.SavedThreads)
            .HasForeignKey(s=>s.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
    }
}