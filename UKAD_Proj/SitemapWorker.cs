using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using RobotsTxt;

namespace UKAD_Proj
{
    class SitemapWorker
    {
        public delegate void SitemapHandler(string message);

        public static event SitemapHandler NoSitemapNotify;

        public static event SitemapHandler NoSitemapAccessNotify;

        public static event SitemapHandler PrintUrl;

        public static List<string> GetUrls(string url)
        {
            List<string> sitemapsAll = new List<string>();
            List<Sitemap> sitemapsRobots;
            List<string> urls = new List<string>();

            try
            {
                sitemapsRobots = GetSitemaps(url);
            }
            catch
            {
                NoSitemapNotify?.Invoke($"Sitemap for \"{url}\" not found");
                return urls;
            }

            foreach (var item in sitemapsRobots)
            {
                sitemapsAll.Add(item.Value);
            }

            urls = IterateSitemaps(sitemapsAll);
            return urls;
        }

        public static async Task<List<string>> GetUrlsAsync(string url)
        {
            return await Task.Run(() => GetUrls(url));
        }

        private static List<Sitemap> GetSitemaps(string url)
        {
            url += url[^1] != '/' ? "/robots.txt" : "robots.txt";

            Robots robots = Robots.Load(new WebClient().DownloadString(url));

            var mainRobotsSitemaps = robots.Sitemaps;

            return mainRobotsSitemaps;
        }

        private static List<string> IterateSitemaps(List<string> sitemaps)
        {
            if (sitemaps.Count == 0)
                return new List<string>();

            List<string> urls = new List<string>();
            List<XmlReader> xmlReaders = new List<XmlReader>();

            foreach (var item in sitemaps)
                xmlReaders.Add(new XmlTextReader(item));

            sitemaps.Clear();

            foreach (var itemReader in xmlReaders)
            {
                int nodeType = 0; //0 -Not initialized; 1 -Sitemap; 2 -Url;
                bool isLoc = false;
                try
                {
                    while (itemReader.Read())
                    {
                         switch (itemReader.NodeType)
                         {
                            case XmlNodeType.Element: // The node is an element.
                                switch (itemReader.Name)
                                {
                                    case "sitemap":
                                        nodeType = 1; //Next XmlNodeType.Text will be sitemap
                                        break;
                                    case "url":
                                        nodeType = 2; //Next XmlNodeType.Text will be url
                                        break;
                                    case "loc":
                                        isLoc = true; //Element is in <loc> tag
                                        break;
                                }
                                break;

                            case XmlNodeType.Text: //Display the text in each element.
                                switch (nodeType)
                                {
                                    case 1:
                                        if (isLoc)
                                            sitemaps.Add(itemReader.Value);
                                        break;
                                    case 2:
                                        if (isLoc)
                                        {
                                            urls.Add(itemReader.Value);
                                            PrintUrl?.Invoke(itemReader.Value);
                                        }

                                        break;
                                }
                                break;

                            case XmlNodeType.EndElement: //Display the end of the element.
                                switch (itemReader.Name)
                                {
                                    case "sitemap":
                                    case "url":
                                        nodeType = 0; //Elements end, value isn't expected
                                        break;
                                    case "loc": 
                                        isLoc = false; //End </loc> tag
                                        break;
                                }
                                break;
                        }
                    }
                }
                catch
                {
                    NoSitemapAccessNotify?.Invoke($"Can not access to sitemap: {itemReader.BaseURI}");
                }
            }

            urls.AddRange(IterateSitemaps(sitemaps));

            return urls;
        }

    }
}
