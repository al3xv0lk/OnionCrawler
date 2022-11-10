using HtmlAgilityPack;

namespace OnionCrawler.Helpers;

public static class HtmlDocumentHelper
{
    public static List<string> GetAllLinks(this HtmlDocument htmlDoc)
    {
        var tempUrls = new List<string>();
        try
        {
            // var links = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
            foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@href]"))
            {
                var endUrl = link.GetAttributeValue("href", string.Empty);
                // System.Console.WriteLine(endUrl);
                tempUrls.Add(endUrl);
            }

            System.Console.WriteLine(tempUrls.Count);
            return tempUrls;
        }
        catch (NullReferenceException)
        {
            System.Console.WriteLine("No results were found. Be aware that some terms are blocked by the Web Engine itself.");
            return tempUrls;
            // return default;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            return tempUrls;
            // return default;
        }
    }
}