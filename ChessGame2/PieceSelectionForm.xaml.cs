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

        public PieceSelectionForm(Piece piece, int targetRow, int targetColumn)
        {
            InitializeComponent();
            this.piece = piece;
            this.targetRow = targetRow;
            this.targetColumn = targetColumn;
            foreach (Grid grid in GridRows.Children)
            {
                gridList.Add(grid);
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
