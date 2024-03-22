using System.Reflection;
using MassTransit;
using MassTransitEFCoreInterceptor.DbContext.Configuration;
using MassTransitEFCoreInterceptor.Domain;
using Microsoft.EntityFrameworkCore;

namespace MassTransitEFCoreInterceptor.DbContext;

public sealed class WriteDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Employee> Employees { get; set; }

    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
            x => x.GetInterfaces().Any(i => i == typeof(IWriteConfiguration)));

        base.OnModelCreating(builder);

        builder.AddOutboxStateEntity();
        builder.AddOutboxMessageEntity();
    }
}