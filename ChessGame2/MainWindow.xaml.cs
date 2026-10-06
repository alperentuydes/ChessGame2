using ChessGame2.Assets;
using ChessGame2.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ChessGame2
{
    public partial class MainWindow : Window
    {
        Piece[,] chessBoard = new Piece[8, 8];

        Border oldSquare;
        Piece selectedPiece;

        PieceColor currentTurn = PieceColor.White;
        PieceColor playerColor;

        PieceImages pieceImages = new PieceImages();

        List<(int Row, int Column)> LegalMoves = new List<(int Row, int Column)>();

        string playerName;
        string enemyName;

        TcpClient client;
        NetworkStream stream;

        public MainWindow(string playerName)
        {
            InitializeComponent();

            this.playerName = playerName;

            UserNameControl();
            CreateChessBoard();
            SetChessPiece();
        }

        public void UserNameControl()
        {
            MyName.Content = playerName;
        }

        private void CreateChessBoard()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Border square = new Border();

                    square.Tag = (row, column);
                    square.Cursor = Cursors.Hand;

                    if ((row + column) % 2 == 0)
                    {
                        square.Background = Brushes.White;
                    }
                    else
                    {
                        square.Background = Brushes.Gray;
                    }

                    square.MouseLeftButtonDown += square_Click;

                    ChessBoardUI.Children.Add(square);
                }
            }
        }

        private void SetChessPiece()
        {
            #region Beyaz Taşlar

            for (int column = 0; column < 8; column++)
            {
                Piece pawn = new Piece();
                pawn.Row = 6;
                pawn.Column = column;
                pawn.DidFirstMove = false;
                pawn.Type = PieceType.Pawn;
                pawn.Color = PieceColor.White;

                chessBoard[pawn.Row, pawn.Column] = pawn;

                int index = pawn.Row * 8 + pawn.Column;
                Border square = ChessBoardUI.Children[index] as Border;

                BitmapImage bitmap = new BitmapImage(pieceImages.PawnWhiteImagePath);

                Image image = new Image();
                image.Source = bitmap;
                image.Stretch = Stretch.Uniform;
                image.IsHitTestVisible = false;

                square.Child = image;
            }

            PieceType[] whiteBackRow =
            {
                    PieceType.Rook,
                    PieceType.Knight,
                    PieceType.Bishop,
                    PieceType.Queen,
                    PieceType.King,
                    PieceType.Bishop,
                    PieceType.Knight,
                    PieceType.Rook
            };

            for (int column = 0; column < 8; column++)
            {
                Piece piece = new Piece();
                piece.Row = 7;
                piece.Column = column;
                piece.DidFirstMove = false;
                piece.Type = whiteBackRow[column];
                piece.Color = PieceColor.White;

                chessBoard[piece.Row, piece.Column] = piece;

                int index = piece.Row * 8 + piece.Column;
                Border square = ChessBoardUI.Children[index] as Border;

                Uri imagePath = null;

                if (piece.Type == PieceType.Rook)
                    imagePath = pieceImages.RookWhiteImagePath;
                else if (piece.Type == PieceType.Knight)
                    imagePath = pieceImages.KnightWhiteImagePath;
                else if (piece.Type == PieceType.Bishop)
                    imagePath = pieceImages.BishopWhiteImagePath;
                else if (piece.Type == PieceType.Queen)
                    imagePath = pieceImages.QueenWhiteImagePath;
                else if (piece.Type == PieceType.King)
                    imagePath = pieceImages.KingWhiteImagePath;

                BitmapImage bitmap = new BitmapImage(imagePath);

                Image image = new Image();
                image.Source = bitmap;
                image.Stretch = Stretch.Uniform;
                image.IsHitTestVisible = false;

                square.Child = image;
            }

            #endregion

            #region Siyah Taşlar

            for (int column = 0; column < 8; column++)
            {
                Piece pawn = new Piece();
                pawn.Row = 1;
                pawn.Column = column;
                pawn.DidFirstMove = false;
                pawn.Type = PieceType.Pawn;
                pawn.Color = PieceColor.Black;

                chessBoard[pawn.Row, pawn.Column] = pawn;

                int index = pawn.Row * 8 + pawn.Column;
                Border square = ChessBoardUI.Children[index] as Border;

                BitmapImage bitmap = new BitmapImage(pieceImages.PawnBlackImagePath);

                Image image = new Image();
                image.Source = bitmap;
                image.Stretch = Stretch.Uniform;
                image.IsHitTestVisible = false;

                square.Child = image;
            }

            PieceType[] blackBackRow =
            {
                    PieceType.Rook,
                    PieceType.Knight,
                    PieceType.Bishop,
                    PieceType.Queen,
                    PieceType.King,
                    PieceType.Bishop,
                    PieceType.Knight,
                    PieceType.Rook
            };

            for (int column = 0; column < 8; column++)
            {
                Piece piece = new Piece();
                piece.Row = 0;
                piece.Column = column;
                piece.DidFirstMove = false;
                piece.Type = blackBackRow[column];
                piece.Color = PieceColor.Black;

                chessBoard[piece.Row, piece.Column] = piece;

                int index = piece.Row * 8 + piece.Column;
                Border square = ChessBoardUI.Children[index] as Border;

                Uri imagePath = null;

                if (piece.Type == PieceType.Rook)
                    imagePath = pieceImages.RookBlackImagePath;
                else if (piece.Type == PieceType.Knight)
                    imagePath = pieceImages.KnightBlackImagePath;
                else if (piece.Type == PieceType.Bishop)
                    imagePath = pieceImages.BishopBlackImagePath;
                else if (piece.Type == PieceType.Queen)
                    imagePath = pieceImages.QueenBlackImagePath;
                else if (piece.Type == PieceType.King)
                    imagePath = pieceImages.KingBlackImagePath;

                BitmapImage bitmap = new BitmapImage(imagePath);

                Image image = new Image();
                image.Source = bitmap;
                image.Stretch = Stretch.Uniform;
                image.IsHitTestVisible = false;

                square.Child = image;
            }

            #endregion
        }

        private void ResetAllColors()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    int index = row * 8 + column;

                    Border square = ChessBoardUI.Children[index] as Border;

                    if ((row + column) % 2 == 0)
                    {
                        square.Background = Brushes.White;
                    }
                    else
                    {
                        square.Background = Brushes.Gray;
                    }
                }
            }
        }

        private async void square_Click(object sender, MouseButtonEventArgs e)
        {
            Border clickedSquare = sender as Border;

            var (row, column) = ((int, int))clickedSquare.Tag;

            if (selectedPiece != null && LegalMoves.Contains((row, column)))
            {
                await MovePiece(sender, selectedPiece);

                selectedPiece = null;
                oldSquare = null;

                return;
            }

            ResetAllColors();

            SelectPiece(sender);
        }

        private void SelectPiece(object sender)
        {
            Border square = sender as Border;

            var (row, column) = ((int, int))square.Tag;

            Piece clickedPiece = chessBoard[row, column];

            if (clickedPiece == null)
            {
                oldSquare = null;
                selectedPiece = null;
                LegalMoves.Clear();

                return;
            }

            if (currentTurn != playerColor)
            {
                oldSquare = null;
                selectedPiece = null;
                LegalMoves.Clear();

                return;
            }

            if (clickedPiece.Color != playerColor)
            {
                oldSquare = null;
                selectedPiece = null;
                LegalMoves.Clear();

                return;
            }

            if (oldSquare == square)
            {
                oldSquare = null;
                selectedPiece = null;
                LegalMoves.Clear();

                return;
            }

            square.Background = Brushes.Orange;

            oldSquare = square;
            selectedPiece = clickedPiece;

            HighlightLegalMoves(clickedPiece);
        }

        private void HighlightLegalMoves(Piece piece)
        {
            LegalMoves.Clear();

            if (piece.Type == PieceType.Pawn)
            {
                if (piece.Color == PieceColor.White)
                {
                    int upperRow = piece.Row - 1;

                    if (upperRow >= 0)
                    {
                        if (chessBoard[upperRow, piece.Column] == null)
                        {
                            int upperIndex = upperRow * 8 + piece.Column;
                            Border upperSquare = ChessBoardUI.Children[upperIndex] as Border;

                            upperSquare.Background = Brushes.Red;

                            LegalMoves.Add((upperRow, piece.Column));

                            if (piece.DidFirstMove == false)
                            {
                                int upperUpperRow = piece.Row - 2;

                                if (upperUpperRow >= 0)
                                {
                                    if (chessBoard[upperUpperRow, piece.Column] == null)
                                    {
                                        int upperUpperIndex = upperUpperRow * 8 + piece.Column;
                                        Border upperUpperSquare = ChessBoardUI.Children[upperUpperIndex] as Border;

                                        upperUpperSquare.Background = Brushes.Red;

                                        LegalMoves.Add((upperUpperRow, piece.Column));
                                    }
                                }
                            }
                        }
                    }

                    int upperLeftColumn = piece.Column - 1;
                    int upperRightColumn = piece.Column + 1;

                    if (upperRow >= 0 && upperLeftColumn >= 0)
                    {
                        Piece enemyPiece = chessBoard[upperRow, upperLeftColumn];

                        if (enemyPiece != null && enemyPiece.Color != piece.Color)
                        {
                            int index = upperRow * 8 + upperLeftColumn;
                            Border square = ChessBoardUI.Children[index] as Border;

                            square.Background = Brushes.DarkRed;

                            LegalMoves.Add((upperRow, upperLeftColumn));
                        }
                    }

                    if (upperRow >= 0 && upperRightColumn <= 7)
                    {
                        Piece enemyPiece = chessBoard[upperRow, upperRightColumn];

                        if (enemyPiece != null && enemyPiece.Color != piece.Color)
                        {
                            int index = upperRow * 8 + upperRightColumn;
                            Border square = ChessBoardUI.Children[index] as Border;

                            square.Background = Brushes.DarkRed;

                            LegalMoves.Add((upperRow, upperRightColumn));
                        }
                    }
                }
                else
                {
                    int lowerRow = piece.Row + 1;

                    if (lowerRow <= 7)
                    {
                        if (chessBoard[lowerRow, piece.Column] == null)
                        {
                            int lowerIndex = lowerRow * 8 + piece.Column;
                            Border lowerSquare = ChessBoardUI.Children[lowerIndex] as Border;

                            lowerSquare.Background = Brushes.Red;

                            LegalMoves.Add((lowerRow, piece.Column));

                            if (piece.DidFirstMove == false)
                            {
                                int lowerLowerRow = piece.Row + 2;

                                if (lowerLowerRow <= 7)
                                {
                                    if (chessBoard[lowerLowerRow, piece.Column] == null)
                                    {
                                        int lowerLowerIndex = lowerLowerRow * 8 + piece.Column;
                                        Border lowerLowerSquare = ChessBoardUI.Children[lowerLowerIndex] as Border;

                                        lowerLowerSquare.Background = Brushes.Red;

                                        LegalMoves.Add((lowerLowerRow, piece.Column));
                                    }
                                }
                            }
                        }
                    }

                    int lowerLeftColumn = piece.Column - 1;
                    int lowerRightColumn = piece.Column + 1;

                    if (lowerRow <= 7 && lowerLeftColumn >= 0)
                    {
                        Piece enemyPiece = chessBoard[lowerRow, lowerLeftColumn];

                        if (enemyPiece != null && enemyPiece.Color != piece.Color)
                        {
                            int index = lowerRow * 8 + lowerLeftColumn;
                            Border square = ChessBoardUI.Children[index] as Border;

                            square.Background = Brushes.DarkRed;

                            LegalMoves.Add((lowerRow, lowerLeftColumn));
                        }
                    }

                    if (lowerRow <= 7 && lowerRightColumn <= 7)
                    {
                        Piece enemyPiece = chessBoard[lowerRow, lowerRightColumn];

                        if (enemyPiece != null && enemyPiece.Color != piece.Color)
                        {
                            int index = lowerRow * 8 + lowerRightColumn;
                            Border square = ChessBoardUI.Children[index] as Border;

                            square.Background = Brushes.DarkRed;

                            LegalMoves.Add((lowerRow, lowerRightColumn));
                        }
                    }
                }
            }
            if (piece.Type == PieceType.Rook)
            {
                if (piece.Color == PieceColor.White)
                {
                    for (int row = piece.Row - 1; row >= 0; row--) // yukarı hareket için
                    {
                        int column = piece.Column;
                        int index = row * 8 + column;
                        Border square = ChessBoardUI.Children[index] as Border;
                        Piece targetPiece = chessBoard[row, column];

                        if (targetPiece == null)
                        {
                            square.Background = Brushes.Red;
                            LegalMoves.Add((row, column));
                        }
                        else
                        {
                            if (targetPiece.Color != piece.Color)
                            {
                                square.Background = Brushes.DarkRed;
                                LegalMoves.Add((row, column));
                            }

                            break;
                        }
                    }
                    for (int row = piece.Row + 1; row <= 7; row++) // aşağı hareket için
                    {
                        int column = piece.Column;
                        int index = row * 8 + column;
                        Border square = ChessBoardUI.Children[index] as Border;
                        Piece targetPiece = chessBoard[row, column];

                        if (targetPiece == null)
                        {
                            square.Background = Brushes.Red;
                            LegalMoves.Add((row, column));
                        }
                        else
                        {
                            if (targetPiece.Color != piece.Color)
                            {
                                square.Background = Brushes.DarkRed;
                                LegalMoves.Add((row, column));
                            }

                            break;
                        }
                    }
                    for (int column = piece.Column - 1; column >= 0; column--) // sola hareket için
                    {
                        int index = piece.Row * 8 + column;
                        Border square = ChessBoardUI.Children[index] as Border;
                        Piece targetPiece = chessBoard[piece.Row, column];

                        if (targetPiece == null)
                        {
                            square.Background = Brushes.Red;
                            LegalMoves.Add((piece.Row, column));
                        }
                        else
                        {
                            if (targetPiece.Color != piece.Color)
                            {
                                square.Background = Brushes.DarkRed;
                                LegalMoves.Add((piece.Row, column));
                            }

                            break;
                        }
                    }
                    for (int column = piece.Column + 1; column <= 7; column++) // sola hareket için
                    {
                        int index = piece.Row * 8 + column;
                        Border square = ChessBoardUI.Children[index] as Border;
                        Piece targetPiece = chessBoard[piece.Row, column];

                        if (targetPiece == null)
                        {
                            square.Background = Brushes.Red;
                            LegalMoves.Add((piece.Row, column));
                        }
                        else
                        {
                            if (targetPiece.Color != piece.Color)
                            {
                                square.Background = Brushes.DarkRed;
                                LegalMoves.Add((piece.Row, column));
                            }

                            break;
                        }
                    }
                }
                if (piece.Color == PieceColor.Black)
                {
                    for (int row = piece.Row - 1; row >= 0; row--) // yukarı
                    {
                        int column = piece.Column;
                        int index = row * 8 + column;
                        Border square = ChessBoardUI.Children[index] as Border;
                        Piece targetPiece = chessBoard[row, column];

                        if (targetPiece == null)
                        {
                            square.Background = Brushes.Red;
                            LegalMoves.Add((row, column));
                        }
                        else
                        {
                            if (targetPiece.Color != piece.Color)
                            {
                                square.Background = Brushes.DarkRed;
                                LegalMoves.Add((row, column));
                            }

                            break;
                        }
                    }

                    for (int row = piece.Row + 1; row <= 7; row++) // aşağı
                    {
                        int column = piece.Column;
                        int index = row * 8 + column;
                        Border square = ChessBoardUI.Children[index] as Border;
                        Piece targetPiece = chessBoard[row, column];

                        if (targetPiece == null)
                        {
                            square.Background = Brushes.Red;
                            LegalMoves.Add((row, column));
                        }
                        else
                        {
                            if (targetPiece.Color != piece.Color)
                            {
                                square.Background = Brushes.DarkRed;
                                LegalMoves.Add((row, column));
                            }

                            break;
                        }
                    }

                    for (int column = piece.Column - 1; column >= 0; column--) // sola
                    {
                        int index = piece.Row * 8 + column;
                        Border square = ChessBoardUI.Children[index] as Border;
                        Piece targetPiece = chessBoard[piece.Row, column];

                        if (targetPiece == null)
                        {
                            square.Background = Brushes.Red;
                            LegalMoves.Add((piece.Row, column));
                        }
                        else
                        {
                            if (targetPiece.Color != piece.Color)
                            {
                                square.Background = Brushes.DarkRed;
                                LegalMoves.Add((piece.Row, column));
                            }

                            break;
                        }
                    }

                    for (int column = piece.Column + 1; column <= 7; column++) // sağa
                    {
                        int index = piece.Row * 8 + column;
                        Border square = ChessBoardUI.Children[index] as Border;
                        Piece targetPiece = chessBoard[piece.Row, column];

                        if (targetPiece == null)
                        {
                            square.Background = Brushes.Red;
                            LegalMoves.Add((piece.Row, column));
                        }
                        else
                        {
                            if (targetPiece.Color != piece.Color)
                            {
                                square.Background = Brushes.DarkRed;
                                LegalMoves.Add((piece.Row, column));
                            }

                            break;
                        }
                    }
                }
            }
            if (piece.Type == PieceType.Bishop)
            {
                int column = piece.Column - 1;

                for (int row = piece.Row - 1; row >= 0 && column >= 0; row--) // Sol üst
                {
                    int index = row * 8 + column;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((row, column));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((row, column));
                        }

                        break;
                    }

                    column--;
                }

                column = piece.Column + 1;


                for (int row = piece.Row - 1; row >= 0 && column <= 7; row--) // Sağ üst
                {
                    int index = row * 8 + column;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((row, column));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((row, column));
                        }

                        break;
                    }

                    column++;
                }

                column = piece.Column - 1;


                for (int row = piece.Row + 1; row <= 7 && column >= 0; row++) // Sol alt
                {
                    int index = row * 8 + column;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((row, column));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((row, column));
                        }

                        break;
                    }

                    column--;
                }

                column = piece.Column + 1;


                for (int row = piece.Row + 1; row <= 7 && column <= 7; row++) // Sağ alt
                {
                    int index = row * 8 + column;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((row, column));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((row, column));
                        }

                        break;
                    }

                    column++;
                }
            }
            if (piece.Type == PieceType.Queen)
            {
                int column = piece.Column - 1;

                for (int row = piece.Row - 1; row >= 0; row--) // yukarı
                {
                    int queenColumn = piece.Column;
                    int index = row * 8 + queenColumn;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[row, queenColumn];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((row, queenColumn));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((row, queenColumn));
                        }

                        break;
                    }
                }

                for (int row = piece.Row + 1; row <= 7; row++) // aşağı
                {
                    int queenColumn = piece.Column;
                    int index = row * 8 + queenColumn;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[row, queenColumn];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((row, queenColumn));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((row, queenColumn));
                        }

                        break;
                    }
                }

                for (int queenColumn = piece.Column - 1; queenColumn >= 0; queenColumn--) // sola
                {
                    int index = piece.Row * 8 + queenColumn;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[piece.Row, queenColumn];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((piece.Row, queenColumn));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((piece.Row, queenColumn));
                        }

                        break;
                    }
                }

                for (int queenColumn = piece.Column + 1; queenColumn <= 7; queenColumn++) // sağa
                {
                    int index = piece.Row * 8 + queenColumn;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[piece.Row, queenColumn];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((piece.Row, queenColumn));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((piece.Row, queenColumn));
                        }

                        break;
                    }
                }

                for (int row = piece.Row - 1; row >= 0 && column >= 0; row--) // Sol üst
                {
                    int index = row * 8 + column;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((row, column));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((row, column));
                        }

                        break;
                    }

                    column--;
                }

                column = piece.Column + 1;


                for (int row = piece.Row - 1; row >= 0 && column <= 7; row--) // Sağ üst
                {
                    int index = row * 8 + column;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((row, column));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((row, column));
                        }

                        break;
                    }

                    column++;
                }

                column = piece.Column - 1;


                for (int row = piece.Row + 1; row <= 7 && column >= 0; row++) // Sol alt
                {
                    int index = row * 8 + column;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((row, column));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((row, column));
                        }

                        break;
                    }

                    column--;
                }

                column = piece.Column + 1;


                for (int row = piece.Row + 1; row <= 7 && column <= 7; row++) // Sağ alt
                {
                    int index = row * 8 + column;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((row, column));
                    }
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((row, column));
                        }

                        break;
                    }

                    column++;
                }
            }
            if (piece.Type == PieceType.King)
            {
                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                //  ÜST

                if (piece.Row - 1 >= 0)
                {
                    int targetRow = piece.Row - 1;
                    int targetColumn = piece.Column;

                    int index = targetRow * 8 + targetColumn;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[targetRow, targetColumn];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                    else if (targetPiece.Color != piece.Color)
                    {
                        square.Background = Brushes.DarkRed;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                }

                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // ALT

                if (piece.Row + 1 <= 7)
                {
                    int targetRow = piece.Row + 1;
                    int targetColumn = piece.Column;

                    int index = targetRow * 8 + targetColumn;
                    Border square = ChessBoardUI.Children[index] as Border;

                    Piece targetPiece = chessBoard[targetRow, targetColumn];
                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                    else if (targetPiece.Color != piece.Color)
                    {
                        square.Background = Brushes.DarkRed;
                        LegalMoves.Add((targetRow, targetColumn));
                    }


                }

                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // SAĞ

                if (piece.Column + 1 <= 7)
                {
                    int targetRow = piece.Row;
                    int targetColumn1 = piece.Column + 1;

                    Piece targetPiece1 = chessBoard[targetRow, targetColumn1];

                    int index1 = targetRow * 8 + targetColumn1;
                    Border targetSquare1 = ChessBoardUI.Children[index1] as Border;

                    if (targetPiece1 == null)
                    {
                        LegalMoves.Add((targetRow, targetColumn1));
                        targetSquare1.Background = Brushes.Red;
                    }
                    else if (targetPiece1.Color != piece.Color)
                    {
                        LegalMoves.Add((targetRow, targetColumn1));
                        targetSquare1.Background = Brushes.DarkRed;
                    }

                    if (piece.DidFirstMove == false && piece.Column == 4)
                    {
                        Piece rightRook = chessBoard[piece.Row, 7];

                        if (rightRook != null && rightRook.Type == PieceType.Rook && rightRook.Color == piece.Color && rightRook.DidFirstMove == false)
                        {
                            int targetColumn2 = piece.Column + 2;

                            if (chessBoard[targetRow, piece.Column + 1] == null && chessBoard[targetRow, targetColumn2] == null)
                            {
                                if (IsKingInCheck(piece.Color) == false && CanCastling(piece, piece.Row, piece.Column + 1) && CanCastling(piece, piece.Row, piece.Column + 2))
                                {
                                    int index2 = targetRow * 8 + targetColumn2;
                                    Border targetSquare2 = ChessBoardUI.Children[index2] as Border;

                                    LegalMoves.Add((targetRow, targetColumn2));
                                    targetSquare2.Background = Brushes.Red;
                                }
                            }
                        }
                    }
                }

                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // SOL

                if (piece.Column - 1 >= 0)
                {
                    int targetRow = piece.Row;
                    int targetColumn1 = piece.Column - 1;

                    Piece targetPiece1 = chessBoard[targetRow, targetColumn1];

                    int index1 = targetRow * 8 + targetColumn1;
                    Border targetSquare1 = ChessBoardUI.Children[index1] as Border;

                    if (targetPiece1 == null)
                    {
                        LegalMoves.Add((targetRow, targetColumn1));
                        targetSquare1.Background = Brushes.Red;
                    }
                    else if (targetPiece1.Color != piece.Color)
                    {
                        LegalMoves.Add((targetRow, targetColumn1));
                        targetSquare1.Background = Brushes.DarkRed;
                    }

                    if (piece.DidFirstMove == false && piece.Column == 4)
                    {
                        Piece leftRook = chessBoard[piece.Row, 0];

                        if (leftRook != null && leftRook.Type == PieceType.Rook && leftRook.Color == piece.Color && leftRook.DidFirstMove == false)
                        {
                            if (chessBoard[targetRow, 1] == null && chessBoard[targetRow, 2] == null && chessBoard[targetRow, 3] == null)
                            {
                                if (IsKingInCheck(piece.Color) == false && CanCastling(piece, piece.Row, piece.Column - 1) && CanCastling(piece, piece.Row, piece.Column - 2))
                                {
                                    int targetColumn2 = piece.Column - 2;
                                    int index2 = targetRow * 8 + targetColumn2;

                                    Border targetSquare2 = ChessBoardUI.Children[index2] as Border;

                                    LegalMoves.Add((targetRow, targetColumn2));
                                    targetSquare2.Background = Brushes.Red;
                                }
                            }
                        }
                    }
                }

                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // SOL ÜST

                if (piece.Row - 1 >= 0 && piece.Column - 1 >= 0)
                {
                    int targetRow = piece.Row - 1;
                    int targetColumn = piece.Column - 1;

                    int index = targetRow * 8 + targetColumn;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[targetRow, targetColumn];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                    else if (targetPiece.Color != piece.Color)
                    {
                        square.Background = Brushes.DarkRed;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                }

                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // SAĞ ÜST

                if (piece.Row - 1 >= 0 && piece.Column + 1 <= 7)
                {
                    int targetRow = piece.Row - 1;
                    int targetColumn = piece.Column + 1;

                    int index = targetRow * 8 + targetColumn;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[targetRow, targetColumn];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                    else if (targetPiece.Color != piece.Color)
                    {
                        square.Background = Brushes.DarkRed;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                }

                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // SOL ALT
                if (piece.Row + 1 <= 7 && piece.Column - 1 >= 0)
                {
                    int targetRow = piece.Row + 1;
                    int targetColumn = piece.Column - 1;

                    int index = targetRow * 8 + targetColumn;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[targetRow, targetColumn];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                    else if (targetPiece.Color != piece.Color)
                    {
                        square.Background = Brushes.DarkRed;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                }
                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

                // SAĞ ALT
                if (piece.Row + 1 <= 7 && piece.Column + 1 <= 7)
                {
                    int targetRow = piece.Row + 1;
                    int targetColumn = piece.Column + 1;

                    int index = targetRow * 8 + targetColumn;
                    Border square = ChessBoardUI.Children[index] as Border;
                    Piece targetPiece = chessBoard[targetRow, targetColumn];

                    if (targetPiece == null)
                    {
                        square.Background = Brushes.Red;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                    else if (targetPiece.Color != piece.Color)
                    {
                        square.Background = Brushes.DarkRed;
                        LegalMoves.Add((targetRow, targetColumn));
                    }
                }
                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
            }
            if (piece.Type == PieceType.Knight)
            {
                int[,] knightMoves =
                {
                        { -2, -1 },
                        { -2,  1 },
                        { -1, -2 },
                        { -1,  2 },
                        {  1, -2 },
                        {  1,  2 },
                        {  2, -1 },
                        {  2,  1 }
                }; // { row, column }

                for (int i = 0; i < knightMoves.GetLength(0); i++) // Burada GetLength dizinin 0. (sıfırıncı) boyutuna kadar dön anlamına geliyor
                {

                    int row = knightMoves[i, 0]; // 0, 0 => -2 değerini veriyor. Yani row = -2
                    int column = knightMoves[i, 1]; // 0, 1 => -1 değerini veriyor. Yani column = -1
                    int targetRow = piece.Row + row;
                    int targetColumn = piece.Column + column;
                    int targetIndex = targetRow * 8 + targetColumn;

                    if (targetRow <= 7 && targetColumn <= 7 && targetRow >= 0 && targetColumn >= 0)
                    {
                        Piece targetPiece = chessBoard[targetRow, targetColumn];
                        Border square = ChessBoardUI.Children[targetIndex] as Border;
                        //if (targetPiece != null)
                        //{
                        //    if (piece.Color != targetPiece.Color)
                        //    {
                        //        square.Background = Brushes.DarkRed;
                        //        LegalMoves.Add((targetRow, targetColumn));
                        //    }
                        //}
                        //else
                        //{
                        //    square.Background = Brushes.Red;
                        //    LegalMoves.Add((targetRow, targetColumn));
                        //}
                        if (targetPiece == null)
                        {
                            square.Background = Brushes.Red;
                            LegalMoves.Add((targetRow, targetColumn));
                        }
                        else if (targetPiece.Color != piece.Color)
                        {
                            square.Background = Brushes.DarkRed;
                            LegalMoves.Add((targetRow, targetColumn));
                        }
                    }
                }
            }
        }

        private async Task MovePiece(object sender, Piece piece)
        {
            Border clickedSquare = sender as Border;

            int oldRow = piece.Row;
            int oldColumn = piece.Column;

            var (row, column) = ((int, int))clickedSquare.Tag;

            int oldIndex = piece.Row * 8 + piece.Column;
            int newIndex = row * 8 + column;

            if (LegalMoves.Contains((row, column)))
            {
                if (MoveLeavesKingInCheck(piece, row, column))
                    return;

                bool isCastling = piece.Type == PieceType.King && Math.Abs(column - piece.Column) == 2;

                if (isCastling)
                {
                    if (IsKingInCheck(piece.Color) == false) // CASTLING İŞLEMİ İÇİN AYRI BİR THREAD TANIMLA
                    {
                        Castling(piece, row, column);

                        piece.DidFirstMove = true;

                        LegalMoves.Clear();
                        ResetAllColors();
                        ChangeTurn();
                    }

                    return;
                }

                Border oldPieceSquare = ChessBoardUI.Children[oldIndex] as Border;
                Border newPieceSquare = ChessBoardUI.Children[newIndex] as Border;

                Image pieceImage = oldPieceSquare.Child as Image;

                Piece targetPiece = chessBoard[row, column];

                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    chessBoard[row, column] = null;
                    clickedSquare.Child = null;
                }

                chessBoard[piece.Row, piece.Column] = null;
                oldPieceSquare.Child = null;

                piece.Row = row;
                piece.Column = column;

                chessBoard[row, column] = piece;
                newPieceSquare.Child = pieceImage;

                // = = = = = = =

                await SendMoveData(client, playerName, enemyName, piece.Type, oldRow, oldColumn, row, column);

                // = = = = = = =

                PawnUpgrade(piece, row, column);

                piece.DidFirstMove = true;

                LegalMoves.Clear();
                ResetAllColors();
                ChangeTurn();

                if (Checkmate(currentTurn))
                {
                    CheckMateForm checkMate = new CheckMateForm();
                    checkMate.ShowDialog();
                }
            }
        }

        private void ChangeTurn()
        {
            if (currentTurn == PieceColor.White)
            {
                currentTurn = PieceColor.Black;
            }
            else
            {
                currentTurn = PieceColor.White;
            }
        }

        private bool IsKingInCheck(PieceColor kingColor) // taşları döndürüp değişkene atayıp => gidebileceği yerlerde King var mı?
        {
            PieceColor enemyColor;

            if (kingColor == PieceColor.White)
            {
                enemyColor = PieceColor.Black;
            }
            else
            {
                enemyColor = PieceColor.White;
            }

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Piece piece = chessBoard[row, column]; // Bizim taş

                    if (piece == null)
                    {
                        continue;
                    }

                    if (piece.Color == enemyColor)
                    {
                        if (piece.Type == PieceType.Pawn)
                        {
                            int targetRow;

                            if (piece.Color == PieceColor.White)
                            {
                                targetRow = piece.Row - 1;
                            }
                            else
                            {
                                targetRow = piece.Row + 1;
                            }

                            int targetLeftColumn = piece.Column - 1;
                            int targetRightColumn = piece.Column + 1;

                            if (targetRow >= 0 && targetRow <= 7 && targetLeftColumn >= 0)
                            {
                                Piece targetPiece = chessBoard[targetRow, targetLeftColumn];

                                if (targetPiece != null && targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                {
                                    return true;
                                }
                            }

                            if (targetRow >= 0 && targetRow <= 7 && targetRightColumn <= 7)
                            {
                                Piece targetPiece = chessBoard[targetRow, targetRightColumn];

                                if (targetPiece != null && targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                {
                                    return true;
                                }
                            }
                        }
                        if (piece.Type == PieceType.Rook)
                        {
                            // kalenin siyah ya da beyaz olması farketmiyor.
                            // Yukarı yön için
                            for (int pieceRow = piece.Row - 1; pieceRow >= 0; pieceRow--) // Yukarı yönde hareket için
                            {
                                Piece targetPiece = chessBoard[pieceRow, piece.Column];

                                if (targetPiece == null)
                                    continue;

                                if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    return true;

                                break;
                            }

                            // Aşağı yön için
                            for (int pieceRow = piece.Row + 1; pieceRow <= 7; pieceRow++)
                            {
                                Piece targetPiece = chessBoard[pieceRow, piece.Column];

                                if (targetPiece == null)
                                    continue;

                                if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    return true;

                                break;
                            }

                            // Sol yönüne doğru
                            for (int piececolumn = piece.Column - 1; piececolumn >= 0; piececolumn--)
                            {
                                Piece targetPiece = chessBoard[piece.Row, piececolumn];

                                if (targetPiece == null)
                                    continue;

                                if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    return true;

                                break;
                            }

                            // Sağ yönüne doğru
                            for (int piececolumn = piece.Column + 1; piececolumn <= 7; piececolumn++)
                            {
                                Piece targetPiece = chessBoard[piece.Row, piececolumn];

                                if (targetPiece == null)
                                    continue;

                                if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    return true;

                                break;
                            }
                        }
                        if (piece.Type == PieceType.Bishop)
                        {
                            // ======================================================================================

                            int pieceUpperLeftColumn = piece.Column - 1;

                            for (int pieceRow = piece.Row - 1; pieceRow >= 0 && pieceUpperLeftColumn >= 0; pieceRow--)
                            {

                                Piece targetPiece = chessBoard[pieceRow, pieceUpperLeftColumn];

                                if (targetPiece != null)
                                {
                                    if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    {
                                        return true;
                                    }
                                    break;
                                }

                                pieceUpperLeftColumn--;
                            }

                            // ======================================================================================

                            int pieceLowerLeftColumn = piece.Column - 1;

                            for (int pieceRow = piece.Row + 1; pieceRow <= 7 && pieceLowerLeftColumn >= 0; pieceRow++)
                            {
                                Piece targetPiece = chessBoard[pieceRow, pieceLowerLeftColumn];

                                if (targetPiece != null)
                                {
                                    if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    {
                                        return true;
                                    }
                                    break;
                                }

                                pieceLowerLeftColumn--;
                            }

                            // ======================================================================================

                            int pieceUpperRightColumn = piece.Column + 1;

                            for (int pieceRow = piece.Row - 1; pieceRow >= 0 && pieceUpperRightColumn <= 7; pieceRow--)
                            {

                                Piece targetPiece = chessBoard[pieceRow, pieceUpperRightColumn];

                                if (targetPiece != null)
                                {
                                    if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    {
                                        return true;
                                    }
                                    break;
                                }

                                pieceUpperRightColumn++;
                            }

                            // ======================================================================================

                            int pieceLowerRightColumn = piece.Column + 1;

                            for (int pieceRow = piece.Row + 1; pieceRow <= 7 && pieceLowerRightColumn <= 7; pieceRow++)
                            {

                                Piece targetPiece = chessBoard[pieceRow, pieceLowerRightColumn];

                                if (targetPiece != null)
                                {
                                    if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    {
                                        return true;
                                    }
                                    break;
                                }

                                pieceLowerRightColumn++;
                            }

                        }
                        if (piece.Type == PieceType.Queen)
                        {
                            for (int pieceRow = piece.Row - 1; pieceRow >= 0; pieceRow--) // Yukarı yönde hareket için
                            {
                                Piece targetPiece = chessBoard[pieceRow, piece.Column];

                                if (targetPiece == null)
                                    continue;

                                if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    return true;

                                break;
                            }

                            // Aşağı yön için
                            for (int pieceRow = piece.Row + 1; pieceRow <= 7; pieceRow++)
                            {
                                Piece targetPiece = chessBoard[pieceRow, piece.Column];

                                if (targetPiece == null)
                                    continue;

                                if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    return true;

                                break;
                            }

                            // Sol yönüne doğru
                            for (int piececolumn = piece.Column - 1; piececolumn >= 0; piececolumn--)
                            {
                                Piece targetPiece = chessBoard[piece.Row, piececolumn];

                                if (targetPiece == null)
                                    continue;

                                if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    return true;

                                break;
                            }

                            // Sağ yönüne doğru
                            for (int piececolumn = piece.Column + 1; piececolumn <= 7; piececolumn++)
                            {
                                Piece targetPiece = chessBoard[piece.Row, piececolumn];

                                if (targetPiece == null)
                                    continue;

                                if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    return true;

                                break;
                            }

                            // ======================================================================================

                            int pieceUpperLeftColumn = piece.Column - 1;

                            for (int pieceRow = piece.Row - 1; pieceRow >= 0 && pieceUpperLeftColumn >= 0; pieceRow--)
                            {

                                Piece targetPiece = chessBoard[pieceRow, pieceUpperLeftColumn];

                                if (targetPiece != null)
                                {
                                    if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    {
                                        return true;
                                    }
                                    break;
                                }

                                pieceUpperLeftColumn--;
                            }

                            // ======================================================================================

                            int pieceLowerLeftColumn = piece.Column - 1;

                            for (int pieceRow = piece.Row + 1; pieceRow <= 7 && pieceLowerLeftColumn >= 0; pieceRow++)
                            {
                                Piece targetPiece = chessBoard[pieceRow, pieceLowerLeftColumn];

                                if (targetPiece != null)
                                {
                                    if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    {
                                        return true;
                                    }
                                    break;
                                }

                                pieceLowerLeftColumn--;
                            }

                            // ======================================================================================

                            int pieceUpperRightColumn = piece.Column + 1;

                            for (int pieceRow = piece.Row - 1; pieceRow >= 0 && pieceUpperRightColumn <= 7; pieceRow--)
                            {

                                Piece targetPiece = chessBoard[pieceRow, pieceUpperRightColumn];

                                if (targetPiece != null)
                                {
                                    if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    {
                                        return true;
                                    }
                                    break;
                                }

                                pieceUpperRightColumn++;
                            }

                            // ======================================================================================

                            int pieceLowerRightColumn = piece.Column + 1;

                            for (int pieceRow = piece.Row + 1; pieceRow <= 7 && pieceLowerRightColumn <= 7; pieceRow++)
                            {

                                Piece targetPiece = chessBoard[pieceRow, pieceLowerRightColumn];

                                if (targetPiece != null)
                                {
                                    if (targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    {
                                        return true;
                                    }
                                    break;
                                }

                                pieceLowerRightColumn++;
                            }
                        }
                        if (piece.Type == PieceType.Knight)
                        {
                            int[,] knightMoves =
                            {
                                { -2, -1 },
                                { -2,  1 },
                                { -1, -2 },
                                { -1,  2 },
                                {  1, -2 },
                                {  1,  2 },
                                {  2, -1 },
                                {  2,  1 }
                            };

                            for (int i = 0; i < knightMoves.GetLength(0); i++)
                            {
                                int targetRow = piece.Row + knightMoves[i, 0];
                                int targetColumn = piece.Column + knightMoves[i, 1];

                                if (targetRow >= 0 && targetRow <= 7 && targetColumn >= 0 && targetColumn <= 7)
                                {
                                    Piece targetPiece = chessBoard[targetRow, targetColumn];

                                    if (targetPiece != null && targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        if (piece.Type == PieceType.King)
                        {
                            int[,] kingMoves =
                                                {
                                                    { -1, -1 },
                                                    { -1,  0 },
                                                    { -1,  1 },
                                                    {  0, -1 },
                                                    {  0,  1 },
                                                    {  1, -1 },
                                                    {  1,  0 },
                                                    {  1,  1 }
                                                };

                            for (int i = 0; i < kingMoves.GetLength(0); i++)
                            {
                                int targetRow = piece.Row + kingMoves[i, 0];
                                int targetColumn = piece.Column + kingMoves[i, 1];

                                if (targetRow >= 0 && targetRow <= 7 && targetColumn >= 0 && targetColumn <= 7)
                                {

                                    Piece targetPiece = chessBoard[targetRow, targetColumn];

                                    if (targetPiece != null && targetPiece.Color == kingColor && targetPiece.Type == PieceType.King)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool MoveLeavesKingInCheck(Piece piece, int targetRow, int targetColumn) // taşı başka bir yere hareket etmemtirdiğimde king şah oluyor mu ?
        {
            int oldRow = piece.Row;
            int oldColumn = piece.Column;

            Piece targetPiece = chessBoard[targetRow, targetColumn];

            chessBoard[oldRow, oldColumn] = null;
            chessBoard[targetRow, targetColumn] = piece;

            piece.Row = targetRow;
            piece.Column = targetColumn;

            bool kingInCheck = IsKingInCheck(piece.Color);

            chessBoard[oldRow, oldColumn] = piece;
            chessBoard[targetRow, targetColumn] = targetPiece;

            piece.Row = oldRow;
            piece.Column = oldColumn;

            return kingInCheck;
        }

        private void PawnUpgrade(Piece piece, int targetRow, int targetColumn) // Yeni Ekledim
        {
            if (piece.Type == PieceType.Pawn)
            {
                if (piece.Color == PieceColor.White && targetRow == 0)
                {
                    PieceSelectionForm form = new PieceSelectionForm(piece, targetRow, targetColumn);

                    if (form.ShowDialog() == true)
                    {
                        piece = form.piece;
                    }

                    int index = piece.Row * 8 + piece.Column;
                    Border square = ChessBoardUI.Children[index] as Border;

                    BitmapImage bitmap = null;

                    switch (piece.Type)
                    {
                        case PieceType.Rook:
                            bitmap = new BitmapImage(pieceImages.RookWhiteImagePath);
                            break;

                        case PieceType.Knight:
                            bitmap = new BitmapImage(pieceImages.KnightWhiteImagePath);
                            break;

                        case PieceType.Bishop:
                            bitmap = new BitmapImage(pieceImages.BishopWhiteImagePath);
                            break;

                        case PieceType.Queen:
                            bitmap = new BitmapImage(pieceImages.QueenWhiteImagePath);
                            break;
                    }

                    if (bitmap != null)
                    {
                        Image image = new Image();

                        image.Source = bitmap;
                        image.Stretch = Stretch.Uniform;
                        image.IsHitTestVisible = false;

                        square.Child = image;
                    }
                }
                if (piece.Color == PieceColor.Black && targetRow == 7)
                {
                    PieceSelectionForm form = new PieceSelectionForm(piece, targetRow, targetColumn);

                    if (form.ShowDialog() == true)
                    {
                        piece = form.piece;
                    }

                    int index = piece.Row * 8 + piece.Column;
                    Border square = ChessBoardUI.Children[index] as Border;

                    BitmapImage bitmap = null;

                    switch (piece.Type)
                    {
                        case PieceType.Rook:
                            bitmap = new BitmapImage(pieceImages.RookBlackImagePath);
                            break;

                        case PieceType.Knight:
                            bitmap = new BitmapImage(pieceImages.KnightBlackImagePath);
                            break;

                        case PieceType.Bishop:
                            bitmap = new BitmapImage(pieceImages.BishopBlackImagePath);
                            break;

                        case PieceType.Queen:
                            bitmap = new BitmapImage(pieceImages.QueenBlackImagePath);
                            break;
                    }

                    if (bitmap != null)
                    {
                        Image image = new Image();
                        image.Source = bitmap;
                        image.Stretch = Stretch.Uniform;
                        image.IsHitTestVisible = false;

                        square.Child = image;
                    }
                }
            }
        }

        private void Castling(Piece piece, int targetRow, int targetColumn)
        {
            if (piece.Type == PieceType.King)
            {
                if (PieceColor.White == piece.Color)
                {
                    if (targetRow == 7 && targetColumn == 6)
                    {
                        int pieceIndex = piece.Row * 8 + piece.Column;

                        Piece RightRook = chessBoard[7, 7];
                        if (RightRook != null && RightRook.DidFirstMove == false)
                        {
                            int rookIndex = RightRook.Row * 8 + RightRook.Column;

                            Border rookSquare = ChessBoardUI.Children[rookIndex] as Border;
                            Border pieceSquare = ChessBoardUI.Children[pieceIndex] as Border;

                            Piece targetPiece1 = chessBoard[7, 5];
                            Piece targetPiece2 = chessBoard[7, 6];

                            if (targetPiece1 == null && targetPiece2 == null)
                            {
                                int target1Index = 7 * 8 + 5;
                                int target2Index = 7 * 8 + 6;

                                Border squareTarget1 = ChessBoardUI.Children[target1Index] as Border;
                                Border squareTarget2 = ChessBoardUI.Children[target2Index] as Border;

                                Image rookImage = rookSquare.Child as Image;
                                Image kingImage = pieceSquare.Child as Image;

                                chessBoard[7, 7] = null;
                                chessBoard[7, 4] = null;

                                RightRook.Row = 7;
                                RightRook.Column = 5;
                                RightRook.DidFirstMove = true;

                                piece.Row = 7;
                                piece.Column = 6;

                                chessBoard[7, 5] = RightRook;
                                chessBoard[7, 6] = piece;

                                //if (IsKingInCheck(piece.Color))
                                //    MessageBox.Show("Şah Oluyor");

                                rookSquare.Child = null;
                                pieceSquare.Child = null;

                                squareTarget1.Child = rookImage;
                                squareTarget2.Child = kingImage;
                            }
                        }
                        else
                            return;
                    }

                    if (targetRow == 7 && targetColumn == 2)
                    {
                        Piece LeftRook = chessBoard[7, 0];

                        if (LeftRook != null && LeftRook.DidFirstMove == false)
                        {
                            Piece target1 = chessBoard[7, 2];
                            Piece target2 = chessBoard[7, 3];

                            if (target1 == null && target2 == null)
                            {
                                chessBoard[7, 0] = null;
                                chessBoard[piece.Row, piece.Column] = null;

                                Border target1Square = ChessBoardUI.Children[7 * 8 + 2] as Border;
                                Border target2Square = ChessBoardUI.Children[7 * 8 + 3] as Border;

                                Border kingSquare = ChessBoardUI.Children[piece.Row * 8 + piece.Column] as Border;
                                Border leftRookSquare = ChessBoardUI.Children[LeftRook.Row * 8 + LeftRook.Column] as Border;

                                Image kingImage = kingSquare.Child as Image;
                                Image leftRookImage = leftRookSquare.Child as Image;

                                target1 = piece;
                                target2 = LeftRook;

                                LeftRook.DidFirstMove = true;
                                LeftRook.Row = 7;
                                LeftRook.Column = 3;

                                chessBoard[7, 3] = LeftRook;

                                piece.DidFirstMove = true;
                                piece.Row = 7;
                                piece.Column = 2;

                                chessBoard[7, 2] = piece;

                                kingSquare.Child = null;
                                leftRookSquare.Child = null;

                                target1Square.Child = kingImage;
                                target2Square.Child = leftRookImage;

                            }
                            else
                                return;
                            // matristeki orijinal yerlerini sil
                            // yeni yerleri o matrislere yerleştir

                            // UI'ları bul ve al.
                            // UI'ları sıfırla

                        }
                        else
                            return;

                    }
                }
                if (PieceColor.Black == piece.Color)
                {
                    if (targetRow == 0 && targetColumn == 6)
                    {
                        int pieceIndex = piece.Row * 8 + piece.Column;

                        Piece RightRook = chessBoard[0, 7];

                        if (RightRook != null && RightRook.DidFirstMove == false)
                        {
                            int rookIndex = RightRook.Row * 8 + RightRook.Column;

                            Border rookSquare = ChessBoardUI.Children[rookIndex] as Border;
                            Border pieceSquare = ChessBoardUI.Children[pieceIndex] as Border;

                            Piece targetPiece1 = chessBoard[0, 5];
                            Piece targetPiece2 = chessBoard[0, 6];

                            if (targetPiece1 == null && targetPiece2 == null)
                            {
                                int target1Index = 0 * 8 + 5;
                                int target2Index = 0 * 8 + 6;

                                Border squareTarget1 = ChessBoardUI.Children[target1Index] as Border;
                                Border squareTarget2 = ChessBoardUI.Children[target2Index] as Border;

                                Image rookImage = rookSquare.Child as Image;
                                Image kingImage = pieceSquare.Child as Image;

                                chessBoard[0, 7] = null;
                                chessBoard[0, 4] = null;

                                RightRook.Row = 0;
                                RightRook.Column = 5;
                                RightRook.DidFirstMove = true;

                                piece.Row = 0;
                                piece.Column = 6;

                                chessBoard[0, 5] = RightRook;
                                chessBoard[0, 6] = piece;

                                rookSquare.Child = null;
                                pieceSquare.Child = null;

                                squareTarget1.Child = rookImage;
                                squareTarget2.Child = kingImage;
                            }
                            else
                                return;
                        }
                    }
                    if (targetRow == 0 && targetColumn == 2)
                    {
                        Piece LeftRook = chessBoard[0, 0];

                        if (LeftRook != null && LeftRook.DidFirstMove == false)
                        {
                            Piece targetPiece1 = chessBoard[0, 1]; // B8
                            Piece targetPiece2 = chessBoard[0, 2]; // C8
                            Piece targetPiece3 = chessBoard[0, 3]; // D8

                            if (targetPiece1 == null && targetPiece2 == null && targetPiece3 == null)
                            {
                                int pieceIndex = piece.Row * 8 + piece.Column;
                                int rookIndex = LeftRook.Row * 8 + LeftRook.Column;

                                Border kingSquare = ChessBoardUI.Children[pieceIndex] as Border;
                                Border leftRookSquare = ChessBoardUI.Children[rookIndex] as Border;

                                Border kingTargetSquare = ChessBoardUI.Children[0 * 8 + 2] as Border;
                                Border rookTargetSquare = ChessBoardUI.Children[0 * 8 + 3] as Border;

                                Image kingImage = kingSquare.Child as Image;
                                Image leftRookImage = leftRookSquare.Child as Image;

                                chessBoard[0, 0] = null;
                                chessBoard[0, 4] = null;

                                LeftRook.Row = 0;
                                LeftRook.Column = 3;
                                LeftRook.DidFirstMove = true;

                                piece.Row = 0;
                                piece.Column = 2;
                                piece.DidFirstMove = true;

                                chessBoard[0, 2] = piece;
                                chessBoard[0, 3] = LeftRook;

                                kingSquare.Child = null;
                                leftRookSquare.Child = null;

                                kingTargetSquare.Child = kingImage;
                                rookTargetSquare.Child = leftRookImage;
                            }
                        }
                        else
                            return;
                    }
                }
            }
        }

        private bool CanCastling(Piece kingPiece, int row, int column)
        {
            PieceColor enemyColor;

            if (kingPiece.Color == PieceColor.White)
                enemyColor = PieceColor.Black;
            else
                enemyColor = PieceColor.White;

            // --------------------------------------------------
            // ROOK / QUEEN - YUKARI

            for (int targetRow = row - 1; targetRow >= 0; targetRow--)
            {
                Piece targetPiece = chessBoard[targetRow, column];

                if (targetPiece == null)
                    continue;

                if (targetPiece.Color == enemyColor && (targetPiece.Type == PieceType.Rook || targetPiece.Type == PieceType.Queen))
                    return false;

                break;
            }

            // --------------------------------------------------
            // ROOK / QUEEN - AŞAĞI

            for (int targetRow = row + 1; targetRow <= 7; targetRow++)
            {
                Piece targetPiece = chessBoard[targetRow, column];

                if (targetPiece == null)
                    continue;

                if (targetPiece.Color == enemyColor && (targetPiece.Type == PieceType.Rook || targetPiece.Type == PieceType.Queen))
                    return false;

                break;
            }

            // --------------------------------------------------
            // ROOK / QUEEN - SOL

            for (int targetColumn = column - 1; targetColumn >= 0; targetColumn--)
            {
                Piece targetPiece = chessBoard[row, targetColumn];

                if (targetPiece == null)
                    continue;

                if (targetPiece.Color == enemyColor && (targetPiece.Type == PieceType.Rook || targetPiece.Type == PieceType.Queen))
                    return false;

                break;
            }

            // --------------------------------------------------
            // ROOK / QUEEN - SAĞ

            for (int targetColumn = column + 1; targetColumn <= 7; targetColumn++)
            {
                Piece targetPiece = chessBoard[row, targetColumn];

                if (targetPiece == null)
                    continue;

                if (targetPiece.Color == enemyColor && (targetPiece.Type == PieceType.Rook || targetPiece.Type == PieceType.Queen))
                    return false;

                break;
            }

            // --------------------------------------------------
            // BISHOP / QUEEN - SOL ÜST

            int targetLeftColumnControl = column - 1;
            for (int targetRow = row - 1; targetRow >= 0 && targetLeftColumnControl >= 0; targetRow--)
            {
                Piece targetPiece = chessBoard[targetRow, targetLeftColumnControl];

                if (targetPiece == null)
                {
                    targetLeftColumnControl--;
                    continue;
                }

                if (targetPiece.Color == enemyColor && (targetPiece.Type == PieceType.Bishop || targetPiece.Type == PieceType.Queen))
                    return false;

                break;
            }

            // --------------------------------------------------
            // BISHOP / QUEEN - SAĞ ÜST

            int targetRightColumnControl = column + 1;
            for (int targetRow = row - 1; targetRow >= 0 && targetRightColumnControl <= 7; targetRow--)
            {
                Piece targetPiece = chessBoard[targetRow, targetRightColumnControl];

                if (targetPiece == null)
                {
                    targetRightColumnControl++;
                    continue;
                }

                if (targetPiece.Color == enemyColor && (targetPiece.Type == PieceType.Bishop || targetPiece.Type == PieceType.Queen))
                    return false;

                break;
            }

            // --------------------------------------------------
            // BISHOP / QUEEN - SOL ALT

            targetLeftColumnControl = column - 1;
            for (int targetRow = row + 1; targetRow <= 7 && targetLeftColumnControl >= 0; targetRow++)
            {
                Piece targetPiece = chessBoard[targetRow, targetLeftColumnControl];

                if (targetPiece == null)
                {
                    targetLeftColumnControl--;
                    continue;
                }

                if (targetPiece.Color == enemyColor && (targetPiece.Type == PieceType.Bishop || targetPiece.Type == PieceType.Queen))
                    return false;

                break;
            }

            // --------------------------------------------------
            // BISHOP / QUEEN - SAĞ ALT

            targetRightColumnControl = column + 1;
            for (int targetRow = row + 1; targetRow <= 7 && targetRightColumnControl <= 7; targetRow++)
            {
                Piece targetPiece = chessBoard[targetRow, targetRightColumnControl];

                if (targetPiece == null)
                {
                    targetRightColumnControl++;
                    continue;
                }

                if (targetPiece.Color == enemyColor && (targetPiece.Type == PieceType.Bishop || targetPiece.Type == PieceType.Queen))
                    return false;

                break;
            }

            // --------------------------------------------------
            // PAWN

            if (enemyColor == PieceColor.Black)
            {
                int pawnRow = row - 1;

                if (pawnRow >= 0)
                {
                    int leftColumn = column - 1;
                    int rightColumn = column + 1;

                    if (leftColumn >= 0)
                    {
                        Piece targetPiece = chessBoard[pawnRow, leftColumn];

                        if (targetPiece != null && targetPiece.Color == enemyColor && targetPiece.Type == PieceType.Pawn)
                            return false;
                    }

                    if (rightColumn <= 7)
                    {
                        Piece targetPiece = chessBoard[pawnRow, rightColumn];

                        if (targetPiece != null && targetPiece.Color == enemyColor && targetPiece.Type == PieceType.Pawn)
                            return false;
                    }
                }
            }
            else
            {
                int pawnRow = row + 1;

                if (pawnRow <= 7)
                {
                    int leftColumn = column - 1;
                    int rightColumn = column + 1;

                    if (leftColumn >= 0)
                    {
                        Piece targetPiece = chessBoard[pawnRow, leftColumn];

                        if (targetPiece != null && targetPiece.Color == enemyColor && targetPiece.Type == PieceType.Pawn)
                            return false;
                    }

                    if (rightColumn <= 7)
                    {
                        Piece targetPiece = chessBoard[pawnRow, rightColumn];

                        if (targetPiece != null && targetPiece.Color == enemyColor && targetPiece.Type == PieceType.Pawn)
                            return false;
                    }
                }
            }

            // --------------------------------------------------
            // KNIGHT için

            int[,] knightMoves =
            {
                    { -2, -1 },
                    { -2,  1 },
                    { -1, -2 },
                    { -1,  2 },
                    {  1, -2 },
                    {  1,  2 },
                    {  2, -1 },
                    {  2,  1 }
            };

            for (int i = 0; i < knightMoves.GetLength(0); i++)
            {
                int targetRow = row + knightMoves[i, 0];
                int targetColumn = column + knightMoves[i, 1];

                if (targetRow >= 0 && targetRow <= 7 && targetColumn >= 0 && targetColumn <= 7)
                {
                    Piece targetPiece = chessBoard[targetRow, targetColumn];

                    if (targetPiece != null && targetPiece.Color == enemyColor && targetPiece.Type == PieceType.Knight)
                        return false;
                }
            }

            // --------------------------------------------------
            // --------------------------------------------------
            // KING için

            int[,] kingMoves =
            {
                    { -1, -1 },
                    { -1,  0 },
                    { -1,  1 },
                    {  0, -1 },
                    {  0,  1 },
                    {  1, -1 },
                    {  1,  0 },
                    {  1,  1 }
            };
            for (int i = 0; i < kingMoves.GetLength(0); i++)
            {
                int targetRow = row + kingMoves[i, 0];
                int targetColumn = column + kingMoves[i, 1];

                if (targetRow >= 0 && targetRow <= 7 && targetColumn >= 0 && targetColumn <= 7)
                {
                    Piece targetPiece = chessBoard[targetRow, targetColumn];

                    if (targetPiece != null && targetPiece.Color == enemyColor && targetPiece.Type == PieceType.King)
                        return false;
                }
            }
            return true;

            // --------------------------------------------------
        }

        private List<(int Row, int Column)> GetLegalMoves(Piece piece)
        {
            List<(int Row, int Column)> moves = new List<(int Row, int Column)>();

            if (piece.Type == PieceType.Pawn)
            {
                if (piece.Color == PieceColor.White)
                {
                    int upperRow = piece.Row - 1;

                    if (upperRow >= 0 && chessBoard[upperRow, piece.Column] == null)
                    {
                        moves.Add((upperRow, piece.Column));

                        int upperUpperRow = piece.Row - 2;

                        if (piece.DidFirstMove == false && upperUpperRow >= 0 && chessBoard[upperUpperRow, piece.Column] == null)
                            moves.Add((upperUpperRow, piece.Column));
                    }

                    int upperLeftColumn = piece.Column - 1;
                    int upperRightColumn = piece.Column + 1;

                    if (upperRow >= 0 && upperLeftColumn >= 0)
                    {
                        Piece targetPiece = chessBoard[upperRow, upperLeftColumn];

                        if (targetPiece != null && targetPiece.Color != piece.Color)
                            moves.Add((upperRow, upperLeftColumn));
                    }

                    if (upperRow >= 0 && upperRightColumn <= 7)
                    {
                        Piece targetPiece = chessBoard[upperRow, upperRightColumn];

                        if (targetPiece != null && targetPiece.Color != piece.Color)
                            moves.Add((upperRow, upperRightColumn));
                    }
                }
                else
                {
                    int lowerRow = piece.Row + 1;

                    if (lowerRow <= 7 && chessBoard[lowerRow, piece.Column] == null)
                    {
                        moves.Add((lowerRow, piece.Column));

                        int lowerLowerRow = piece.Row + 2;

                        if (piece.DidFirstMove == false && lowerLowerRow <= 7 && chessBoard[lowerLowerRow, piece.Column] == null)
                            moves.Add((lowerLowerRow, piece.Column));
                    }

                    int lowerLeftColumn = piece.Column - 1;
                    int lowerRightColumn = piece.Column + 1;

                    if (lowerRow <= 7 && lowerLeftColumn >= 0)
                    {
                        Piece targetPiece = chessBoard[lowerRow, lowerLeftColumn];

                        if (targetPiece != null && targetPiece.Color != piece.Color)
                            moves.Add((lowerRow, lowerLeftColumn));
                    }

                    if (lowerRow <= 7 && lowerRightColumn <= 7)
                    {
                        Piece targetPiece = chessBoard[lowerRow, lowerRightColumn];

                        if (targetPiece != null && targetPiece.Color != piece.Color)
                            moves.Add((lowerRow, lowerRightColumn));
                    }
                }
            }

            if (piece.Type == PieceType.Rook || piece.Type == PieceType.Queen)
            {
                for (int row = piece.Row - 1; row >= 0; row--)
                {
                    Piece targetPiece = chessBoard[row, piece.Column];

                    if (targetPiece == null)
                        moves.Add((row, piece.Column));
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                            moves.Add((row, piece.Column));

                        break;
                    }
                }

                for (int row = piece.Row + 1; row <= 7; row++)
                {
                    Piece targetPiece = chessBoard[row, piece.Column];

                    if (targetPiece == null)
                        moves.Add((row, piece.Column));
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                            moves.Add((row, piece.Column));

                        break;
                    }
                }

                for (int column = piece.Column - 1; column >= 0; column--)
                {
                    Piece targetPiece = chessBoard[piece.Row, column];

                    if (targetPiece == null)
                        moves.Add((piece.Row, column));
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                            moves.Add((piece.Row, column));

                        break;
                    }
                }

                for (int column = piece.Column + 1; column <= 7; column++)
                {
                    Piece targetPiece = chessBoard[piece.Row, column];

                    if (targetPiece == null)
                        moves.Add((piece.Row, column));
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                            moves.Add((piece.Row, column));

                        break;
                    }
                }
            }

            if (piece.Type == PieceType.Bishop || piece.Type == PieceType.Queen)
            {
                for (int row = piece.Row - 1, column = piece.Column - 1; row >= 0 && column >= 0; row--, column--)
                {
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                        moves.Add((row, column));
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                            moves.Add((row, column));

                        break;
                    }
                }

                for (int row = piece.Row - 1, column = piece.Column + 1; row >= 0 && column <= 7; row--, column++)
                {
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                        moves.Add((row, column));
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                            moves.Add((row, column));

                        break;
                    }
                }

                for (int row = piece.Row + 1, column = piece.Column - 1; row <= 7 && column >= 0; row++, column--)
                {
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                        moves.Add((row, column));
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                            moves.Add((row, column));

                        break;
                    }
                }

                for (int row = piece.Row + 1, column = piece.Column + 1; row <= 7 && column <= 7; row++, column++)
                {
                    Piece targetPiece = chessBoard[row, column];

                    if (targetPiece == null)
                        moves.Add((row, column));
                    else
                    {
                        if (targetPiece.Color != piece.Color)
                            moves.Add((row, column));

                        break;
                    }
                }
            }

            if (piece.Type == PieceType.Knight)
            {
                int[,] knightMoves =
                {
            { -2, -1 }, { -2, 1 },
            { -1, -2 }, { -1, 2 },
            { 1, -2 }, { 1, 2 },
            { 2, -1 }, { 2, 1 }
        };

                for (int i = 0; i < 8; i++)
                {
                    int targetRow = piece.Row + knightMoves[i, 0];
                    int targetColumn = piece.Column + knightMoves[i, 1];

                    if (targetRow >= 0 && targetRow <= 7 && targetColumn >= 0 && targetColumn <= 7)
                    {
                        Piece targetPiece = chessBoard[targetRow, targetColumn];

                        if (targetPiece == null || targetPiece.Color != piece.Color)
                            moves.Add((targetRow, targetColumn));
                    }
                }
            }

            if (piece.Type == PieceType.King)
            {
                for (int rowDifference = -1; rowDifference <= 1; rowDifference++)
                {
                    for (int columnDifference = -1; columnDifference <= 1; columnDifference++)
                    {
                        if (rowDifference == 0 && columnDifference == 0)
                            continue;

                        int targetRow = piece.Row + rowDifference;
                        int targetColumn = piece.Column + columnDifference;

                        if (targetRow >= 0 && targetRow <= 7 && targetColumn >= 0 && targetColumn <= 7)
                        {
                            Piece targetPiece = chessBoard[targetRow, targetColumn];

                            if (targetPiece == null || targetPiece.Color != piece.Color)
                                moves.Add((targetRow, targetColumn));
                        }
                    }
                }
            }

            return moves;
        }

        private bool HasAnyLegalMove(PieceColor color)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Piece piece = chessBoard[row, column];

                    if (piece != null && piece.Color == color)
                    {
                        List<(int Row, int Column)> moves = GetLegalMoves(piece);

                        for (int i = 0; i < moves.Count; i++)
                        {
                            (int targetRow, int targetColumn) = moves[i];

                            if (MoveLeavesKingInCheck(piece, targetRow, targetColumn) == false)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool Checkmate(PieceColor color)
        {
            if (IsKingInCheck(color) == false)
                return false;

            if (HasAnyLegalMove(color))
                return false;

            return true;
        }

        private async Task SendUsername(string username, TcpClient client)
        {
            //MessageBox.Show("SendUsername başladı: " + username);

            JObject jsonObj = new JObject();

            jsonObj["Type"] = "Username";
            jsonObj["Data"] = username;

            string json = jsonObj.ToString();

            byte[] jsonByteArr = Encoding.UTF8.GetBytes(json);

            NetworkStream stream = client.GetStream();

            await stream.WriteAsync(jsonByteArr, 0, jsonByteArr.Length);

        }

        private void ChangeUIForBlackPiece()
        {
            ChessBoardUI.LayoutTransform = new RotateTransform(180);

            foreach (Border square in ChessBoardUI.Children)
            {
                if (square.Child is Image image)
                {
                    image.RenderTransformOrigin = new Point(0.5, 0.5);
                    image.RenderTransform = new RotateTransform(180);
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            client = new TcpClient();

            await client.ConnectAsync("127.0.0.1", 5000);

            stream = client.GetStream();

            await SendUsername(playerName, client);

            await ListenServer();
        }

        //private async Task ReadColorMessage(NetworkStream stream) // Çünkü async bir metodun tamamlanmasını takip edebilmek istiyoruz.
        //{
        //    byte[] buffer = new byte[1024];

        //    int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);

        //    string message = Encoding.UTF8.GetString(buffer, 0, byteCount);

        //    JObject jsonObj = JObject.Parse(message);

        //    string type = jsonObj["Type"].ToString();
        //    string data = jsonObj["Data"].ToString();

        //    if (type == "Color")
        //    {
        //        if (data == "White")
        //        {
        //            return;
        //        }
        //        else if (data == "Black")
        //        {
        //            ChangeUIForBlackPiece();
        //        }
        //    }
        //}

        //public async Task<string> ReadEnemyName(NetworkStream stream)
        //{
        //    byte[] buffer = new byte[1024];

        //    int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);

        //    string message = Encoding.UTF8.GetString(buffer, 0, byteCount);

        //    JObject json = JObject.Parse(message);

        //    string type = json["Type"].ToString();
        //    string enemyName = json["Data"].ToString();

        //    return enemyName;
        //}

        private async Task ListenServer()
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (byteCount == 0)
                    break;

                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);

                JObject json = JObject.Parse(message);

                string type = json["Type"].ToString();

                if (type == "Color")
                {
                    string color = json["Data"].ToString();

                    if (color == "White")
                    {
                        playerColor = PieceColor.White;
                    }
                    else if (color == "Black")
                    {
                        playerColor = PieceColor.Black;

                        ChangeUIForBlackPiece();
                    }
                }
                else if (type == "EnemyName")
                {
                    enemyName = json["Data"].ToString();

                    EnemyName.Content = enemyName;
                }
                else if (type == "PieceMove")
                {
                    string pieceName = json["Piece"].ToString();

                    int fromRow = (int)json["FromRow"];
                    int fromColumn = (int)json["FromColumn"];

                    int toRow = (int)json["ToRow"];
                    int toColumn = (int)json["ToColumn"];

                    Piece piece = chessBoard[fromRow, fromColumn];

                    if (piece == null)
                    {
                        MessageBox.Show("Gönderilen başlangıç karesinde taş bulunamadı.");
                        continue;
                    }

                    if (piece.Type.ToString() == pieceName)
                    {
                        bool isCastling = piece.Type == PieceType.King && Math.Abs(toColumn - piece.Column) == 2;

                        if (isCastling)
                        {
                            CastlingMessage(piece, toRow, toColumn);
                        }
                        else
                        {
                            MovePieceMessage(piece, toRow, toColumn);
                        }

                        ChangeTurn();
                    }
                    else
                    {
                        MessageBox.Show("Tür Eşleşmedi");
                    }
                }
            }
        }

        private void MovePieceMessage(Piece piece, int targetRow, int targetColumn)
        {
            int oldIndex = piece.Row * 8 + piece.Column;

            Border square = ChessBoardUI.Children[oldIndex] as Border;

            int targetIndex = targetRow * 8 + targetColumn;

            Border targetSquare = ChessBoardUI.Children[targetIndex] as Border;

            Piece targetPiece = chessBoard[targetRow, targetColumn];

            if (targetPiece != null)
                targetPiece.IsEaten = true;

            Image image = square.Child as Image;

            square.Child = null;

            targetSquare.Child = image;

            chessBoard[piece.Row, piece.Column] = null;

            piece.Row = targetRow;
            piece.Column = targetColumn;

            piece.DidFirstMove = true;

            chessBoard[targetRow, targetColumn] = piece;

            ResetAllColors();
        }

        private async Task SendMoveData(TcpClient client, string username, string enemyname, PieceType pieceType, int oldRow, int oldColumn, int targetRow, int targetColumn)
        {
            JObject jsonObject = new JObject();

            jsonObject["Type"] = "PieceMove";
            jsonObject["User"] = username;
            jsonObject["Piece"] = pieceType.ToString();

            jsonObject["FromRow"] = oldRow;
            jsonObject["FromColumn"] = oldColumn;

            jsonObject["ToRow"] = targetRow;
            jsonObject["ToColumn"] = targetColumn;

            jsonObject["ToEnemy"] = enemyname;

            byte[] jsonByteArr = Encoding.UTF8.GetBytes(jsonObject.ToString());

            NetworkStream stream = client.GetStream();

            await stream.WriteAsync(jsonByteArr, 0, jsonByteArr.Length);
        }


        private void CastlingMessage(Piece piece, int targetRow, int targetColumn)
        {
            if (piece.Color == PieceColor.White)
            {
                if (targetRow == 7 && targetColumn == 2)
                {
                    // Beyaz uzun rok
                    Piece rook = chessBoard[7, 0];

                    Border rookSquare = ChessBoardUI.Children[rook.Row * 8 + rook.Column] as Border;
                    Border kingSquare = ChessBoardUI.Children[piece.Row * 8 + piece.Column] as Border;

                    Image rookImage = rookSquare.Child as Image;
                    Image kingImage = kingSquare.Child as Image;

                    rookSquare.Child = null;
                    kingSquare.Child = null;

                    Border rookNewSquare = ChessBoardUI.Children[7 * 8 + 3] as Border;
                    Border kingNewSquare = ChessBoardUI.Children[7 * 8 + 2] as Border;

                    rookNewSquare.Child = rookImage;
                    kingNewSquare.Child = kingImage;

                    chessBoard[7, 0] = null;
                    chessBoard[piece.Row, piece.Column] = null;

                    rook.Row = 7;
                    rook.Column = 3;
                    rook.DidFirstMove = true;

                    piece.Row = 7;
                    piece.Column = 2;
                    piece.DidFirstMove = true;

                    chessBoard[7, 3] = rook;
                    chessBoard[7, 2] = piece;
                }
                else if (targetRow == 7 && targetColumn == 6)
                {
                    // Beyaz kısa rok
                    Piece rook = chessBoard[7, 7];

                    Border rookSquare = ChessBoardUI.Children[rook.Row * 8 + rook.Column] as Border;
                    Border kingSquare = ChessBoardUI.Children[piece.Row * 8 + piece.Column] as Border;

                    Image rookImage = rookSquare.Child as Image;
                    Image kingImage = kingSquare.Child as Image;

                    rookSquare.Child = null;
                    kingSquare.Child = null;

                    Border rookNewSquare = ChessBoardUI.Children[7 * 8 + 5] as Border;
                    Border kingNewSquare = ChessBoardUI.Children[7 * 8 + 6] as Border;

                    rookNewSquare.Child = rookImage;
                    kingNewSquare.Child = kingImage;

                    chessBoard[7, 7] = null;
                    chessBoard[piece.Row, piece.Column] = null;

                    rook.Row = 7;
                    rook.Column = 5;
                    rook.DidFirstMove = true;

                    piece.Row = 7;
                    piece.Column = 6;
                    piece.DidFirstMove = true;

                    chessBoard[7, 5] = rook;
                    chessBoard[7, 6] = piece;
                }
            }
            else if (piece.Color == PieceColor.Black)
            {
                if (targetRow == 0 && targetColumn == 2)
                {
                    // Siyah uzun rok
                    Piece rook = chessBoard[0, 0];

                    Border rookSquare = ChessBoardUI.Children[rook.Row * 8 + rook.Column] as Border;
                    Border kingSquare = ChessBoardUI.Children[piece.Row * 8 + piece.Column] as Border;

                    Image rookImage = rookSquare.Child as Image;
                    Image kingImage = kingSquare.Child as Image;

                    rookSquare.Child = null;
                    kingSquare.Child = null;

                    Border rookNewSquare = ChessBoardUI.Children[0 * 8 + 3] as Border;
                    Border kingNewSquare = ChessBoardUI.Children[0 * 8 + 2] as Border;

                    rookNewSquare.Child = rookImage;
                    kingNewSquare.Child = kingImage;

                    chessBoard[0, 0] = null;
                    chessBoard[piece.Row, piece.Column] = null;

                    rook.Row = 0;
                    rook.Column = 3;
                    rook.DidFirstMove = true;

                    piece.Row = 0;
                    piece.Column = 2;
                    piece.DidFirstMove = true;

                    chessBoard[0, 3] = rook;
                    chessBoard[0, 2] = piece;
                }
                else if (targetRow == 0 && targetColumn == 6)
                {
                    // Siyah kısa rok
                    Piece rook = chessBoard[0, 7];

                    Border rookSquare = ChessBoardUI.Children[rook.Row * 8 + rook.Column] as Border;
                    Border kingSquare = ChessBoardUI.Children[piece.Row * 8 + piece.Column] as Border;

                    Image rookImage = rookSquare.Child as Image;
                    Image kingImage = kingSquare.Child as Image;

                    rookSquare.Child = null;
                    kingSquare.Child = null;

                    Border rookNewSquare = ChessBoardUI.Children[0 * 8 + 5] as Border;
                    Border kingNewSquare = ChessBoardUI.Children[0 * 8 + 6] as Border;

                    rookNewSquare.Child = rookImage;
                    kingNewSquare.Child = kingImage;

                    chessBoard[0, 7] = null;
                    chessBoard[piece.Row, piece.Column] = null;

                    rook.Row = 0;
                    rook.Column = 5;
                    rook.DidFirstMove = true;

                    piece.Row = 0;
                    piece.Column = 6;
                    piece.DidFirstMove = true;

                    chessBoard[0, 5] = rook;
                    chessBoard[0, 6] = piece;
                }
            }
        }

    }
}