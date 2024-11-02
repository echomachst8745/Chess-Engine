using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chess_Engine
{
    public class Board
    {
        // FEN string for the starting position.
        public const string DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public byte[] Squares { get { return _squares; } }
        private byte[] _squares;

        private bool isWhiteMove;

        /// <summary>
        /// A byte that stores the castle rights of both sides.
        /// For example, 0b00001111 means both colours can castle on both sides.
        /// 
        /// Represents:
        /// 0b0000<White King Side><White Queen Side><Black King Side><Black Queen Side>.
        /// </summary>
        private byte castleRightBits;

        // The index of the square that a pawn can move to for en passant.
        private int enPassantTargetIndex;

        // Half moves since a pawn advance or piece capture.
        private int halfMoves;
        // Full moves since the start.
        private int fullMoves;

        public Board(string fenString = DefaultFEN)
        {
            string[] fenStringParts = fenString.Split();

            // Setup squares
            _squares = SquaresFromFEN(fenStringParts[0]);

            // Set current move colour
            isWhiteMove = fenStringParts[1] == "w";

            // Set castling rights
            castleRightBits = CastlingRightsFromFEN(fenStringParts[2]);

            // Set en passant target
            enPassantTargetIndex = fenStringParts[3] != "-" ? FileRankStringToIndex(fenStringParts[3]) : -1;

            // Set move counters
            halfMoves = Convert.ToInt32(fenStringParts[4]);
            fullMoves = Convert.ToInt32(fenStringParts[5]);
        }

        public void MakeMove(Move move)
        {
            _squares[move.endSquareIndex] = _squares[move.startSquareIndex];
            _squares[move.startSquareIndex] = Piece.NoneValue;
        }

        private int FileRankStringToIndex(string fileRankString)
        {
            int file = (int)fileRankString[0] - (int)'a';
            int rank = (int)char.GetNumericValue(fileRankString[1]) - 1;

            return IndexFromFileRank(file, rank);
        }

        private byte[] SquaresFromFEN(string fenSquares)
        {
            byte[] squares = new byte[64];

            int currentIndex = 0;

            foreach (string rank in fenSquares.Split('/').Reverse())
            {
                foreach (char rankElement in rank)
                {
                    if (char.IsDigit(rankElement))
                    {
                        currentIndex += (int)char.GetNumericValue(rankElement);
                    }
                    else
                    {
                        squares[currentIndex++] = Piece.SymbolToPiece(rankElement);
                    }
                }
            }

            return squares;
        }

        private byte CastlingRightsFromFEN(string castlingFEN)
        {
            byte castlingRights = 0b0000;

            castlingRights |= (byte)(castlingFEN.Contains("K") ? 0b1000 : 0b0000);
            castlingRights |= (byte)(castlingFEN.Contains("Q") ? 0b0100 : 0b0000);
            castlingRights |= (byte)(castlingFEN.Contains("k") ? 0b0010 : 0b0000);
            castlingRights |= (byte)(castlingFEN.Contains("q") ? 0b0001 : 0b0000);

            return castlingRights;
        }

        public static int IndexFromFileRank(int file, int rank)
        {
            return rank * 8 + file;
        }

        public override string ToString()
        {
            string boardString = string.Empty;

            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 0; file <= 7; file++)
                {
                    boardString += Piece.PieceToSymbol(_squares[IndexFromFileRank(file, rank)]);
                }
                boardString += '\n';
            }

            return boardString;
        }
    }

    public static class Piece
    {
        public const byte NoneValue   = 0b00000000;
        public const byte PawnValue   = 0b00000001;
        public const byte KnightValue = 0b00000010;
        public const byte BishopValue = 0b00000100;
        public const byte RookValue   = 0b00001000;
        public const byte QueenValue  = 0b00010000;
        public const byte KingValue   = 0b00100000;

        public const byte WhiteValue  = 0b01000000;
        public const byte BlackValue  = 0b10000000;

        public static bool IsWhite(byte piece)
        {
            return (piece & WhiteValue) == WhiteValue;
        }

        public static byte GetType(byte piece)
        {
            return (byte)(piece & 0b00111111);
        }

        public static char PieceToSymbol(byte piece)
        {
            char symbol;

            switch (Piece.GetType(piece))
            {
                case NoneValue:
                    return '.';
                case PawnValue:
                    symbol = 'P';
                    break;
                case KnightValue:
                    symbol = 'N';
                    break;
                case BishopValue:
                    symbol = 'B';
                    break;
                case RookValue:
                    symbol = 'R';
                    break;
                case QueenValue:
                    symbol = 'Q';
                    break;
                case KingValue:
                    symbol = 'K';
                    break;
                default:
                    throw new ArgumentException($"{nameof(piece)} was not a valid piece.");
            }

            return Piece.IsWhite(piece) ? char.ToUpper(symbol) : char.ToLower(symbol);
        }

        public static byte SymbolToPiece(char symbol)
        {
            byte piece = NoneValue;

            switch (char.ToUpper(symbol))
            {
                case 'P':
                    piece |= PawnValue;
                    break;
                case 'N':
                    piece |= KnightValue;
                    break;
                case 'B':
                    piece |= BishopValue;
                    break;
                case 'R':
                    piece |= RookValue;
                    break;
                case 'Q':
                    piece |= QueenValue;
                    break;
                case 'K':
                    piece |= KingValue;
                    break;
                default:
                    throw new ArgumentException($"{nameof(symbol)} was not a valid piece symbol.");
            }

            piece |= char.IsUpper(symbol) ? WhiteValue : BlackValue;

            return piece;
        }
    }
}
