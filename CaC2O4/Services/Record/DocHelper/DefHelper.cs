namespace CaC2O4.Services.Record.DocHelper;

static class DefHelper {
    internal static String[] BuildTitleList(Type type) {
        var emptyDoc = new List<String>();

        var fields = type.GetFields();
        foreach (var field in fields) {
            if (field.FieldType != typeof(String)) {
                continue;
            }
            emptyDoc.Add($"{field.GetValue(null)!}");
        }

        return emptyDoc.ToArray();
    }
}
