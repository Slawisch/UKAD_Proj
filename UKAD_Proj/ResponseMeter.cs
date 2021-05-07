using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace UKAD_Proj
{
    class ResponseMeter
    {
        private static Dictionary<string, int> urlTimings = new Dictionary<string, int>();

        public static Dictionary<string, int> GetUrlsTimings(List<string> urls)
        {
            Parallel.ForEach(urls, GetTiming);

            return urlTimings.OrderBy(u => u.Value)
                .ToDictionary(u => u.Key, u => u.Value);
        }

        private static void GetTiming(string url)
        {
            Stopwatch timer = new Stopwatch();
            HttpWebRequest request;
            timer.Start();

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                timer.Stop();
                urlTimings.Add(url, (int)timer.ElapsedMilliseconds);
            }
            catch
            {
                return;
            }

            timer.Stop();
        }
    }
}
