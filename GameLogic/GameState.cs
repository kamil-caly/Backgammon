namespace GameLogic
{
    public class GameState
{
        private BoardField[,] board = new BoardField[12,12];
        public GameState()
        {
            InitBoard();
        }

        private void InitBoard()
        {
            for (int c = 0; c < 12; c++)
            {
                if (c == 0)
                {
                    board[0, c] = new BoardField(Player.red, 2);
                    board[1, c] = new BoardField(Player.white, 2);
                }
                else if (c == 5)
                {
                    board[0, c] = new BoardField(Player.white, 5);
                    board[1, c] = new BoardField(Player.red, 5);
                }
                else if (c == 7)
                {
                    board[0, c] = new BoardField(Player.white, 3);
                    board[1, c] = new BoardField(Player.red, 3);
                }
                else if (c == 11)
                {
                    board[0, c] = new BoardField(Player.red, 5);
                    board[1, c] = new BoardField(Player.white, 5);
                }
                else
                {
                    board[0, c] = new BoardField(Player.none, 0);
                    board[1, c] = new BoardField(Player.none, 0);
                }
            }
        }

        //public bool setBoardField(int row, int col, Player player, int amount = 0)
        //{
        //    if (row < 0 || row > 1 || col < 0 || col > 11) return false;
        //    board[row, col] = new BoardField(player, amount);
        //    return true;
        //}

        public bool isEmpty(int row, int col)
        {
            if (row < 0 || row > 1 || col < 0 || col > 11) return false;
            if (board[row, col].player == Player.none) return true;
            return false;
        }

        public bool canMove(int row, int col, Player player)
        {
            if (this.isEmpty(row, col)) return true;
            if (board[row, col].player == player) return true;
            if (board[row, col].player != player && board[row, col].amount == 1) return true;
            return false;
        }
    }
}
