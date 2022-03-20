using System.Text.Json;
using CaC2O4.Repositories;
using NPinyin;

namespace CaC2O4.Services.Record.DocHelper;

public class EmployeeHelper : IDocHelper {
    readonly JsonElement _records;

    public String Tin => _records.GetProperty("tin").GetString()!;
    public String Name => _records.GetProperty("name").GetString()!;
    public String NameIndex => Pinyin.GetInitials(Name);
    public String Gender => _records.GetProperty("ethnicity").GetString()!;
    public String Ethnicity => _records.GetProperty("ethnicity").GetString()!;
    public DateOnly Dob => DateOnly.FromDateTime(_records.GetProperty("dob").GetDateTime()!);
    public String Address => _records.GetProperty("address").GetString()!;
    public String Household => _records.GetProperty("household").GetString()!;
    public String CertificateNo => _records.GetProperty("certNo").GetString()!;
    public DateOnly SCardExpireAt => DateOnly.FromDateTime(_records.GetProperty("scardExp").GetDateTime()!);
    public UInt32 RecordNo => _records.GetProperty("recordNo").GetUInt32()!;

    public EmployeeHelper(String recordStr) {
        if (!String.IsNullOrWhiteSpace(recordStr)) {
            _records = JsonDocument.Parse(recordStr).RootElement;
        }
    }

    public String ProcessDoc(IDoc obj, String docTitle) {
        var employee = (Employee)obj;

        if (docTitle is EmployeeDef.IdCard) {
            employee.Tin = Tin;
            employee.Name = Name;
            employee.NameIndex = NameIndex;
            employee.Gender = Gender;
            employee.Ethnicity = Ethnicity;
            employee.Dob = Dob;
            employee.Address = Address;

            return $"{Tin},{Name},{NameIndex},{Gender},{Ethnicity},{Dob},{Address}";
        }
        if (docTitle is EmployeeDef.HouseholdRegister) {
            employee.Household = Household;

            return Household;
        }
        if (docTitle is EmployeeDef.QualificationCertificate) {
            employee.CertificateNo = CertificateNo;

            return CertificateNo;
        }
        if (docTitle is EmployeeDef.PrivateSCard) {
            employee.SCardExpireAt = SCardExpireAt;

            return SCardExpireAt.ToString();
        }
        if (docTitle is EmployeeDef.DriverRecord) {
            employee.RecordNo = RecordNo;

            return RecordNo.ToString();
        }

        return "null";
    }
}
