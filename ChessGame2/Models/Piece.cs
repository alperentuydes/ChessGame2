using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame2.Models
{

    public enum PieceColor
    {
        White,
        Black
    }

    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    internal class Piece
    {
        public int ID { get; set; }

        public PieceColor Color { get; set; }

        public PieceType Type { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public bool IsEaten { get; set; }

        public bool DidFirstMove { get; set; }

        public string Position
        {
            get
            {
                char letter = (char)('A' + Column);

                int number = 8 - Row;

                return $"{letter}{number}";
            }
        }
    }
}
