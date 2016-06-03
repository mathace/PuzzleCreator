using MvbaCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgnFileTools.Constants
{
    public class SymbolicMoveAnnotation : NamedConstant<SymbolicMoveAnnotation>
    {
        public static readonly SymbolicMoveAnnotation GoodMove = new SymbolicMoveAnnotation("!", 1);
        public static readonly SymbolicMoveAnnotation PoorMove = new SymbolicMoveAnnotation("?", 2);
        public static readonly SymbolicMoveAnnotation QuestionableMove = new SymbolicMoveAnnotation("?!", 6);
        public static readonly SymbolicMoveAnnotation SpeculativeMove = new SymbolicMoveAnnotation("!?", 5);
        public static readonly SymbolicMoveAnnotation VeryGoodMove = new SymbolicMoveAnnotation("!!", 3);
        public static readonly SymbolicMoveAnnotation VeryPoorMove = new SymbolicMoveAnnotation("??", 4);

        private SymbolicMoveAnnotation(string symbol, int id)
        {
            Id = id;
            Add(symbol, this);
        }

        public int Id { get; private set; }
    }
}
