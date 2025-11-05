namespace GameLogic
{
    public class GameState
{
        private BoardField[,] board = new BoardField[12, 12];

        private Move move;
        public Player currentPlayer { get; private set; }

        public GameState()
        {
            InitBoard();
            move = new Move(board);
            currentPlayer = Player.red;
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
                else if (c == 8)
                {
                    board[1, c] = new BoardField(Player.none, 0);
                    board[0, c] = new BoardField(Player.red, 3);
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

        //public bool setBoardField(int pos.row, int pos.col, Player player, int amount = 0)
        //{
        //    if (pos.row < 0 || pos.row > 1 || pos.col < 0 || pos.col > 11) return false;
        //    board[pos.row, pos.col] = new BoardField(player, amount);
        //    return true;
        //}

        public void SwitchPlayer()
        {
            currentPlayer = currentPlayer == Player.red ? Player.white : Player.red;
        }

        public BoardField GetBoardField(Position pos)
        {
            if (pos.row < 0 || pos.row > 1 || pos.col < 0 || pos.col > 11) return new BoardField(Player.none, 0);
            return board[pos.row, pos.col];
        }

        private bool IsEmpty(Position pos)
        {
            if (pos.row < 0 || pos.row > 1 || pos.col < 0 || pos.col > 11) return false;
            if (board[pos.row, pos.col].player == Player.none) return true;
            return false;
        }

        private bool CanMove(Position pos)
        {
            if (this.IsEmpty(pos)) return true;
            if (board[pos.row, pos.col].player == currentPlayer) return true;
            if (board[pos.row, pos.col].player != currentPlayer && board[pos.row, pos.col].amount == 1) return true;
            return false;
        }

        private Position? CalculateNextField(Position pos, int amount)
        {
            if ((currentPlayer == Player.red && pos.row == 0) || (currentPlayer == Player.white && pos.row == 1))
            {
                int newCol = pos.col + amount;
                if (newCol < 12)
                {
                    return new Position(pos.row, newCol);
                }
                else
                {
                    int newRow = currentPlayer == Player.red ? 1 : 0;
                    newCol = 12 - (newCol - 11);
                    return new Position(newRow, newCol);
                }
            }
            else
            {
                int newCol = pos.col - amount;
                if (newCol >= 0)
                {
                    return new Position(pos.row, newCol);
                } 
                else
                {
                    // wyjście poza plansze
                    return null;
                }
            }
        }

        public IEnumerable<Position> GetPossibleMoves(Position pos, int firstDice, int secondDice)
        {
            List<Position> possibleMoves = new List<Position>();

            var nextFieldFirstDice = CalculateNextField(pos, firstDice); 
            var nextFieldSecondDice = CalculateNextField(pos, secondDice);

            if (nextFieldFirstDice != null && CanMove(nextFieldFirstDice)) possibleMoves.Add(nextFieldFirstDice);
            if (nextFieldSecondDice != null && CanMove(nextFieldSecondDice)) possibleMoves.Add(nextFieldSecondDice);

            if (possibleMoves.Count == 2
                && possibleMoves[0].row == possibleMoves[1].row
                && possibleMoves[0].col == possibleMoves[1].col
            ) return possibleMoves.Take(1).ToList();
            return possibleMoves;
        }
    }
}
