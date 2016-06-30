using GameCollector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PuzzleCreatorConsole
{
    public class EngineProcess
    {
        int pply = 0;
        Puzzle puzzle = null;
        StringBuilder outputBuilder;
        ProcessStartInfo processStartInfo;
        Process process;
        List<AnalysisInfo> infolist = new List<AnalysisInfo>();
        bool moveprocessed = false;
        bool comboalreadyfound = false;
        AnalysisInfo moveprocessresult = null;
        public void AnalyzeGame(LichessGame lg)
        {
            
            
           
            outputBuilder = new StringBuilder();

            processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.UseShellExecute = false;
            //processStartInfo.Arguments = "";
            processStartInfo.FileName = "Imortal v2.0.exe";

            process = new Process();
            process.StartInfo = processStartInfo;
            // enable raising events because Process does not raise events by default
            process.EnableRaisingEvents = true;
            // attach the event handler for OutputDataReceived before starting the process
            process.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {
                    // append the new data to the data already read-in
                    //outputBuilder.Append(e.Data);
                    if (e == null || e.Data == null) return;
                    if (e.Data.IndexOf("    ") > -1) ProcessData(e.Data);
                }
            );
            // start the process
            // then begin asynchronously reading the output
            // then wait for the process to exit
            // then cancel asynchronously reading the output
            
            process.Start();
            StreamWriter myStreamWriter = process.StandardInput;
            process.BeginOutputReadLine();
            myStreamWriter.WriteLine("xboard");
            myStreamWriter.WriteLine("protover 2");
            myStreamWriter.WriteLine("option MultiPV=2 1 300");
            myStreamWriter.WriteLine("post");
            myStreamWriter.WriteLine("new");
            myStreamWriter.WriteLine("analyze");

            //process.WaitForExit();
            //process.CancelOutputRead();

            var movelist = lg.Data.TreeParts.Skip(1).Select(tp => new { move = tp.Uci, fen = tp.Fen }).ToList();
            foreach (var m in movelist)
            {
                infolist.Clear();
                moveprocessed = false;
                if (comboalreadyfound==true)
                {
                    break;
                }
                var moveidx = movelist.IndexOf(m);
                Console.WriteLine("         move " + moveidx + " of " + movelist.Count() + " moves");
                if (moveidx == 22)
                {
                    ;
                }
                if (moveprocessresult != null)
                {
                    break;
                }
                    myStreamWriter.WriteLine(m.move);
                while (!moveprocessed)
                {
                    Thread.Sleep(100);
                }
                if (moveprocessresult!=null)
                {
                    //COMBO FOUND
                    var player1 = new Player()
                    {
                        Color = lg.Data.Player.Color,
                        Name = lg.Data.Player.User.Username,
                        Rating = lg.Data.Player.Rating
                    };
                    var player2 = new Player()
                    {
                        Color = lg.Data.Opponent.Color,
                        Name = lg.Data.Opponent.User.Username,
                        Rating = lg.Data.Opponent.Rating
                    };
                    var players = new List<Player>() { player1, player2 };
                    var whiteplayer = players.Single(p => p.Color == "white");
                    var blackplayer = players.Single(p => p.Color == "black");
                    puzzle = new Puzzle()
                    {
                        WhiteName = whiteplayer.Name,
                        WhiteElo = whiteplayer.Rating,
                        BlackName = blackplayer.Name,
                        BlackElo = blackplayer.Rating,
                        Fen = movelist[moveidx].fen,
                        Movelist = string.Join(";", moveprocessresult.moves),
                        PType = moveprocessresult.score > 1000 ? PuzzleType.Mate : PuzzleType.Attack,
                        Diff = Puzzle.GetDifficulty(pply)                    
                    };
                    puzzle.Save();
                    moveprocessresult = null;
                    break;
                }
                
            }
            myStreamWriter.WriteLine("quit");
            myStreamWriter.Close();
        }
        public void ProcessData(string data)
        {
            var items = data.ReduceWhitespace().Trim().Split(new char[] { ' ' }).ToList();
            int score = Convert.ToInt32(items[1]);
            int ply = Convert.ToInt32(items[0]);
            items.RemoveRange(0, 4);
            var pair = infolist.FirstOrDefault(i => i.ply == ply);
            if (pair!=null)
            {
                Console.WriteLine("                        score: " + pair.score + "     " + score + "  ply: " + ply);
                if (pair.score > 29000 && score < 29000)
                {
                    moveprocessresult = pair;
                    moveprocessed = true;
                    comboalreadyfound = true;
                    pply = ply;
                    return;
                }
                if (pair.score > 29000 && score > 29000)
                {
                    moveprocessed = true;
                    comboalreadyfound = true;
                    pply = ply;
                    return;
                }
                if (pair.ply > 7 && pair.score>500 && score>100 && score<pair.score/2)
                {
                    moveprocessresult = pair;
                    comboalreadyfound = true;
                    moveprocessed = true;
                    pply = ply;
                    return;
                }
                if (pair.ply == 7)
                {
                    moveprocessed = true;
                }
              
            }
            infolist.Add(new AnalysisInfo() { ply = ply, moves = items, score = score });
            return;
        }
    }
    public class AnalysisInfo
    {
        //public string gen { get; set; }
        public int ply { get; set; }
        public int score { get; set; }
        public List<string> moves { get; set; }
    }
    public class Player
    {
        public string Name { get; set; }
        public int Rating { get; set; }
        public string Color { get; set; }
    }
}
