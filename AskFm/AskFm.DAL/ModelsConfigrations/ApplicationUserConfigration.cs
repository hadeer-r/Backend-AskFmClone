using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AskFm.DAL.Models;

public class ApplicationUserConfigration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(u => u.UserName).IsUnique();

        builder.Property(u => u.Name)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(u => u.Bio)
            .HasMaxLength(500);
        
        builder.Property(x => x.UserName).IsRequired();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(255)
            .IsRequired();

    }
}