using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicketSystem.Feature.Auth.Model ;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
       builder.HasKey(x => x.Id);

       builder.Property(x => x.UserId).IsRequired();

       builder.Property(x => x.ExpiresAtUtc).IsRequired();
    }
}