using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Connect4WPF
{
    class ArtificialIntelligence
    {
        private Game _game;
        private Random _random = new Random();

        private int _depthLevel = 2;

        public ArtificialIntelligence(Game game)
        {
            this._game = game;
        }

        public void MakeMove()
        {
            if (this._game.HasGameEnded())
            {
                return;
            }


            if (! this.IsAiTurn())
            {
                return;
            }

            List<Tuple<int, int>> moves = new List<Tuple<int, int>>();
            for (int x = 0; x < this._game.GetColumnAmount(); x++)
            {
                if (! this._game.PlayColumn(x))
                {
                    continue;
                }

                int score = this.MiniMax(this._depthLevel, this._game, false);
                moves.Add(Tuple.Create(x, score));
                this._game.RemoveTokenFromColumn(x);
            }

            int maxMoveScore = moves.Max(t => t.Item2);
            List<Tuple<int, int>> bestMoves = moves.Where(t => t.Item2 == maxMoveScore).ToList();
            this._game.PlayColumn(bestMoves[this._random.Next(0, bestMoves.Count())].Item1);
        }

        public bool IsAiTurn()
        {
            return this._game.GetCurrentPlayerColor().Equals(Colors.Yellow);
        }

        // Credits to: https://stackoverflow.com/a/36802499
        private int MiniMax(int depth, Game game, bool maximizingPlayer)
        {

            if (depth <= 0)
            {
                return 0;
            }

            if (! game.SpotLeftOnGameBoard())
            {
                return -1000;
            }

            Color winner = game.GetWinner();
            if (winner.Equals(Colors.Yellow)) // AI
            {
                return depth;
            }

            if (winner.Equals(Colors.Red)) // Player
            {
                return -depth;
            }

            // Determine if we want to find the best score for the player or AI.
            int bestValue = 0;
            for (int x = 0; x < game.GetColumnAmount(); x++)
            {
                if (!this._game.PlayColumn(x))
                {
                    continue;
                }

                int score = this.MiniMax(depth - 1, game, ! maximizingPlayer);
                //Console.WriteLine("depth: " + depth + ", x: " + x + ", score: " + score + ", bestValue: " + bestValue + ", Min: " + Math.Min(bestValue, score) + ", Max: " + Math.Max(bestValue, score));
                bestValue = maximizingPlayer ? Math.Max(bestValue, score) : Math.Min(bestValue, score);
                //Console.WriteLine("BestValue Result: " + bestValue);
                this._game.RemoveTokenFromColumn(x);
            }

            return bestValue;
        }
    }
}
