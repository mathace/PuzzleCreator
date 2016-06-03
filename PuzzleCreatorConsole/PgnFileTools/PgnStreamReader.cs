using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgnFileTools
{
    public class PgnStreamReader
    {
        public IEnumerable<GameInfo> Read(TextReader reader)
        {
            var parser = new GameInfoParser();
            while (reader.Peek() != -1)
            {
                var game = parser.Parse(reader);
                yield return game;
            }
        }
        public List<GameInfo> ReadToList(TextReader reader)
        {
            var gameinfos = new List<GameInfo>();
            var parser = new GameInfoParser();
            while (reader.Peek() != -1)
            {
                var game = parser.Parse(reader);
                gameinfos.Add(game);
            }
            return gameinfos;
        }
    }
}
