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
    public static void TotalResultsPanel()
    {
        WhiteSpace();
        var panel = new Panel(new BreakdownChart()
            .Width(75)
            .AddItem("Links online", LinksHelper.GetAllOnline(), Color.Purple)
            .AddItem("Links analizados", LinksHelper.GetAllAnalized(), Color.White))
        {
            Header = new PanelHeader("Total", Justify.Center),
            Padding = new Padding(1),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Purple)
        };
        Write(panel);
        WhiteSpace();
    }
    public static void CreateTable(string title, string body)
    {
        var table = new Table()
        {
            Width = 110,
            Border = TableBorder.Rounded,
            BorderStyle = new Style(Color.Purple)
        };

        table.AddColumn(new TableColumn(new Markup($"[bold]{title}[/]")));
        table.AddRow(new Markup($"[link]{body}[/]"));

        Write(table);
    }
    public static void ExitMessage()
    {
        WhiteSpace();
        System.Console.WriteLine(" A pesquisa foi concluída, pressione CTRL + C para sair...");
        WhiteSpace();
    }
     public static void WelcomeMsg()
    {
        Write(new FigletText("OnionCrawler").Alignment(Justify.Left).Color(Color.Purple));

        WhiteSpace();

        var panel = new Panel("Um crawler assíncrono para a rede Tor(Deep Web)." +
        "\nDigite um link inicial, o crawler vai encontrar todos os links online vinculados de alguma forma a ele." + 
        "\n[bold]Atenção:[/] 99% dos sites que pedem algum tipo de pagamento, são golpes." + 
        "\n[bold]Lembre-se[/]: [italic]você é o único responsável por suas ações.[/]")
        {
            Header = new PanelHeader("Bem-vindo(a)!", Justify.Center),
            Padding = new Padding(1),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Purple)
        };

        Write(panel);
        WhiteSpace(3);
    }
}