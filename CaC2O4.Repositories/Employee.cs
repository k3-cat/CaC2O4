using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaC2O4.Repositories;

public class Employee {
    public Guid Id { get; set; }
    public UInt32 LogicId { get; set; } // 职号
    public String Tin { get; set; } = null!; // 身份证号
    public String Name { get; set; } = null!;
    public String NameIndex { get; set; } = null!;
    public String Gender { get; set; } = null!;
    public String Ethnicity { get; set; } = null!; // 民族
    public DateOnly Dob { get; set; }
    public String Address { get; set; } = null!;
    public String Household { get; set; } = null!; // 户口所在地
    public String CertificateNo { get; set; } = null!; // 资格证号
    public DateOnly SCardExpireAt { get; set; } // 服务监督卡 小卡 有效期
    public UInt32 RecordNo { get; set; } // 备案证号

    public String Phone { get; set; } = null!;
    public String EducationLevel { get; set; } = null!;

    public Boolean IsRemoved { get; set; } = false;
    public String Log { get; set; } = null!;
    public DateTimeOffset Timestamp { get; set; }
}

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee> {
    public void Configure(EntityTypeBuilder<Employee> builder) {
        builder.Property(x => x.CertificateNo).HasColumnName("cert_no");
        builder.Property(x => x.SCardExpireAt).HasColumnName("scard_exp");
    }
}
