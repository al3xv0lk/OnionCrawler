namespace OnionCrawler.Helpers;
static class LinksHelper
{
    private static HashSet<string> currentLinks = new();
    private static HashSet<string> tempUrls = new();
    private static HashSet<string> urlsAnalized = new();
    private static HashSet<string> sitesOnline = new();
    public static void RemoveKnown(HashSet<string> links)
    {
        var result = links.Except(urlsAnalized);
        tempUrls.UnionWith(result);
    }
    public static void TempToCurrent()
    {
        currentLinks.Clear();
        currentLinks.UnionWith(tempUrls);
        tempUrls.Clear();
    }
    public static HashSet<string> GetCurrentLinks()
    {
        return currentLinks;
    }
    public static void AskForLink()
    {
        System.Console.WriteLine("Digite o link inicial: ");
        var link = System.Console.ReadLine();
        if(link != null)
        {
            currentLinks.Add(link);
        }
    }

    public static void AddToSitesOnline(string link)
    {
        sitesOnline.Add(link);
    }
}
