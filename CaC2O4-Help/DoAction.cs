namespace CaC2O4_Help;

static class DoAction {
    internal delegate void ActionDelegate();

    internal static void Do(ActionDelegate action) {
        Console.Clear();
        action();
        Console.ReadKey();
        Console.Clear();
    }
}
