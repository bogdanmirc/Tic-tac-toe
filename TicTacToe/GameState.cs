using System;

namespace TicTacToe
{
    public class GameState
    {
        public Player[,] GameGrid { get;private set; } //3x3 game grid as 2 dimensional player array
        public Player CurrentPlayer { get; private set; } // which player's turn it is
        public int TurnsPassed { get; private set; } // how many turns have passed
        public bool GameOver { get; private set; } //whether the game is end or not

        public event Action<int, int> MoveMade; //the two integers will be the row and the column of the square that was marked with the move
        public event Action<GameResult> GameEnded; //this even will supply receivers with a game result object when the game has ended
        public event Action GameRestarted; //restarted event which is raised when the game is started over

        public GameState()   //this is a constructor
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            GameOver = false;
        }

        private bool CanMakeMove(int r, int c)
        {
            return !GameOver && GameGrid[r, c] == Player.None;
        }

        private bool IsGridFull()
        {
            return TurnsPassed == 9; //the grid must be full if 9 turns have passed
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        }
        
        /*we can write in like this or we can convert to noditional expresiion  
         if (CurrentPlayer == Player.X)
            {
                CurrentPlayer = Player.O;
            }
        else
            {
                CurrentPlayer = Player.X;
            } */


        private bool AreSquaresMarked((int, int)[] squares, Player player)
        {
            foreach ((int r, int c) in squares)
            {
                if (GameGrid[r, c] != player)
                {
                    return false;
                }
            }

            return true; //we must know when a plyer has won the game
        }

        private bool DidMoveWin(int r, int c, out WinInfo winInfo)
        {
            (int, int)[] row = new[] { (r, 0), (r, 1), (r, 2) };
            (int, int)[] col = new[] { (0, c), (1, c), (2, c) };
            (int, int)[] mainDiag = new[] { (0, 0), (1, 1), (2, 2), };
            (int, int)[] antiDiag = new[] { (0, 2), (1, 1), (2, 0), };
            // if every square in one of these arrays is marked by the current player then that player has won the game

            if (AreSquaresMarked(row, CurrentPlayer))
            {
                winInfo = new WinInfo { Type = WinType.Row, Number = r };
                return true;
            }

            if (AreSquaresMarked(col, CurrentPlayer))
            {
                winInfo = new WinInfo { Type = WinType.Column, Number = c };
                return true;
            }

            if (AreSquaresMarked(mainDiag, CurrentPlayer))
            {
                winInfo = new WinInfo { Type = WinType.MainDiagonal };
                return true;
            }

            if (AreSquaresMarked(antiDiag, CurrentPlayer))
            {
                winInfo = new WinInfo { Type = WinType.AntiDiagonal };
                return true;
            }

            winInfo = null;
            return false;
        }     

        private bool DidMoveEndGame(int r, int c, out GameResult gameResult)
        {
            if (DidMoveWin(r, c, out WinInfo winInfo)) 
            {
                gameResult = new GameResult { Winner = CurrentPlayer, WinInfo = winInfo };
                return true;
            }  // someone win and the game is finished

            if(IsGridFull())
            {
                gameResult = new GameResult { Winner = Player.None };
                return true;
            } // none of the player won but the game is finished

            gameResult = null;
            return false;  //the game continued
        }

        public void MakeMove(int r, int c)
        {
            if (!CanMakeMove(r, c))
            {
                return;
            }

            GameGrid[r, c] = CurrentPlayer;
            TurnsPassed++;

            if (DidMoveEndGame(r, c, out GameResult gameResult))
            {
                GameOver = true;
                MoveMade?.Invoke(r, c);
                GameEnded?.Invoke(gameResult);
                //if (MoveMade !=null)
                //{
                //    MoveMade(r, c);
                //}
            }
            else
            {
                SwitchPlayer();
                MoveMade?.Invoke(r,c);
            }
        }

        public void Reset()
        {
            GameGrid = new Player[3, 3];
            CurrentPlayer = Player.X;
            TurnsPassed = 0;
            GameOver = false;
            GameRestarted?.Invoke();

        } //here we replace the game grid with a new array which contains only player.none by default
    }
}
