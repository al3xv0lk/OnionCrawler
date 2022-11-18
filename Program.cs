using static OnionCrawler.Services.TorService;
using static OnionCrawler.Helpers.LinksHelper;
using static OnionCrawler.Helpers.ConsoleHelper;


await LoadTor();
WelcomeMsg();
while(true)
{
    AskForLink();
    while(GetCurrentLinks().Count > 0)
    {
        await RunSearch();
        TempToCurrent();
    }
    TotalResultsPanel();
    ExitMessage();
}
