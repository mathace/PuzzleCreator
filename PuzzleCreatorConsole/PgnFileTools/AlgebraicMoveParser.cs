using PgnFileTools.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PgnFileTools
{
    public class AlgebraicMoveParser
    {
        private const string Capture = "capture";
        private const string Castle = "castle";
        private const string Check = "check";
        private const string DestinationFile = "destinationfile";
        private const string Destinationrow = "destinationrow";
        private const string DoubleCheck = "doublecheck";
        private const string EnPassant = "enpassant";
        private const string Mate = "mate";
        private const string Piece = "piece";
        private const string Promotionpiece = "promotionpiece";
        private const string SourceFile = "sourcefile";
        private const string SourceRow = "sourcerow";
        private const string Drop = "drop";
        private static readonly Regex _regex;

        static AlgebraicMoveParser()
        {
            _regex = new Regex(
                "^(" +
                "((?<" + Piece + ">[PNBRQK])?(?<" + Drop + ">@)?(?<" + DestinationFile + ">[a-h])(?<" + Destinationrow + ">[1-8]))|" +
                "((?<" + Piece + ">[NBRQK])?(?<" + SourceFile + ">[a-h])?(?<" + SourceRow + ">[1-8])?(?<" + Capture + ">x)?(?<" + DestinationFile + ">[a-h])(?<" + Destinationrow + ">[1-8]))|" +
                "(((?<" + Piece + ">[NBRQK])?(?<" + SourceFile + ">[a-h])?(?<" + SourceRow + ">[1-8])?(?<" + Capture + ">x)(?<" + DestinationFile + ">[a-h])(?<" + Destinationrow + ">[36]))(?<" + EnPassant + ">ep))|" +                
                "((?<" + SourceFile + ">[a-h])?(?<" + Capture + ">x)?(?<" + DestinationFile + ">[a-h])(?<" + Destinationrow + ">[2-7]))|" +
                "((?<" + SourceFile + ">[a-h])(?<" + Capture + ">x)(?<" + DestinationFile + ">[a-h])(?<" + Destinationrow + ">[36])(?<" + EnPassant + ">ep))|" +
                "((?<" + SourceFile + ">[a-h])?(?<" + Capture + ">x)?(?<" + DestinationFile + ">[a-h])(?<" + Destinationrow + @">[18])(?:\=?(?<" + Promotionpiece + ">[NBRQK])))|" +
                "(?<" + Castle + ">O(-?O){1,2})" + 
                ")" +
                "((?<" + Check + @">\+)(?<" + DoubleCheck + @">\+)?|(?<" + Mate + ">#))?$",
                RegexOptions.Compiled);
        }

        public Move Parse(string algebraicMove)
        {
            try
            {
                var match = _regex.Match(algebraicMove);

                var move = new Move();
                if (match.Length == 0)
                {
                    move.HasError = true;
                    move.ErrorMessage = "Failed to match expected structure";
                    return move;
                }

                move.CastleType = CastleType.GetFor(match.Groups[Castle].Value);
                move.IsCastle = move.CastleType != null;
                if (!move.IsCastle)
                {
                    move.IsDrop = !match.Groups[Drop].Value.IsNullOrEmpty();
                    move.PieceType = PieceType.GetFor(match.Groups[Piece].Value);
                    if (move.PieceType == null && move.IsDrop == true)
                    {
                        move.PieceType = PieceType.Pawn;
                    }
                    move.SourceFile = match.Groups[SourceFile].Value.Length == 0 ? null : File.GetFor(match.Groups[SourceFile].Value[0]);
                    move.SourceRow = match.Groups[SourceRow].Value.Length == 0 ? null : Row.GetFor(match.Groups[SourceRow].Value);
                    move.IsCapture = !match.Groups[Capture].Value.IsNullOrEmpty();                    
                    move.DestinationFile = match.Groups[DestinationFile].Value.Length == 0 ? null : File.GetFor(match.Groups[DestinationFile].Value[0]);
                    move.DestinationRow = Row.GetFor(match.Groups[Destinationrow].Value);
                    if (move.PieceType == PieceType.Pawn)
                    {
                        move.PromotionPiece = match.Groups[Promotionpiece].Value.Length == 0 ? null : PieceType.GetFor(match.Groups[Promotionpiece].Value[0]);
                        move.IsPromotion = move.PromotionPiece != null;
                    }
                    if (move.IsCapture)
                    {
                        move.IsEnPassantCapture = !match.Groups[EnPassant].Value.IsNullOrEmpty();
                    }
                    if (!move.PieceType.IsLegal(new Position
                    {
                        File = move.SourceFile,
                        Row = move.SourceRow
                    }, move.IsCapture, new Position
                    {
                        File = move.DestinationFile,
                        Row = move.DestinationRow
                    }))
                    {
                        move.HasError = true;
                        move.ErrorMessage = "The source and destination are illegal for this piece type";
                    }
                }

                move.IsCheck = !match.Groups[Check].Value.IsNullOrEmpty();
                if (move.IsCheck)
                {
                    move.IsDoubleCheck = !match.Groups[DoubleCheck].Value.IsNullOrEmpty();
                }
                else
                {
                    move.IsMate = !match.Groups[Mate].Value.IsNullOrEmpty();
                }
                return move;
            }
            catch (Exception)
            {

            }

            return null;
        }
    }
}
