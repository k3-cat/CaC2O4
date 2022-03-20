namespace CaC2O4.Types;

public static class DateSerilzer {
    public static Date Serilize(DateOnly date) {
        var zipedDate = date.Day & 0b11111;
        zipedDate += (date.Month & 0b1111) << 5;
        zipedDate += date.Year << (5 + 4);

        return new Date {
            ZipedDate = zipedDate
        };
    }

    public static DateOnly Deserilize(Date date) {
        var year = date.ZipedDate >> (5 + 4);
        var month = (date.ZipedDate >> 5) & 0b1111;
        var day = date.ZipedDate & 0b11111;

        return new DateOnly(year, month, day);
    }
}
