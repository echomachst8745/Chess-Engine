using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chess_Engine;

namespace Front_End
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Board board;

        Image? selectedPieceImage;
        bool isDraggingPiece;
        Point? selectedPieceInitalPosition;

        public MainWindow()
        {
            InitializeComponent();

            board = new Board();

            selectedPieceImage = null;
            isDraggingPiece = false;
            selectedPieceInitalPosition = null;

            DisplayPieces(board);
        }

        private void DisplayPieces(Board board)
        {
            PieceGrid.Children.Clear();

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    int squareIndex = Board.IndexFromFileRank(file, rank);
                    byte piece = board.Squares[squareIndex];
                    
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri($"Images/{(Piece.IsWhite(piece) ? "w" : "b")}{char.ToLower(Piece.PieceToSymbol(piece))}.png", UriKind.Relative));
                    image.MouseLeftButtonDown += Piece_MouseLeftButtonDown;
                    image.MouseMove += Piece_MouseMove;
                    image.MouseLeftButtonUp += (sender, e) => Piece_MouseLeftButtonUp(sender, e, board);
                    image.Tag = squareIndex;

                    Grid.SetColumn(image, file);
                    Grid.SetRow(image, 7 - rank);

                    PieceGrid.Children.Add(image);
                }
            }
        }

        private void Piece_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedPieceImage = sender as Image;

            if (selectedPieceImage != null)
            {
                Panel.SetZIndex(selectedPieceImage, int.MaxValue);
                isDraggingPiece = true;
                selectedPieceImage.CaptureMouse();
                selectedPieceInitalPosition = e.GetPosition(PieceGrid);
            }
        }

        private void Piece_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingPiece && selectedPieceImage != null && selectedPieceInitalPosition != null)
            {
                Point mousePosition = e.GetPosition(PieceGrid);
                TranslateTransform transform = new TranslateTransform(mousePosition.X - selectedPieceInitalPosition.Value.X, mousePosition.Y - selectedPieceInitalPosition.Value.Y);
                selectedPieceImage.RenderTransform = transform;
            }
        }

        private void Piece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e, Board board)
        {
            if (isDraggingPiece && selectedPieceImage != null)
            {
                Point mousePosition = e.GetPosition(PieceGrid);
                int file = (int)(mousePosition.X / (PieceGrid.ActualWidth / 8));
                int rank = 7 - (int)(mousePosition.Y / (PieceGrid.ActualHeight / 8));

                file = Math.Clamp(file, 0, 7);
                rank = Math.Clamp(rank, 0, 7);

                Move move = new Move()
                {
                    startSquareIndex = (int)selectedPieceImage.Tag,
                    endSquareIndex = Board.IndexFromFileRank(file, rank)
                };

                board.MakeMove(move);

                selectedPieceImage.RenderTransform = null;
                selectedPieceImage.ReleaseMouseCapture();
                isDraggingPiece = false;
                selectedPieceImage = null;
                selectedPieceInitalPosition = null;

                DisplayPieces(board);
            }
        }
    }
}