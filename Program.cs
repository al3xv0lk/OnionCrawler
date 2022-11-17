using static OnionCrawler.Services.TorService;
using static OnionCrawler.Helpers.LinksHelper;


await LoadTor();
while(true)
{
    AskForLink();
    await RunSearch();
}
