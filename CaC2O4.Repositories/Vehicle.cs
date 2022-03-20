using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaC2O4.Repositories;

public class Vehicle {
    public Guid Id { get; set; }
    public UInt32 LogicId { get; set; } // 门号
    public String NumberPlate { get; set; } = null!; // 车牌号
    public String Model { get; set; } = null!;
    public String Vin { get; set; } = null!;
    public String EngineNo { get; set; } = null!;
    public FuelType FuelType { get; set; }
    public DateOnly RegisterDate { get; set; } // 注册时间
    public String Usci { get; set; } = null!; // 统一社会信用代码
    public String LicenseNo { get; set; } = null!; // 经营许可证号 + 道路运输证号
    public String BusinessName { get; set; } = null!;
    public DateOnly TransportPermitExpireAt { get; set; } // 道路运输证有效期
    public DateOnly BusinessLicenseExpireAt { get; set; } // 经营许可证有效期
    public UInt32 RecordNo { get; set; } // 备案证号
    public Guid OwnerId { get; set; } = Guid.Empty;

    public Boolean IsRemoved { get; set; } = false;
    public String Log { get; set; } = null!;
    public DateTimeOffset Timestamp { get; set; }
}

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle> {
    public void Configure(EntityTypeBuilder<Vehicle> builder) {
        builder.Property(x => x.TransportPermitExpireAt).HasColumnName("t_permit_exp");
        builder.Property(x => x.BusinessLicenseExpireAt).HasColumnName("b_license_exp");
    }
}
