using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicketSystem.Feature.Teams.Models;

public class TeamConfiguration : IEntityTypeConfiguration<TeamProfile>
{
    public void Configure(EntityTypeBuilder<TeamProfile> builder)
    {
        builder.HasKey(x => x.TeamId);
        builder.HasOne(x => x.Employee).WithOne().HasForeignKey<TeamProfile>(x => x.ManagerId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);
    }
}