using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Common.Model.Department;

namespace TicketSystem.Feature.Employee.Model;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<DepartmentProfile>
{
    public void Configure(EntityTypeBuilder<DepartmentProfile> builder)
    {
        builder.HasKey(x=> x.DepartmentId);

        builder.Property(x => x.Name).IsRequired();

       builder.Property(x => x.Location).IsRequired();
    }
}