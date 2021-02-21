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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Connect4WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _gameWidth = 7;
        private int _gameHeight = 6;

        private Game _game;
        private Ellipse[,] _grid;

        private ArtificialIntelligence _ai;

        public MainWindow()
        {
            InitializeComponent();

            // Resize window based on variables
            this.Width = 42 * this._gameWidth;
            this.Height = 27 + 40 * (this._gameHeight + 1);

            this._game = new Game(this._gameWidth, this._gameHeight);
            this._grid = new Ellipse[this._gameWidth, this._gameHeight];

            this._ai = new ArtificialIntelligence(this._game);

            this.UpdateCurrentPlayer();
            this.DrawGrid();

            this._game.GridUpdated += UpdateGrid;
            this._game.WinnerAnnounced += GameEnded;
        }

        private void GameEnded(object sender, EventArgs e)
        {
            Token token = (Token)sender;

            string winner = "Red";
            if (token.GetColor() == Colors.Yellow)
            {
                winner = "Yellow";
            }

            MessageBox.Show(winner + " has won!", "We got a winner!");
        }

        private void UpdateGrid(object sender, EventArgs e)
        {
            this.RedrawGrid();
        }

        private void Reset()
        {
            this._game.Reset();
            this.UpdateCurrentPlayer();
            this.RedrawGrid();

        }

        private void UpdateCurrentPlayer()
        {
            this.CurrentPlayer.Fill = new SolidColorBrush(this._game.GetCurrentPlayerColor());
        }

        private void DrawGrid()
        {
            this.CreateGridDefinitions();

            this.ProgramGrid.Children.Add(this.CreateBoard());

            for (int x = 0; x < this._gameWidth; x++)
            {
                // As we want to draw the first layer the lowest, we will start with drawing the last tokens.
                for (int y = this._gameHeight - 1; y >= 0; y--)
                {
                    Color color = this._game.GetGridToken(x, y).GetColor();
                    Ellipse rec = CreateGameToken(color);
                    this._grid[x, y] = rec;

                    // Draw the ellipse to the screen and put it in its spot on the grid.
                    Grid.SetColumn(rec, x);
                    Grid.SetRow(rec, this._gameHeight - 1 - y);
                    this.ProgramGrid.Children.Add(rec);
                }
            }
        }

        private void RedrawGrid()
        {
            for (int x = 0; x < this._gameWidth; x++)
            {
                // As we want to draw the first layer the lowest, we will start with drawing the last tokens.
                for (int y = this._gameHeight - 1; y >= 0; y--)
                {
                    Token token = this._game.GetGridToken(x, y);
                    Ellipse ell = this._grid[x, y];
                    ell.Fill = new SolidColorBrush(token.GetColor());
                }
            }
        }

        private void CreateGridDefinitions()
        {
            this.CreateGridColumnDefinitions();
            this.CreateGridRowDefinitions();
        }

        private void CreateGridRowDefinitions()
        {
            for (int y = 0; y < this._gameHeight; y++)
            {
                RowDefinition row = new RowDefinition();
                this.ProgramGrid.RowDefinitions.Add(row);
            }

            // Add another one with Height auto. That way it will not be stretched.
            RowDefinition finalRow = new RowDefinition();
            finalRow.Height = GridLength.Auto;
            this.ProgramGrid.RowDefinitions.Add(finalRow);
        }

        private void CreateGridColumnDefinitions()
        {
            for (int x = 0; x < this._gameWidth; x++)
            {
                ColumnDefinition col = new ColumnDefinition();
                this.ProgramGrid.ColumnDefinitions.Add(col);
            }

            // Add another one with Width auto. That way it will not be stretched.
            ColumnDefinition finalCol = new ColumnDefinition();
            finalCol.Width = GridLength.Auto;
            this.ProgramGrid.ColumnDefinitions.Add(finalCol);
        }

        private Rectangle CreateBoard()
        {
            Rectangle board = new Rectangle();

            board.HorizontalAlignment = HorizontalAlignment.Left;
            board.VerticalAlignment = VerticalAlignment.Top;
            board.Fill = new SolidColorBrush(Colors.Blue);
            board.Height = this._gameHeight * 40;
            board.Width = this._gameWidth * 40;

            Grid.SetColumn(board, 0);
            Grid.SetRow(board, 0);
            Grid.SetColumnSpan(board, this._gameWidth);
            Grid.SetRowSpan(board, this._gameHeight);

            return board;
        }

        private Ellipse CreateGameToken(Color color)
        {
            Ellipse ell = new Ellipse();

            ell.Fill = new SolidColorBrush(color);
            ell.Width = 30; 
            ell.Height = 30;
            ell.HorizontalAlignment = HorizontalAlignment.Left;
            ell.VerticalAlignment = VerticalAlignment.Top;
            ell.Margin = new Thickness(5, 5, 5, 5);
            ell.MouseLeftButtonUp += ButtonEventHandler;

            return ell;
        }

        private void ButtonEventHandler(object sender, MouseButtonEventArgs e)
        {
            if (this._game.HasGameEnded())
            {
                return;
            }

            // Just to make sure we don't get stuck somewhere.
            //if (this._ai.IsAiTurn())
            //{
            //    this._ai.MakeMove();

            //    return;
            //}

            int x = Grid.GetColumn((UIElement) sender);
            this._game.PlayColumn(x);

            this.UpdateCurrentPlayer();

            //// Let AI also directly do it's thing
            //this._ai.MakeMove();

            //this.UpdateCurrentPlayer();
        }

        private void NewGameButtonEventHandler(object sender, RoutedEventArgs e)
        {
            this.Reset();
        }
    }
}
