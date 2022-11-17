using static OnionCrawler.Services.TorService;
using static OnionCrawler.Helpers.LinksHelper;


await LoadTor();
AskForLink();
while(true)
{
    await RunSearch();
    
}