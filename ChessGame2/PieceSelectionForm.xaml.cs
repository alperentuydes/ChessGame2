using ChessGame2.Assets;
using ChessGame2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChessGame2
{
    /// <summary>
    /// Interaction logic for PieceSelectionForm.xaml
    /// </summary>
    public partial class PieceSelectionForm : Window
    {

        public Piece piece;
        public int targetRow;
        public int targetColumn;
        public PieceType pieceType;
        public SelectedPiece selectedPiece;
        List<Grid> gridList = new List<Grid>();
        bool hasSelectedPiece = false;
        PieceColor pieceColor;
        PieceImages pieceImages = new PieceImages();

        public PieceSelectionForm(Piece piece, int targetRow, int targetColumn)
        {
            InitializeComponent();
            this.piece = piece;
            this.targetRow = targetRow;
            this.targetColumn = targetColumn;
            this.pieceColor = piece.Color;
            ImageByColor(piece.Color);
            foreach (Grid grid in GridRows.Children)
            {
                gridList.Add(grid);
            }
        }

        private void ImageByColor(PieceColor color)
        {
            if (color == PieceColor.White)
            {
                Border queenSquare = QueenImage;
                Border knightSquare = KnightImage;
                Border bishopSquare = BishopImage;
                Border rookSquare = RookImage;

                Image queenImage = new Image();
                queenImage.Source = pieceImages.BitmapWhiteQueen;
                queenImage.Stretch = Stretch.Uniform;

                Image knightImage = new Image();
                knightImage.Source = pieceImages.BitmapWhiteKnight;
                knightImage.Stretch = Stretch.Uniform;

                Image bishopImage = new Image();
                bishopImage.Source = pieceImages.BitmapWhiteBishop;
                bishopImage.Stretch = Stretch.Uniform;

                Image rookImage = new Image();
                queenImage.Source = pieceImages.BitmapWhiteRook;
                queenImage.Stretch = Stretch.Uniform;

                queenSquare.Child = queenImage;
                knightSquare.Child = knightImage;
                bishopSquare.Child = bishopImage;
                rookSquare.Child = rookImage;
            }
            else
            {
                Border queenSquare = QueenImage;
                Border knightSquare = KnightImage;
                Border bishopSquare = BishopImage;
                Border rookSquare = RookImage;

                Image queenImage = new Image();
                queenImage.Source = pieceImages.BitmapBlackQueen;
                queenImage.Stretch = Stretch.Uniform;

                Image knightImage = new Image();
                knightImage.Source = pieceImages.BitmapBlackKnight;
                knightImage.Stretch = Stretch.Uniform;

                Image bishopImage = new Image();
                bishopImage.Source = pieceImages.BitmapBlackBishop;
                bishopImage.Stretch = Stretch.Uniform;

                Image rookImage = new Image();
                rookImage.Source = pieceImages.BitmapBlackRook;
                rookImage.Stretch = Stretch.Uniform;

                queenSquare.Child = queenImage;
                knightSquare.Child = knightImage;
                bishopSquare.Child = bishopImage;
                rookSquare.Child = rookImage;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = (Grid)sender;
            Label label = grid.Children[1] as Label;
            string labelContent = label.Content.ToString();
            //List<Grid> grids = GridRows.Children as List<Grid>

            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].Background = null;
            }

            if (labelContent == "Queen")
            {
                pieceType = PieceType.Queen;
            }
            else if (labelContent == "Bishop")
            {
                pieceType = PieceType.Bishop;
            }
            else if (labelContent == "Knight")
            {
                pieceType = PieceType.Knight;
            }
            else if (labelContent == "Rook")
            {
                pieceType = PieceType.Rook;
            }

            grid.Background = Brushes.Orange;
            hasSelectedPiece = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!hasSelectedPiece)
                return;

            piece.Type = pieceType;
            piece.Row = targetRow;
            piece.Column = targetColumn;

            selectedPiece = new SelectedPiece();
            selectedPiece.Piece = piece;
            DialogResult = true;

            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult != true)
            {
                e.Cancel = true;
            }
        }
    }
}
