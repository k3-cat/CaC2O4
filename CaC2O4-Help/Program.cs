using CaC2O4_Help;

var genPwd = new DoAction.ActionDelegate(Password.Generate);
var genKey = new DoAction.ActionDelegate(SignKey.Generate);

while (true) {
    Console.WriteLine("Hello, World!");

    var opt = Console.ReadKey();
    if (opt.Key == ConsoleKey.P) {
        DoAction.Do(genPwd);
    }
    else if (opt.Key == ConsoleKey.K) {
        DoAction.Do(genKey);
    }
}
