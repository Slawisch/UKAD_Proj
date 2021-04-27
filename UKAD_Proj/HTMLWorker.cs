using System.Collections.Generic;
using System.Linq;
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
                .Where(u => !string.IsNullOrEmpty(u) && u.StartsWith(_primaryUrl) && !_urls.Contains(u)).Distinct()
                .ToList();

            _urls.AddRange(linkedPages.ToList());

            foreach (var item in linkedPages)
            {
                PrintUrl?.Invoke(item);
                GetUrls(item);
            }

            return _urls;
        }

        public static async Task<List<string>> GetUrlsAsync(string url)
        {
            return await Task.Run((() => GetUrls(url)));
        }

    }
}
