namespace OnionCrawler.Helpers;
static class LinksHelper
{
    private static HashSet<string> currentLinks = new();
    private static HashSet<string> tempUrls = new();
    private static HashSet<string> urlsAnalized = new();
    private static HashSet<string> sitesOnline = new();

    public static int GetAllAnalized()
    {
        return urlsAnalized.Count;
    }
    public static int GetAllOnline()
    {
        return sitesOnline.Count;
    }

    public static void SaveUniqueToTemp(HashSet<string> links)
    {
        var result = links.Except(urlsAnalized);
        tempUrls.UnionWith(result);
        urlsAnalized.UnionWith(tempUrls);
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
        System.Console.Write("Digite o link inicial: ");
        // var link = System.Console.ReadLine();
        var link = "http://zqktlwiuavvvqqt4ybvgvi7tyo4hjl5xgfuvpdf6otjiycgwqbym2qad.onion/wiki/index.php/Main_Page";
        if(link != null)
        {
            currentLinks.Add(link);
        }
    }

    public static void AddToSitesOnline(string link)
    {
        sitesOnline.Add(link);
        // System.Console.WriteLine($"Total Online: {sitesOnline.Count} | Total Found: {urlsAnalized.Count}");
    }
}
