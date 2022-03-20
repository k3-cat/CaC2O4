namespace CaC2O4.Services.Record.DocHelper;

public interface IDoc {
    String Log { get; set; }
    DateTimeOffset Timestamp { get; set; }
}

public interface IDocHelper {
    String ProcessDoc(IDoc obj, String docTitle);
}
