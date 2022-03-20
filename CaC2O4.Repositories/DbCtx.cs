using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CaC2O4.Repositories;

public class DbCtx : DbContext {
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Login> Logins => Set<Login>();
    public DbSet<Paper> Papers => Set<Paper>();
    public DbSet<UploadingRecord> UploadingRecords => Set<UploadingRecord>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    static DbCtx() {
        NpgsqlConnection.GlobalTypeMapper.MapEnum<FuelType>();
        NpgsqlConnection.GlobalTypeMapper.MapEnum<UserAcl>();
    }

    public DbCtx(DbContextOptions<DbCtx> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.HasPostgresEnum<UserAcl>();
        builder.HasPostgresEnum<FuelType>();
        builder.HasPostgresEnum<DocSubject>();
        builder.HasPostgresEnum<FileState>();

        builder.ApplyConfiguration(new ContractConfiguration());
        builder.ApplyConfiguration(new DocumentConfiguration());
        builder.ApplyConfiguration(new EmployeeConfiguration());
        builder.ApplyConfiguration(new LoginConfiguration());
        builder.ApplyConfiguration(new PaperConfiguration());
        builder.ApplyConfiguration(new UploadingRecordConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new VehicleConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder) {
        builder.UseSnakeCaseNamingConvention();
    }
}
