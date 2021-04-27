using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace UKAD_Proj
{
    class Program
    {
        static void Main(string[] args)
        {
            string url;

            List<string> urlsFromSitemap;
            List<string> urlsFromPages;
            List<string> urlsTotal;

            Console.WriteLine("Input url in format: <https://www.site.com/> , or copy url from browser's query string"); 
            //For example: http://www.right-driving.zzz.com.ua/
            url = Console.ReadLine();

            Console.WriteLine("Please, wait for urls list...");

            SitemapWorker.NoSitemapNotify += Console.WriteLine;
            SitemapWorker.NoSitemapAccessNotify += Console.WriteLine;
            urlsFromSitemap = SitemapWorker.GetUrls(url);

            urlsFromPages = HtmlWorker.GetUrls(url);

            urlsTotal = urlsFromSitemap.Union(urlsFromPages).ToList();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Urls which exists in sitemap and doesn't on web site pages:\n");
            foreach (var item in urlsFromSitemap.Except(urlsFromPages))
            {
                Console.WriteLine(item);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Urls which exists on web site but doesn't in sitemap.xml:\n");
            foreach (var item in urlsFromPages.Except(urlsFromSitemap))
            {
                Console.WriteLine(item);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please, wait for timings list...");

            foreach (var item in ResponseMeter.GetUrlsTimings(urlsTotal))
            {
                Console.WriteLine(item.Key + "\n" + item.Value + " ns");
            }

            Console.WriteLine($"Urls found after crawling a website:{urlsFromPages.Count}");
            Console.WriteLine($"Urls found in sitemap:{urlsFromSitemap.Count}");

            Console.WriteLine("Press Enter to exit.");
            Console.ReadKey();
        }

    }
}