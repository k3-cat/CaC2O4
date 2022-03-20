using PwdHasher;

namespace CaC2O4_Help;

static class Password {
    internal static void Generate() {
        Console.WriteLine("input a password: ");
        var pwd = Console.ReadLine();

        var hasher = new Argon2PwdHasher();
        Console.WriteLine(hasher.HashPassword(pwd!));
    }
}
