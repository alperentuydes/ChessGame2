using ChessGame2.Assets;
using ChessGame2.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessGame2
{
    public partial class MainWindow : Window
    {
        Piece[,] chessBoard = new Piece[8, 8];

        Border oldSquare;
        Piece selectedPiece;

        PieceColor currentTurn = PieceColor.White;

        PieceImages pieceImages = new PieceImages();

        List<(int Row, int Column)> LegalMoves = new List<(int Row, int Column)>();

        BitmapImage bitmap;


        public MainWindow()
        {
            InitializeComponent();

            CreateChessBoard();
            SetChessPiece();
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

        private void square_Click(object sender, MouseButtonEventArgs e)
        {
            Border clickedSquare = sender as Border;

            var (row, column) = ((int, int))clickedSquare.Tag;

            if (selectedPiece != null && LegalMoves.Contains((row, column)))
            {
                MovePiece(sender, selectedPiece);

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

            if (clickedPiece.Color != currentTurn)
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
                // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
                // SOL

                if (piece.Column - 1 >= 0)
                {
                    int targetRow = piece.Row;
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

        private void MovePiece(object sender, Piece piece)
        {
            Border clickedSquare = sender as Border;

            var (row, column) = ((int, int))clickedSquare.Tag;

            int oldIndex = piece.Row * 8 + piece.Column;
            int newIndex = row * 8 + column;

            if (LegalMoves.Contains((row, column)))
            {
                if (MoveLeavesKingInCheck(piece, row, column))
                    return;

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

                PawnUpgrade(piece, row, column);

                piece.DidFirstMove = true;

                LegalMoves.Clear();

                ResetAllColors();

                ChangeTurn();
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

        private bool IsKingInCheck(PieceColor kingColor)
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

        //private bool MoveLeavesKingInCheck(Piece piece, int targetRow, int targetColumn)
        //{
        //    int oldRow = piece.Row;
        //    int oldColumn = piece.Column;

        //    Piece targetPiece = chessBoard[targetRow, targetColumn];

        //    chessBoard[oldRow, oldColumn] = null;
        //    chessBoard[targetRow, targetColumn] = piece;

        //    piece.Row = targetRow;
        //    piece.Column = targetColumn;

        //    bool kingInCheck = IsKingInCheck(piece.Color);

        //    chessBoard[oldRow, oldColumn] = piece;
        //    chessBoard[targetRow, targetColumn] = null;

        //    return kingInCheck;
        //}

        private bool MoveLeavesKingInCheck(Piece piece, int targetRow, int targetColumn)
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
    }
}
// - - - - EKLENECEKLER - - - - 
// 1. Ses eklenecek
// 2. Yediğimiz taşlar bizim isimlerin yanında gözükecek
// Yenen taraf için sevinç müziği olsun, yenilen taraf için ise üzgün müzik çalsın