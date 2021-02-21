using System;
using System.Windows.Media;

namespace Connect4WPF
{
    class Game
    {
        private int _columns;
        private int _rows;

        private Token[,] _grid;
        private Color _currentColor;

        private bool _gameHasWinner;

        public event EventHandler WinnerAnnounced;
        public event EventHandler GridUpdated;

        // Default game size.
        public Game(): this(7, 6)
        {
        }

        public Game(int columns, int rows)
        {
            this._columns = columns;
            this._rows = rows;

            this.Reset();
        }

        // For Cloning
        public Game(int columns, int rows, Token[,] grid, Color currentColor, bool gameHasWinner)
        {
            this._columns = columns;
            this._rows = rows;
            this._grid = grid;
            this._currentColor = currentColor;
            this._gameHasWinner = gameHasWinner;
        }

        public Color GetCurrentPlayerColor()
        {
            return this._currentColor;
        }

        public int GetColumnAmount()
        {
            return this._columns;
        }

        public int GetRowAmount()
        {
            return this._rows;
        }

        public Token[,] GetGrid()
        {
            return this._grid;
        }

        public Token GetGridToken(int x, int y)
        {
            return this._grid[x, y];
        }

        public void Reset()
        {
            this._grid = new Token[this._columns, this._rows];
            this._currentColor = Colors.Red;
            this._gameHasWinner = false;

            for (int x = 0; x < this._columns; x++)
            {
                for (int y = 0; y < this._rows; y++)
                {
                    this._grid[x, y] = new Token(x, y);
                }
            }

            // Throw an event so display logic is not part of game logic.
            this.AnnounceGridUpdate();
        }

        public bool HasGameEnded()
        {
            if (this.HasGameWinner())
            {
                return true;
            }

            return ! this.SpotLeftOnGameBoard();
        }

        public bool HasGameWinner()
        {
            return this._gameHasWinner;
        }

        public Color GetWinner()
        {
            if (this._gameHasWinner)
            {
                return this._currentColor;
            }

            return Colors.White;
        }

        public bool SpotLeftOnGameBoard()
        {
            // For efficiency we start with the top row.
            for (int y = this._rows - 1; y >= 0; y--)
            {
                for (int x = 0; x < this._columns; x++)
                {
                    if (! this._grid[x, y].IsSet())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool PlayColumn(int x)
        {
            for (int y = 0; y < this._rows; y++)
            {
                Token current = _grid[x, y];
                if (current.IsSet())
                {
                    continue;
                }

                this.UpdateTokenColor(current, this._currentColor);

                if (this.DoesTokenWin(current))
                {
                    // Throw an event so display logic is not part of game logic.
                    this.AnnounceWinner(current);
                    this._gameHasWinner = true;

                    return true;
                }

                this.SwitchPlayer();
                return  true; // Early return so we don't fully loop and set everything in the row to the color.
            }
            return false;
        }

        public void RemoveTokenFromColumn(int x)
        {
            for (int y = this._rows - 1; y >= 0; y--)
            {
                Token current = this._grid[x, y];
                if (current.IsSet())
                {
                    current.Reset();

                    return;
                }
            }
        }

        private void UpdateTokenColor(Token token, Color color)
        {
            token.SetColor(color);

            // Throw an event so display logic is not part of game logic.
            this.AnnounceGridUpdate();
        }

        private void SwitchPlayer()
        {
            this.SwitchPlayerColor();
        }

        private void SwitchPlayerColor()
        {
            if (this._currentColor == Colors.Red)
            {
                this._currentColor = Colors.Yellow;
                return;
            }

            this._currentColor = Colors.Red;
        }

        private bool DoesTokenWin(Token token)
        {
            // Checks for 3 instead of 4 as we don't count the token we are checking.
            if (this.GetAmountOnBottomSide(token) >= 3)
            {
                return true;
            }

            if ((this.GetAmountOnTopLeftSide(token) + this.GetAmountOnBottomRightSide(token)) >= 3)
            {
                return true;
            }

            if ((this.GetAmountOnLeftSide(token) + this.GetAmountOnRightSide(token)) >= 3)
            {
                return true;
            }

            if ((this.GetAmountOnBottomLeftSide(token) + this.GetAmountOnTopRightSide(token)) >= 3)
            {
                return true;
            }

            // Game continues;
            return false;
        }

        private int GetAmountOnTopLeftSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, -1, 1);
        }

        private int GetAmountOnLeftSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, -1, 0);
        }

        private int GetAmountOnBottomLeftSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, -1, -1);
        }

        private int GetAmountOnBottomSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, 0, -1);
        }

        private int GetAmountOnBottomRightSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, 1, -1);
        }

        private int GetAmountOnRightSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, 1, 0);
        }

        private int GetAmountOnTopRightSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, 1, 1);
        }

        private int GetAmountOfSameTokensInDirection(Token token, int xIncrease, int yIncrease)
        {
            if (this.IsOutsideGameArea(token.GetX() + xIncrease, token.GetY() + yIncrease))
            {
                return 0;
            }

            Token tokenToCheck = _grid[token.GetX() + xIncrease, token.GetY() + yIncrease];
            if (! token.GetColor().Equals(tokenToCheck.GetColor()))
            {
                return 0;
            }

            return this.GetAmountOfSameTokensInDirection(tokenToCheck, xIncrease, yIncrease) + 1;
        }

        public bool IsOutsideGameArea(int x, int y)
        {
            if (x < 0 || x >= this._columns)
            {
                return true; 
            }

            if (y < 0 || y >= this._rows)
            {
                return true;
            }

            return false;
        }

        private bool IsInsideGameArea(int x, int y)
        {
            return ! this.IsOutsideGameArea(x, y);
        }

        public virtual void AnnounceWinner(Token token)
        {
            EventHandler handler = this.WinnerAnnounced;
            if (handler != null)
            {
                handler(token, EventArgs.Empty);
            }
        }

        public virtual void AnnounceGridUpdate()
        {
            EventHandler handler = this.GridUpdated;
            if (handler != null)
            {
                handler(this._grid, EventArgs.Empty);
            }
        }
    }
}
