
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
    private static HttpClient _httpClient = new HttpClient(new SocketsHttpHandler()
    {
        Proxy = new System.Net.WebProxy("socks5://127.0.0.1:9050"),
        UseProxy = true
    });


    public static async Task LoadTor()
    {
        if (isTorProcessOn())
        {
            await ConnectTorProxy();
        };
    }

    public static async Task RunSearch()
    {
        var links = LinksHelper.GetCurrentLinks();
        await TestUrls(links);
    }



    private static bool isTorProcessOn()
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

    private static async Task<string> ConnectTorProxy()
    {
        var value = string.Empty;

        while (true)
        {
            try
            {
                await AnsiStatusAsync("Conectando ao proxy...", async ctx =>
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

    private static async Task TestUrls(HashSet<string> urls)
    {
        try
        {
            ResultsRule();

            var tester = new ActionBlock<string>(async url =>
            {
                var htmlDoc = await _httpClient.LoadHtmlDocument(url);

                var title = HttpHelper.PageTitle(htmlDoc);

                var links = HttpHelper.PageLinks(htmlDoc);

                LinksHelper.SaveUniqueToTemp(links);

                // await HttpHelper.UploadJsonToOpenSearch(htmlDoc, url);

                CreateTable(title, url);

                LinksHelper.AddToSitesOnline(url);
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 100 });

            Parallel.ForEach(urls, (url) => tester.SendAsync(url));

            tester.Complete();
            tester.Completion.Wait();
        }
        catch (System.Exception) { }
    }


}