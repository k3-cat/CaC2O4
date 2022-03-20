using System.Text.Json;
using CaC2O4.Repositories;

namespace CaC2O4.Services.Record.DocHelper;

public class VehicleHelper : IDocHelper {
    readonly JsonElement _records;

    public String Vin => _records.GetProperty("vin").GetString()!;
    public String EngineNo => _records.GetProperty("engineNo").GetString()!;
    public String Model => _records.GetProperty("model").GetString()!;
    public FuelType FuelType => Enum.Parse<FuelType>(_records.GetProperty("fuelType").GetString()!);
    public DateOnly RegisterDate => DateOnly.FromDateTime(_records.GetProperty("registerDate").GetDateTime()!);
    public String NumberPlate => _records.GetProperty("numberPlate").GetString()!;
    public String Usci => _records.GetProperty("usci").GetString()!;
    public DateOnly TransportPermitExpireAt => DateOnly.FromDateTime(_records.GetProperty("tPermitExp").GetDateTime()!);
    public String LicenseNo => _records.GetProperty("licenseNo").GetString()!;
    public String BusinessName => _records.GetProperty("businessName").GetString()!;
    public DateOnly BusinessLicenseExpireAt => DateOnly.FromDateTime(_records.GetProperty("bLicenseExp").GetDateTime()!);
    public UInt32 RecordNo => _records.GetProperty("recordNo").GetUInt32()!;

    public VehicleHelper(String recordStr) {
        if (!String.IsNullOrWhiteSpace(recordStr)) {
            _records = JsonDocument.Parse(recordStr).RootElement;
        }
    }

    public String ProcessDoc(IDoc obj, String docTitle) {
        var vehicle = (Vehicle)obj;

        if (docTitle is VehicleDef.RegisterCertificate) {
            vehicle.Vin = Vin;
            vehicle.EngineNo = EngineNo;
            vehicle.Model = Model;
            vehicle.FuelType = FuelType;
            vehicle.RegisterDate = RegisterDate;

            return $"{Vin},{EngineNo},{Model},{FuelType},{RegisterDate}";
        }
        if (docTitle is VehicleDef.VehicleLicense) {
            vehicle.NumberPlate = NumberPlate;

            return NumberPlate;
        }
        if (docTitle is VehicleDef.BusinessLicense) {
            vehicle.Usci = Usci;

            return Usci;
        }
        if (docTitle is VehicleDef.TransportPermit) {
            vehicle.TransportPermitExpireAt = TransportPermitExpireAt;

            return TransportPermitExpireAt.ToString();
        }
        if (docTitle is VehicleDef.TransportBusinessLicense) {
            vehicle.LicenseNo = LicenseNo;
            vehicle.BusinessName = BusinessName;
            vehicle.BusinessLicenseExpireAt = BusinessLicenseExpireAt;

            return $"{LicenseNo},{BusinessName},{BusinessLicenseExpireAt}";
        }
        if (docTitle is VehicleDef.TaxiRecording) {
            vehicle.RecordNo = RecordNo;

            return RecordNo.ToString();
        }

        return "null";
    }
}
