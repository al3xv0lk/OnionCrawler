using static OnionCrawler.Services.TorService;

// Begin loop, try to connect to tor, then the predefined run search with the results

await LoadTor();
await RunSearch();
