using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PuzzleCreatorConsole
{
    [Serializable]
    public enum PuzzleType {[XmlEnum("0")]Attack, [XmlEnum("1")]Mate }

    [Serializable]
    public enum Difficulty {[XmlEnum("0")]Easy, [XmlEnum("1")]Medium, [XmlEnum("1")]Hard }
    [Serializable]
    public class Puzzle
    {
        public PuzzleType PType { get; set; }
        public string Url { get; set; }
        public string Movelist { get; set; }
        public string Fen { get; set; }
        public AnalysisInfo Analysis{ get; set; }
        public Difficulty Diff { get; set; }

        public string WhiteName { get; set; }
        public string BlackName { get; set; }
        public int WhiteElo { get; set; }
        public int BlackElo { get; set; }

        public void Save()
        {
            var files = Directory.GetFiles("Puzzles").Select(x => Convert.ToInt32(Path.GetFileName(x))).ToList();
            int num = files.Count()==0 ? 0 : files.OrderByDescending(x => x).ToList()[0] + 1;
            File.WriteAllText("Puzzles\\" + num.ToString(), this.Serialize());
        }
        public static Difficulty GetDifficulty(int ply)
        {
            if (ply < 4) return Difficulty.Easy;
            if (ply < 7) return Difficulty.Medium;
            return Difficulty.Hard;
        }

    }
}
