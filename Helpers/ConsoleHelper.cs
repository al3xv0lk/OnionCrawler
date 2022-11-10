using Spectre.Console;
using static Spectre.Console.AnsiConsole;



namespace OnionCrawler.Helpers;

public static class ConsoleHelper
{
    public static void WhiteSpace(int n = 1)
    {
        for (int i = 0; i <= n; i++)
        {
            System.Console.WriteLine();
        }
    }
    public static void ResultsRule()
    {
        WriteLine();

        var rule = new Rule("[purple]Resultados[/]");
        rule.Alignment = Justify.Left;

        Write(rule);

    }
}