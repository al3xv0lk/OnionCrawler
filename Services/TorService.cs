using Spectre.Console;
using System.Diagnostics;
using OnionCrawler.Helpers;
using System.Threading.Tasks.Dataflow;

using static Spectre.Console.AnsiConsole;
using static OnionCrawler.Helpers.AnsiConsoleHelper;
using static OnionCrawler.Helpers.ConsoleHelper;
using System.Runtime.InteropServices;
using System.Net.Http.Json;


namespace OnionCrawler.Services;

public static class TorService
{
    private static string webEngine = "http://juhanurmihxlp77nkq76byazcldy2hlmovfu2epvl5ankdibsot4csyd.onion/search/?q=";

    private static List<string> tempUrls = new();
    private static List<string> initialUrls = new();
    private static List<string> sitesOnline = new();
    private static HttpClient unProxiedClient = new();
    private static HttpClient _httpClient = new HttpClient(new SocketsHttpHandler()
    {
        Proxy = new System.Net.WebProxy("socks5://127.0.0.1:9050"),
        UseProxy = true
    });


    public static async Task LoadTor()
    {
        // TODO Verify if tor is running

        if (TestTorProccess())
        {
            await TestProxy();
        };
    }

    public static async Task RunInitialSearch()
    {
        await AnsiStatusAsync("Buscando links...", async ctx =>
        {
            // string initialLink = "http://xsglq2kdl72b2wmtn5b2b7lodjmemnmcct37owlz5inrhzvyfdnryqid.onion";
            string initialLink = "http://jaz45aabn5vkemy4jkg4mi4syheisqn2wn2n4fsuitpccdackjwxplad.onion/";
            // TODO Get the initial links to crawl from the
            var resultPage = await _httpClient.LoadHtmlDocument(initialLink);
            initialUrls = resultPage.GetAllLinks();
        });

        if (initialUrls.Count > 0)
        {
            MarkupLine($"Links encontrados: [bold][purple]{initialUrls.Count}[/][/]");
            while(true)
            {
                System.Console.WriteLine($"Initial: {initialUrls.Count}");
                await TestUrls(initialUrls);
                initialUrls.Clear();
                System.Console.WriteLine($"Current initial urls: {initialUrls.Count}");
                System.Console.WriteLine("Adding range");
                initialUrls.AddRange(tempUrls);
                tempUrls.Clear();
            }
        }

        else
        {
            System.Console.WriteLine();
            MarkupLine($"Nenhum resultado foi encontrado.");
        }
        WhiteSpace();
        TotalResultsPanel(sitesOnline, initialUrls);
        WhiteSpace();
        System.Console.WriteLine($"Total new links found: {tempUrls.Count}");
        
        // AnsiChart(sitesOnline.Count, "Links online", tempUrls.Count, "Links encontrados");

        tempUrls.Clear();
        sitesOnline.Clear();
    }

    private static bool TestTorProccess()
    {
        if (Process.GetProcessesByName("tor").Length == 0)
        {
            try
            {
                AnsiStatus("Iniciando Tor Browser...", ctx =>
                {
                    // TODO Start Tor process here
                    // Process.Start(new ProcessStartInfo { FileName = torPath, Arguments = "--detach", UseShellExecute = true });
                    // Thread.Sleep(5000);
                    return Task.CompletedTask;
                });
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        return true;
    }

    private static async Task SetupTor()
    {
        WriteLine();

        await AnsiStatusAsync("Baixando e configurando o Tor...", async ctx =>
        {
            var htmlDoc = await _httpClient.LoadHtmlDocument("https://www.torproject.org/download/");

            var downloadLink = string.Empty;

            var downloadDoc = htmlDoc.DocumentNode.SelectNodes("//a")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("btn btn-primary mt-4 downloadLink"));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                foreach (var node in downloadDoc)
                {
                    if (node.GetAttributeValue("href", "").EndsWith(".tar.xz"))
                    {
                        downloadLink = node.GetAttributeValue("href", "");
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                foreach (var node in downloadDoc)
                {
                    if (node.GetAttributeValue("href", "").EndsWith(".exe"))
                    {
                        downloadLink = node.GetAttributeValue("href", "");
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                foreach (var node in downloadDoc)
                {
                    if (node.GetAttributeValue("href", "").EndsWith(".dmg"))
                    {
                        downloadLink = node.GetAttributeValue("href", "");
                    }
                }
            }
            // TODO Any installation related stuff must be called here
            // await Configure.InstallAsync(_httpClient, torRoot, downloadLink);
        });
    }

    static async Task SendDataToDb(HttpClient httpClient, string url, StringContent jsonContent)
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic b25pb25maW5kZXI6T24xb25maW5kZXIh");

        var apiPut = "https://search-onionfinder-giusor5h6brlyi4sicbljtouhy.us-east-1.es.amazonaws.com/onionsites/_doc/";
        using HttpResponseMessage response = await httpClient.PutAsync(
            apiPut + url, jsonContent);

        System.Console.WriteLine(response.EnsureSuccessStatusCode());

        var jsonResponse = await response.Content.ReadAsStringAsync();
        WriteLine($"{jsonResponse}\n");
    }
    private static async Task<string> TestProxy()
    {
        var value = string.Empty;

        while (true)
        {
            try
            {
                await AnsiStatusAsync("Testando a conexão com o proxy...", async ctx =>
                {
                    value = await _httpClient.GetStringAsync("http://duckduckgo.com");
                });

                MarkupLine(" Proxy Tor conectado :check_mark:");
                break;
            }
            catch (Exception)
            {
                Thread.Sleep(100);
            }
        };

        return value;
    }

    private static async Task TestUrls(List<string> urls)
    {
        try
        {
            ResultsRule();

            // await AnsiStatusAsync("Testando links...", ctx =>
            // {
            var tester = new ActionBlock<string>(async url =>
            {
                var htmlDoc = await _httpClient.LoadHtmlDocument(url);

                var title = HttpHelper.PageTitle(htmlDoc);

                var links = HttpHelper.PageLinks(htmlDoc);
                tempUrls.AddRange(links);
                

                await HttpHelper.SendJsonToDb(htmlDoc, url);

                // await SendDataToDb(unProxiedClient, url, jsonContent);
                CreateTable(title, url);
                
                sitesOnline.Add(url);
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 100 });

            Parallel.ForEach(urls, (url) => tester.SendAsync(url));

            tester.Complete();
            tester.Completion.Wait();

            //     return Task.CompletedTask;
            // });
        }
        catch (System.Exception) { }
    }
}