using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Engine
{
    internal class Board
    {

    }

    internal class Piece
    {
        // Values for piece types. e.g. Pawn = 0b00000001
        private const int PawnValue   = (0b1 << 0);
        private const int KnightValue = (0b1 << 1);
        private const int BishopValue = (0b1 << 2);
        private const int RookValue   = (0b1 << 3);
        private const int QueenValue  = (0b1 << 4);
        private const int KingValue   = (0b1 << 5);

        // Values for piece colours. e.g. White = 0b01000000
        private const int WhiteValue  = (0b1 << 6);
        private const int BlackValue  = (0b1 << 7);

        public int Type { get; }
        public int Colour { get; }

        public bool IsWhite => (Colour & WhiteValue) == 0;

        public Piece(char pieceSymbol, bool isWhite)
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
