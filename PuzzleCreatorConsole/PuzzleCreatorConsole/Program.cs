using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GameCollector;

namespace PuzzleCreatorConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            SearchPuzzles("0h6gsqoS.g");
        }
        static void SearchPuzzles(string gamefile)
        {
            var games = File.ReadAllText(gamefile).Deserialize<List<LichessGame>>();
            foreach(var g in games)
            {
                var idx = games.IndexOf(g);
                Console.WriteLine("analyzing " + idx + " of " + games.Count() + " games");
                var ep = new EngineProcess();
                ep.AnalyzeGame(g);
            }

        }
    }
}
