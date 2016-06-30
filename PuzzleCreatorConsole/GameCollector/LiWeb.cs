using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GameCollector
{
    public static class LiWeb
    {
        // Returns JSON string
        public static string GET(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                var httpresponse = (HttpWebResponse)response;
                if ((int) httpresponse.StatusCode == 429)
                {
                    return "429";
                } 
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        public static LichessUser GetUser(string username)
        {
            var json = GET("https://hu.lichess.org/api/user/" + username);
            return JsonConvert.DeserializeObject<LichessUser>(json);
        }
        public static LichessGames GetGames(string username, int nb, int page)
        {
            var json = GET("https://hu.lichess.org/api/user/" + username + "/games?nb=" + nb + "&page=" + page);
            if (json=="429")
            {
                Thread.Sleep(70000);
                json = GET("https://hu.lichess.org/api/user/" + username + "/games?nb=" + nb + "&page=" + page);
            }
            json = json.Replace("\"nextPage\":null", "\"nextPage\":0");
            return JsonConvert.DeserializeObject<LichessGames>(json);
        }
        public static LichessGame GetGame(string url)
        {
            LichessGame lgame;
            using (WebClient wc = new WebClient())
            {
                var input = wc.DownloadString(url);
                var idx1 = input.IndexOf("lichess.analyse = {");
                var idx2 = input.IndexOf(@"</script>", idx1);
                var json = input.Substring(idx1, idx2 - idx1 - 2).Replace("lichess.analyse = ","");
                lgame = JsonConvert.DeserializeObject<LichessGame>(json); 
            }
            return lgame;
        }
    }
}
