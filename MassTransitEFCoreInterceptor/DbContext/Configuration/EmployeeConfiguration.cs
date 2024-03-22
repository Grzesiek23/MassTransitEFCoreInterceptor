using MassTransitEFCoreInterceptor.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransitEFCoreInterceptor.DbContext.Configuration;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>, IWriteConfiguration
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.Property(e => e.Id);
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasMaxLength(64)
            .IsRequired();
    }
}