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
    public static void TotalResultsPanel(List<string> sitesOnline, List<string> tempUrls)
    {
        var panel = new Panel(new BreakdownChart()
            .Width(75)
            .AddItem("Links online", sitesOnline.Count, Color.Purple)
            .AddItem("Links encontrados", tempUrls.Count, Color.White))
        {
            Header = new PanelHeader("Total", Justify.Center),
            Padding = new Padding(1),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Purple)
        };
        Write(panel);
    }
}