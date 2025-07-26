using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AskFm.DAL.Moodels;

public class UserConfigration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.Name)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(u => u.Bio)
            .HasMaxLength(500);
        
        builder.Property(x => x.Username).IsRequired();

        builder.Property(x => x.Password)
            .HasMaxLength(225)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(255)
            .IsRequired();

    }
}