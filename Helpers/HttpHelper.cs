using HtmlAgilityPack;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using RestSharp.Serializers;

namespace OnionCrawler.Helpers;

public static class HttpHelper
{
    private static StreamReader ab = new StreamReader("login");
    private static string user = ab.ReadLine();
    private static string pass = ab.ReadLine();
    private static string API = ab.ReadLine();

    public static async Task<byte[]> DownloadFileAsync(this HttpClient httpClient, string uri, string path)
    {
        var response = await httpClient.GetAsync(uri);

        using var fileStream = new FileStream(path, FileMode.CreateNew);

        await response.Content.CopyToAsync(fileStream);

        return await response.Content.ReadAsByteArrayAsync();
    }

    public static async Task<HtmlDocument> LoadHtmlDocument(this HttpClient httpClient, string uri)
    {
        var htmlDocument = new HtmlDocument();

        htmlDocument.LoadHtml(await httpClient.GetStringAsync(uri));

        return htmlDocument;
    }

    public static string PageTitle(HtmlDocument htmlDoc)
    {
        var title = htmlDoc.DocumentNode.SelectSingleNode("//head/title").InnerText;
        return title.Length > 50 ? title[..50] : title;
    }

    public static string PageLastTimeTested()
    {
        var lastTimeTested = DateTime.UtcNow.ToString("dd-MM-yyyy");
        return lastTimeTested;
    }

    public static HashSet<string> PageLinks(HtmlDocument htmlDoc)
    {
        HashSet<string> pageLinks = new();
        var links = htmlDoc.DocumentNode.SelectNodes("//a[@href]")
                    .Where(node => node.GetAttributeValue("href", string.Empty)
                    .Contains(".onion"));
        foreach (var link in links)
        {
            pageLinks.Add(link.GetAttributeValue("href", string.Empty));
        }
        return pageLinks;
    }

    public static string GetAllUniqueWordsInPage(HtmlDocument htmlDoc)
    {
        return string.Join(" ", htmlDoc.DocumentNode.SelectSingleNode("//body").InnerText.Split(' ').Distinct());
    }

    // Checks if page content has any suspicious words //! Experimental
    public static bool GetIsPotentialScam(HtmlDocument htmlDoc)
    {
        return GetAllUniqueWordsInPage(htmlDoc).Contains("Restricted words") ? true : false;
    }
    public static string LinkListToString(HashSet<string> pageLinks)
    {
        var linksString = string.Join(", ", pageLinks);
        return linksString;
    }

    public static string HtmlContentToJson(HtmlDocument htmlDoc, string url)
    {
        var model = new OpenSearchJsonModel
        {
            url = url,
            title = PageTitle(htmlDoc),
            links = LinkListToString(PageLinks(htmlDoc)),
            content = GetAllUniqueWordsInPage(htmlDoc),
            potentialScammer = GetIsPotentialScam(htmlDoc),
            lastTimeTested = PageLastTimeTested()
        };
        string result = JsonConvert.SerializeObject(model);
        
        return result;
    }
    public static async Task UploadJsonToOpenSearch(HtmlDocument htmlDoc, string url)
    {
        using (var client = new RestClient(API))
        {
            client.Authenticator = new HttpBasicAuthenticator(user, pass);
            byte[] urlAsBytes = Encoding.ASCII.GetBytes(url);
            var id = System.Convert.ToBase64String(urlAsBytes);
            var request = new RestRequest(id, Method.Put);
            request.AddStringBody(HtmlContentToJson(htmlDoc, url), ContentType.Json);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
        }
    }
}
