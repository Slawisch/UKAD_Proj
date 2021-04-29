
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace UKAD_Proj
{
    class HtmlWorker
    {
        private static List<string> _urls = new();
        private static string _primaryUrl = string.Empty;
        public delegate void HtmlHandler(string message);

        public static event HtmlHandler PrintUrl;

        public static List<string> GetUrls(string url)
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
                return new List<string>();
            }

            var linkedPages = doc.DocumentNode.Descendants("a")
                .Select(a => a.GetAttributeValue("href", null))
                .Union(doc.DocumentNode.Descendants("img")
                        .Select(a => a.GetAttributeValue("src", null)))
                .Where(u => !string.IsNullOrEmpty(u) && url.Contains(WebRequest.Create(_primaryUrl).RequestUri.Host) && !_urls.Contains(u)).Distinct()
                .ToList();

            //117

            _urls.AddRange(linkedPages.ToList());

            foreach (var item in linkedPages)
            {
                PrintUrl?.Invoke(item);
                GetUrls(item);
            }

            _urls = _urls.Select(l =>
                { if (l.StartsWith('/'))  l = _primaryUrl + l[1..]; return l; }).ToList();

            return _urls;
        }

        public static async Task<List<string>> GetUrlsAsync(string url)
        {
            return await Task.Run((() => GetUrls(url)));
        }

    }
}
