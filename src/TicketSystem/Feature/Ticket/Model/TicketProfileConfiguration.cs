using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicketSystem.Feature.Ticket.Model;

public class TicketProfileConfiguration : IEntityTypeConfiguration<TicketProfile>
{
    public void Configure(EntityTypeBuilder<TicketProfile> builder)
    {
        builder.HasKey(x => x.TicketId);
        
        builder.HasOne(x => x.EmployeeProfile).WithOne(x => x.Ticket).HasForeignKey<TicketProfile>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade); 
        
        builder.Property(x => x.FromEmail).IsRequired();
        builder.Property(x => x.TicketSubject).IsRequired();
    }
}