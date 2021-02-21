﻿using System;
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
        private Random random = new Random();

        public ArtificialIntelligence(Game game)
        {
            this._game = game;
        }

        public void MakeMove()
        {
            if (! this.IsAiTurn())
            {
                return;
            }

            List<Tuple<int, int>> moves = new List<Tuple<int, int>>();
            for (int x = 0; x < this._game.GetColumnAmount(); x++)
            {
                moves.Add(Tuple.Create(x, this.MiniMax(6, this._game)));
            }

            int maxMoveScore = moves.Max(t => t.Item2);
            List<Tuple<int, int>> bestMoves = moves.Where(t => t.Item2 == maxMoveScore).ToList();
            this._game.PlayColumn(bestMoves[this.random.Next(0, bestMoves.Count())].Item1);
        }

        public bool IsAiTurn()
        {
            return this._game.GetCurrentPlayerColor().Equals(Colors.Yellow);
        }

        // Credits to: https://stackoverflow.com/a/36802499
        private int MiniMax(int depth, Game game)
        {
            if (depth <= 0 || ! game.SpotLeftOnGameBoard())
            {
                return 0;
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
            bool maximizingPlayer = this.IsMaximizingPlayer(game);
            int bestValue = maximizingPlayer ? -1 : 1;
            for (int x = 0; x < game.GetColumnAmount(); x++)
            {
                Game copy = (Game)game.Clone();
                int score = this.MiniMax(depth - 1, copy);
                bestValue = maximizingPlayer ? Math.Max(bestValue, score) : Math.Min(bestValue, score);
            }

            return bestValue;
        }

        private bool IsMaximizingPlayer(Game game)
        {
            return game.GetCurrentPlayerColor().Equals(Colors.Red);
        }
    }
}