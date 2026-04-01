using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Common.Model;

namespace TicketSystem.Feature.Employee.Model;

public class EmployeeConfiguration : IEntityTypeConfiguration<EmployeeProfile>
{
    public void Configure(EntityTypeBuilder<EmployeeProfile> builder)
    {
        builder.HasKey(x => x.EmployeeId);
        
        builder.HasOne(x => x.AppUser).WithOne(x => x.Employee).HasForeignKey<EmployeeProfile>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);

       
    
       builder.Property(x => x.Salary).IsRequired();
    }
}