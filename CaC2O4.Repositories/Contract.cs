using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaC2O4.Repositories;

public class Contract {
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Guid EmployeeId { get; set; }

    public Boolean IsRemoved { get; set; } = false;
    public String Log { get; set; } = null!;
    public DateTimeOffset Timestamp { get; set; }
}

public class ContractConfiguration : IEntityTypeConfiguration<Contract> {
    public void Configure(EntityTypeBuilder<Contract> builder) {
        builder
            .HasIndex(x => new { x.VehicleId, x.EmployeeId })
            .IsUnique();

        builder
            .HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne<Employee>()
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
