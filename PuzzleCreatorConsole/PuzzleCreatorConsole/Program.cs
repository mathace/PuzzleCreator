using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PgnFileTools;
using System.IO;
using ZhTools;

namespace PuzzleCreatorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            /* 
             var pgnReader = new PgnStreamReader();
             var reader = File.OpenText("lichess1.pgn");
             var gameInfos = pgnReader.ReadToList(reader);
             //var zh = gameInfos.Where(gi => gi.Headers["Variant"] == "Crazyhouse").ToList();
             var err = gameInfos.Where(gi => gi.Headers.Count==0).ToList();
             var list = gameInfos.ToList<GameInfo>();
             var list2 = list;
             var m = gameInfos[0].Moves.Select(mv => mv.ToAlgebraicString()).ToList();           
             */

            var b = new Board();
            int x;
            var h = b.GenerateMoves(out x);
            b.MakeUserMove("e2e4");
            b.MakeUserMove("d7d5");
            var h2 = b.GenerateMoves(out x);
        }
    }
}
