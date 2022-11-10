using HtmlAgilityPack;
using System.Text;
using System.Text.Json;
using RestSharp;
using Newtonsoft.Json.Linq;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using RestSharp.Serializers;
using System.Threading;

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

    // Get the Title
    public static string PageTitle(HtmlDocument htmlDoc)
    {
        var title = htmlDoc.DocumentNode.SelectSingleNode("//head/title").InnerText;
        return title.Length > 50 ? title[..50] : title;
    }

    // Get the las time the page was tested
    public static string PageLastTimeTested()
    {
        var lastTimeTested = DateTime.UtcNow.ToString("dd-MM-yyyy");
        return lastTimeTested;
    }

    // Gets all links in the page
    public static List<string> PageLinks(HtmlDocument htmlDoc)
    {
        List<string> pageLinks = new();
        var links = htmlDoc.DocumentNode.SelectNodes("//a[@href]")
                    .Where(node => node.InnerText
                    .Contains(".onion"));
        foreach (var link in links)
        {
            pageLinks.Add(link.GetAttributeValue("href", string.Empty));
        }
        return pageLinks;
    }

    // Get page content
    public static string GetPageContent(HtmlDocument htmlDoc)
    {
        return htmlDoc.DocumentNode.SelectSingleNode("//body").InnerText;
    }

    // Checks if page content has any suspicious words //! Experimental
    public static bool GetIsPotentialScam(HtmlDocument htmlDoc)
    {
        return GetPageContent(htmlDoc).Contains("Restricted words") ? true : false;
    }

    // Converts the website data found into json string
    public static string DataToJson(HtmlDocument htmlDoc, string url)
    {
        var model = new OpenSearchJsonModel
        {
            url = url,
            title = PageTitle(htmlDoc),
            links = PageLinks(htmlDoc).ToString(),
            // todo FIX
            content = GetPageContent(htmlDoc),
            potentialScammer = GetIsPotentialScam(htmlDoc),
            lastTimeTested = PageLastTimeTested()
        };
        string result = JsonConvert.SerializeObject(model);
        return result;

    }
    public static async Task SendJsonToDb(HtmlDocument htmlDoc, string url)
    {
        using (var client = new RestClient(API))
        {
            client.Authenticator = new HttpBasicAuthenticator(user, pass);
            byte[] urlAsBytes = Encoding.ASCII.GetBytes(url);
            var id = System.Convert.ToBase64String(urlAsBytes);
            var request = new RestRequest(id, Method.Put);
            request.AddStringBody(DataToJson(htmlDoc, url), ContentType.Json);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
        }
    }
}
