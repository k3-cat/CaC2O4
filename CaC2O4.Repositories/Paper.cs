using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaC2O4.Repositories;

public class Paper {
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public UInt32 SerialNo { get; set; }
    public String Title { get; set; } = null!;
    public FileState State { get; set; }
    public String Log { get; set; } = String.Empty;
    public DateTimeOffset Timestamp { get; set; }
}

public class PaperConfiguration : IEntityTypeConfiguration<Paper> {
    public void Configure(EntityTypeBuilder<Paper> builder) {
        builder
            .HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
