using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgnFileTools
{
    public class GameInfo
    {
        public GameInfo()
        {
            Headers = new Dictionary<string, string>();
            Moves = new List<Move>();
        }

        public string Comment { get; set; }

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public IDictionary<string, string> Headers { get; private set; }
        public IList<Move> Moves { get; private set; }
    }
}
