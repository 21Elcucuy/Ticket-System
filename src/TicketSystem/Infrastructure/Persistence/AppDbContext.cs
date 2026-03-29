using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Common.Model;
using TicketSystem.Feature.Auth.Model;
using TicketSystem.Feature.Employee.Model;





namespace TicketSystem.Infrastructure.Persistence;

public sealed class AppDbContext(IHttpContextAccessor httpContextAccessor ,DbContextOptions options ) : IdentityDbContext<AppUser , IdentityRole<int> , int> (options)
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    // public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<EmployeeProfile> Employees => Set<EmployeeProfile>();
    public DbSet<Department> Departments => Set<Department>();





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       modelBuilder.Entity<Department>().HasData(new Department
       {
           DepartmentId =1 ,
           Name ="IT",
           CreatedAtUtc =DateTime.Parse("03/29/2026 22:49:07"),
           LastModifiedAtUtc =DateTime.Parse("03/29/2026 22:49:07"),
           IsActive =true,
           IsDeleted =false,
           
       });




        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach(var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if(typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter =Expression.Parameter(entityType.ClrType ,"x");  // like appuser 
                

                //Is Active ==true  Filter 
                var isActiveCheck = Expression.Equal(Expression.Property(parameter, nameof(IAuditableEntity.IsActive)),Expression.Constant(true)); // appuser.IsActive == true

                // if IsDeleted == false Filter 
                var isNotDeleted = Expression.Equal(Expression.Property(parameter, nameof(IAuditableEntity.IsDeleted)),Expression.Constant(false)); // appuser.IsActive 
                 
                          var baseFilter = Expression.AndAlso(isActiveCheck, isNotDeleted); 
            }

        }


    }



    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {

        var user = httpContextAccessor.HttpContext?.User;
        if(user is null)
        {
            throw new NullReferenceException();
        }
        var userIdClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        int ? userId = int.TryParse(userIdClaim , out var CurrrentuserId) ? CurrrentuserId : null;
        // if(userId is null)
        // {
        //     throw new NullReferenceException();
        // }
       foreach(var entries in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch(entries.State)
            {
                case EntityState.Added: 
                
                entries.Entity.CreatedAtUtc = DateTime.UtcNow;
                entries.Entity.CreatedBy = userId;
                entries.Entity.LastModifiedAtUtc = DateTime.UtcNow;
                entries.Entity.LastModifiedBy = userId;
                entries.Entity.IsActive = true;
                entries.Entity.IsDeleted = false;
                break;

                case EntityState.Modified:
                entries.Entity.LastModifiedAtUtc = DateTime.UtcNow;
                entries.Entity.LastModifiedBy = userId;
                 break;
                
                case EntityState.Deleted:

                entries.Entity.LastModifiedAtUtc = DateTime.UtcNow;
                entries.Entity.LastModifiedBy = userId;
                entries.Entity.IsActive = false;
                entries.Entity.IsDeleted = true;
                 break;
                 
            }
        }
 

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}

