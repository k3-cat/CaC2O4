namespace CaC2O4.Services.Record.DocHelper;

static class EmployeeDef {
    public static String[] TitleList;

    public const String IdCard = "$身份证";
    public const String HouseholdRegister = "$户口本";
    public const String DrivingLicense = "$驾照";
    public const String QualificationCertificate = "$从业资格证";
    public const String PublicSCard = "$大卡";
    public const String PrivateSCard = "$小卡";
    public const String DriverRecord = "$驾驶员备案证明";
    public const String ProofOfResidence = "$居住证明";
    public const String VaccinationCertificate = "$疫苗接种证明";

    static EmployeeDef() {
        TitleList = DefHelper.BuildTitleList(typeof(EmployeeDef));
    }
}
