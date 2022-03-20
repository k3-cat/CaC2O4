using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaC2O4.Repositories;

public class UploadingRecord {
    public String Key { get; set; } = null!;
    public Guid Creator { get; set; }
    public Boolean IsSuccess { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? SuccessedAt { get; set; }
}

public class UploadingRecordConfiguration : IEntityTypeConfiguration<UploadingRecord> {
    public void Configure(EntityTypeBuilder<UploadingRecord> builder) {
        builder.HasKey(x => x.Key);

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.Creator)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
