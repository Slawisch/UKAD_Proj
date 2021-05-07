
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace UKAD_Proj
{
    class HtmlWorker
    {
        private static readonly List<string> Urls = new();
        private static string _primaryUrl = string.Empty;

        public static List<string> GetUrls(string url)
        {
            IterateUrls(url);
            return Urls;
        }

        private static void IterateUrls(string url)
        {
            if (_primaryUrl == string.Empty) //If it's first method's call
                _primaryUrl = url;

            HtmlDocument doc;
            try
            {
                doc = new HtmlWeb().Load(url);
            }
            catch
            {
                return;
            }

            var linkedPages = doc.DocumentNode.Descendants("a")
                .Select(hrefNode => hrefNode.GetAttributeValue("href", null))
                .Union(doc.DocumentNode.Descendants("img")
                    .Select(imgNode => imgNode.GetAttributeValue("src", null)))
                .Select(node =>
                    { if (node != null && node.StartsWith('/')) node = _primaryUrl + node[1..]; return node; })
                .Where(node => !string.IsNullOrEmpty(node) && node.Contains(WebRequest.Create(_primaryUrl).RequestUri.Host) && !Urls.Contains(node)).Distinct()
                .ToList();

            Urls.AddRange(linkedPages.ToList());

            Parallel.ForEach(linkedPages, IterateUrls);
        }

        public static async Task<List<string>> GetUrlsAsync(string url)
        {
            return await Task.Run((() => GetUrls(url)));
        }

    }
}
