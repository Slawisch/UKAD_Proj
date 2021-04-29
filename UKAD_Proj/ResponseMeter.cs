using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UKAD_Proj
{
    class ResponseMeter
    {
        public static Dictionary<string, int> GetUrlsTimings(List<string> urls)
        {
            HttpWebRequest request;
            Stopwatch timer = new Stopwatch();
            Dictionary<string, int> urlTimings = new Dictionary<string, int>();

            foreach (var item in urls)
            {
                
                timer.Reset();
                timer.Start();

                try
                {
                    request = (HttpWebRequest)WebRequest.Create(item);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse(); //EXCEPTION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    response.Close();
                    timer.Stop();
                    urlTimings.Add(item, (int) timer.ElapsedMilliseconds);
                }
                catch
                {
                    continue;
                }

                timer.Stop();
            }

            return urlTimings.OrderBy(u => u.Value)
                .ToDictionary(u => u.Key, u => u.Value);
        }
    }
}
