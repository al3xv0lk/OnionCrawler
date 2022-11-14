using static OnionCrawler.Services.TorService;


await LoadTor();
await RunInitialSearch();
