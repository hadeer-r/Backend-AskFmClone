using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AskFm.DAL.Moodels;

public class UserConfigration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(x => x.Username).IsRequired();

        builder.Property(x => x.Password)
            .HasMaxLength(225)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasMany(u => u.AskedThreads)
            .WithOne(t => t.Asker)
            .HasForeignKey(t => t.AskerId);

        builder.HasMany(u => u.ReceivedThreads)
            .WithOne(t => t.Asked)
            .HasForeignKey(t => t.AskedId);
    }
}