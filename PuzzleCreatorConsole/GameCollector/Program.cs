using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static GameCollector.LichessGames;

namespace GameCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            //DownloadGame("https://hu.lichess.org/uvTH7zmt/white");
            DownloadGames("games_mathace.gam");
            
        }
        static void DownloadAllUserGames(string username)
        {
            var results = new List<CurrentPageResult>();
            var nextpage = 1;
            while (nextpage != 0)
            {
                var games = LiWeb.GetGames(username, 100, nextpage);
                Console.WriteLine(nextpage + ". page is processing");
                nextpage = games.NextPage;
                results.AddRange(games.CurrentPageResults);
                File.WriteAllText("games_" + username + ".gam", results.Serialize());
                Thread.Sleep(4111);
                //var json = JsonConvert.SerializeObject(user);
            }
        }
        static LichessGame DownloadGame(string url)
        {            
            var game = LiWeb.GetGame(url);
            return game;
        }
        static void DownloadGames(string gamecollectionfilename)
        {
            var games = new List<LichessGame>();
            var urls = File.ReadAllLines("gameurls.txt").ToList();
            var gamelist = File.ReadAllText(gamecollectionfilename).Deserialize<List<CurrentPageResult>>();
            foreach (var g in gamelist)
            {
                if (g.Variant != "crazyhouse")
                {
                    Console.WriteLine("game already saved");
                    continue;
                }
                    var url = g.Url;
                    if (!urls.Contains(url))
                    {
                        urls.Add(url);
                        var game = DownloadGame(url);
                        games.Add(game);
                        Console.WriteLine(games.Count() + " games has been saved");
                        if (games.Count() % 50 == 0)
                        {
                        File.WriteAllText(g.Id + ".g", games.Serialize());
                        Thread.Sleep(65000);
                        }
                        Thread.Sleep(3500);
                    }
                    File.WriteAllLines("gameurls.txt", urls);                
            }
        }
    }
}
