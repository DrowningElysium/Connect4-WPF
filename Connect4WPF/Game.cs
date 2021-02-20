using System;
using System.Windows.Media;

namespace Connect4WPF
{
    class Game
    {
        private int _width;
        private int _height;

        private Token[,] _grid;
        private Color _currentColor;

        public event EventHandler WinnerAnnounced;
        public event EventHandler GridUpdated;

        // Default game size.
        public Game(): this(7, 6)
        {
        }

        public Game(int width, int height)
        {
            this._width = width;
            this._height = height;

            this.Reset();
        }

        public Color GetCurrentPlayerColor()
        {
            return this._currentColor;
        }

        public Token GetGridToken(int x, int y)
        {
            return this._grid[x, y];
        }

        public void Reset()
        {
            this._grid = new Token[this._width, this._height];
            this._currentColor = Colors.Red;

            for (int x = 0; x < this._width; x++)
            {
                for (int y = 0; y < this._height; y++)
                {
                    this._grid[x, y] = new Token(x, y);
                }
            }

            // Throw an event so display logic is not part of game logic.
            this.AnnounceGridUpdate();
        }

        public void PlayColumn(int x)
        {
            for (int y = 0; y < this._height; y++)
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

                    return;
                }

                this.SwitchPlayer();
                return; // Early return so we don't fully loop and set everything in the row to the color.
            }
        }

        public void UpdateTokenColor(Token token, Color color)
        {
            token.SetColor(color);
            // Throw an event so display logic is not part of game logic.
            this.AnnounceGridUpdate();
        }

        public void SwitchPlayer()
        {
            if (this._currentColor == Colors.Red)
            {
                this._currentColor = Colors.Yellow;
                return;
            }

            this._currentColor = Colors.Red;
        }

        public bool DoesTokenWin(Token token)
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

        public int GetAmountOnTopLeftSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, -1, 1);
        }

        public int GetAmountOnLeftSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, -1, 0);
        }

        public int GetAmountOnBottomLeftSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, -1, -1);
        }

        public int GetAmountOnBottomSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, 0, -1);
        }

        public int GetAmountOnBottomRightSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, 1, -1);
        }

        public int GetAmountOnRightSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, 1, 0);
        }

        public int GetAmountOnTopRightSide(Token token)
        {
            return this.GetAmountOfSameTokensInDirection(token, 1, 1);
        }

        public int GetAmountOfSameTokensInDirection(Token token, int xIncrease, int yIncrease)
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
            if (x < 0 || x >= this._width - 1)
            {
                return true; 
            }

            if (y < 0 || y >= this._height - 1)
            {
                return true;
            }

            return false;
        }

        public bool IsInsideGameArea(int x, int y)
        {
            return !this.IsOutsideGameArea(x, y);
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
