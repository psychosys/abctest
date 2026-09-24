using System.Globalization;
using Calculator;

var evaluator = new Evaluator();

// One-shot mode: dotnet run -- "2 + 3 * 4"
if (args.Length > 0)
{
    try
    {
        Console.WriteLine(Format(evaluator.Evaluate(string.Join(' ', args))));
        return 0;
    }
    catch (Exception ex) when (ex is FormatException or DivideByZeroException)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        return 1;
    }
}

// Interactive mode
Console.WriteLine("C# Calculator");
Console.WriteLine("Supports + - * / % ^ and parentheses. Type 'exit' to quit.");

while (true)
{
    Console.Write("> ");
    string? line = Console.ReadLine();
    if (line is null) break;

    line = line.Trim();
    if (line.Length == 0) continue;
    if (line.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
        line.Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

    try
    {
        Console.WriteLine(Format(evaluator.Evaluate(line)));
    }
    catch (Exception ex) when (ex is FormatException or DivideByZeroException)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

return 0;

static string Format(double value) => value.ToString("G15", CultureInfo.InvariantCulture);
