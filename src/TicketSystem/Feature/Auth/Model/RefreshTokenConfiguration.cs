using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicketSystem.Feature.Auth.Model ;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenProfile>
{
    public void Configure(EntityTypeBuilder<RefreshTokenProfile> builder)
    {
       builder.HasKey(x => x.Id);

       builder.Property(x => x.UserId).IsRequired();

       builder.Property(x => x.ExpiresAtUtc).IsRequired();
       
       builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}