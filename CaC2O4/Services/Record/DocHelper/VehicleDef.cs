namespace CaC2O4.Services.Record.DocHelper;

static class VehicleDef {
    public static String[] TitleList;

    public const String RegisterCertificate = "$机动车登记证书";
    public const String VehicleLicense = "$行驶证";
    public const String BusinessLicense = "$营业执照";
    public const String TransportPermit = "$道路运输证";
    public const String TransportBusinessLicense = "$运输经营许可证";
    public const String TaxiRecording = "$出租车备案证";
    public const String TaxClearanceCertificate = "$完税证明";
    public const String CompulsoryTrafficInsurance = "$交强险";
    public const String ThirdPartyLiabilityInsurance = "$三责险";
    public const String CarrierLiabilityInsurance = "$承运人责任险";

    static VehicleDef() {
        TitleList = DefHelper.BuildTitleList(typeof(VehicleDef));
    }
}
