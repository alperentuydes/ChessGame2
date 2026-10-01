using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ChessGame2.Assets
{
    internal class PieceImages
    {

        public Uri RookBlackImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Blacks\Chess_rdt60.png");

        public Uri KingBlackImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Blacks\Chess_kdt60.png");

        public Uri QueenBlackImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Blacks\Chess_qdt60.png");

        public Uri BishopBlackImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Blacks\Chess_bdt60.png");

        public Uri KnightBlackImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Blacks\Chess_ndt60.png");

        public Uri PawnBlackImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Blacks\Chess_pdt60.png");


        // =========================
        // WHITE PIECES
        // =========================

        public Uri RookWhiteImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Whites\Chess_rlt60.png");

        public Uri KingWhiteImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Whites\Chess_klt60.png");

        public Uri QueenWhiteImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Whites\Chess_qlt60.png");

        public Uri BishopWhiteImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Whites\Chess_blt60.png");

        public Uri KnightWhiteImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Whites\Chess_nlt60.png");

        public Uri PawnWhiteImagePath { get; } =
            new Uri(@"C:\Users\Casper\source\repos\ChessGame\ChessGame\Assets\Whites\Chess_plt60.png");


        // =========================
        // BLACK BITMAPS
        // =========================

        public BitmapImage BitmapBlackRook { get; }
        public BitmapImage BitmapBlackKing { get; }
        public BitmapImage BitmapBlackQueen { get; }
        public BitmapImage BitmapBlackBishop { get; }
        public BitmapImage BitmapBlackKnight { get; }
        public BitmapImage BitmapBlackPawn { get; }


        // =========================
        // WHITE BITMAPS
        // =========================

        public BitmapImage BitmapWhiteRook { get; }
        public BitmapImage BitmapWhiteKing { get; }
        public BitmapImage BitmapWhiteQueen { get; }
        public BitmapImage BitmapWhiteBishop { get; }
        public BitmapImage BitmapWhiteKnight { get; }
        public BitmapImage BitmapWhitePawn { get; }


        public PieceImages()
        {
            // Black pieces
            BitmapBlackRook = new BitmapImage(RookBlackImagePath);
            BitmapBlackKing = new BitmapImage(KingBlackImagePath);
            BitmapBlackQueen = new BitmapImage(QueenBlackImagePath);
            BitmapBlackBishop = new BitmapImage(BishopBlackImagePath);
            BitmapBlackKnight = new BitmapImage(KnightBlackImagePath);
            BitmapBlackPawn = new BitmapImage(PawnBlackImagePath);

            // White pieces
            BitmapWhiteRook = new BitmapImage(RookWhiteImagePath);
            BitmapWhiteKing = new BitmapImage(KingWhiteImagePath);
            BitmapWhiteQueen = new BitmapImage(QueenWhiteImagePath);
            BitmapWhiteBishop = new BitmapImage(BishopWhiteImagePath);
            BitmapWhiteKnight = new BitmapImage(KnightWhiteImagePath);
            BitmapWhitePawn = new BitmapImage(PawnWhiteImagePath);
        }

    }
}
