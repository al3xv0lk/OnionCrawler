namespace OnionCrawler.Helpers;
static class LinksHelper
{
    private static HashSet<string> currentLinks = new();
    private static HashSet<string> tempUrls = new();
    private static HashSet<string> urlsAnalized = new();
    private static HashSet<string> sitesOnline = new();
    public static void Analize()
    {
        foreach (var l in links)
        {
            if (urlsAnalized.Contains(l) == false)
            {
                tempUrls.Add(l);
            }
        };
    }
    public static HashSet<string> GetCurrentLinks()
    {
        return currentLinks;
    }
}
