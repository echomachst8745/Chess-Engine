using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chess_Engine
{
    internal class Board
    {
        public const string DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        Piece[] pieceArray = new Piece[64];

        public Board(string fenString = DefaultFEN)
        {
            pieceArray = BoardFromFENString(fenString);
        }

        private Piece[] BoardFromFENString(string fenString)
        {
            Piece[] board = Enumerable.Repeat(new Piece(), 64).ToArray();

            // Validate FEN string
            if (!Regex.IsMatch(fenString, @"^([pnbrqkPNBRQK1-8]+/){7}[pnbrqkPNBRQK1-8]+ [wb] K?Q?k?q? (([a-h][36])|-) \d \d$"))
            {
                throw new ArgumentException($"{nameof(fenString)} was not a valid FEN string.");
            }

            string[] fenStringParts = fenString.Split();

            // Place pieces
            int file = 0;
            int rank = 0;
            
            foreach (string fenRank in fenStringParts[0].Split('/'))
            {
                foreach (char rankElement in fenRank)
                {
                    bool elementIsNumber = char.IsDigit(rankElement);

                    if (char.IsDigit(rankElement))
                    {
                        file += (int)char.GetNumericValue(rankElement);
                    }
                    else
                    {
                        board[FileRankToIndex(file, rank)] = new Piece(rankElement);
                        file++;
                    }
                }

                file = 0;
                rank++;
            }

            // TODO: ADD Functionality for other FEN parts
            throw new NotImplementedException();

            return board;
        }

        private int FileRankToIndex(int file, int rank)
        {
            if (file < 0 || file > 7 || rank < 0 || rank > 7)
            {
                throw new ArgumentException($"{nameof(file)} or {nameof(rank)} was invalid.");
            }

            return rank * 8 + file;
        }
    }

    internal class Piece
    {
        private const int NullValue   = (0b0 << 0);

        // Values for piece types, e.g. Pawn = 0b00000001
        private const int PawnValue   = (0b1 << 0);
        private const int KnightValue = (0b1 << 1);
        private const int BishopValue = (0b1 << 2);
        private const int RookValue   = (0b1 << 3);
        private const int QueenValue  = (0b1 << 4);
        private const int KingValue   = (0b1 << 5);

        // Values for piece colours, e.g. White = 0b01000000
        private const int WhiteValue  = (0b1 << 6);
        private const int BlackValue  = (0b1 << 7);

        public int Type { get; } = NullValue;
        public int Colour { get; } = NullValue;

        public bool IsWhite => (Colour & WhiteValue) == WhiteValue;
        public bool IsNull => (Type & 0b11111111) == NullValue;

        public Piece()
        {

        }

        public Piece(char pieceSymbol)
        {
            switch (char.ToLower(pieceSymbol))
            {
                case 'p':
                    Type = PawnValue;
                    break;
                case 'n':
                    Type = KnightValue;
                    break;
                case 'b':
                    Type = BishopValue;
                    break;
                case 'r':
                    Type = RookValue;
                    break;
                case 'q':
                    Type = QueenValue;
                    break;
                case 'k':
                    Type = KingValue;
                    break;
                default:
                    throw new ArgumentException($"{nameof(pieceSymbol)} was not a valid piece symbol.");
            }

            Colour = char.IsLower(pieceSymbol) ? BlackValue : WhiteValue;
        }
    }
}
