namespace GameLogic
{
    public class GameState
    {
        private BoardField[,] board = new BoardField[12, 12];
        public Dictionary<Player, int> beatenPawns { get; private set; } = new Dictionary<Player, int>() {
            { Player.red, 0 },
            { Player.white, 0 }
        };
        public Player currentPlayer { get; private set; }


        public GameState()
        {
            InitBoard();
            currentPlayer = Player.red;
        }

        private void InitBoard()
        {
            for (int c = 0; c < 12; c++)
            {
                board[0, 1] = new BoardField(Player.white, 2);
                board[0, 2] = new BoardField(Player.white, 2);
                board[0, 3] = new BoardField(Player.white, 2);
                board[0, 4] = new BoardField(Player.white, 2);

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

        public bool SetBoardField(Position pos, Player player, int amount)
        {
            if (pos.row < 0 || pos.row > 1 || pos.col < 0 || pos.col > 11) return false;
            board[pos.row, pos.col] = new BoardField(player, amount);
            return true;
        }

        public BoardField GetBoardField(Position pos)
        {
            if (pos.row < 0 || pos.row > 1 || pos.col < 0 || pos.col > 11) return new BoardField(Player.none, 0);
            return board[pos.row, pos.col];
        }

        public void SwitchPlayer()
        {
            currentPlayer = currentPlayer == Player.red ? Player.white : Player.red;
        }

        public Player GetOppositePlayer()
        {
            return currentPlayer == Player.red ? Player.white : Player.red;
        }

        private bool IsEmpty(Position pos)
        {
            if (pos.row < 0 || pos.row > 1 || pos.col < 0 || pos.col > 11) return false;
            if (board[pos.row, pos.col].player == Player.none) return true;
            return false;
        }

        public bool IsCurrPlayerHere(Position pos)
        {
            return GetBoardField(pos).player == currentPlayer;
        }

        private bool CanMove(Position pos)
        {
            if (pos.row < 0 || pos.row > 1 || pos.col < 0 || pos.col > 11) return false;
            if (this.IsEmpty(pos)) return true;
            if (board[pos.row, pos.col].player == currentPlayer) return true;
            if (board[pos.row, pos.col].player != currentPlayer && board[pos.row, pos.col].amount == 1) return true;
            return false;
        }

        private Position? CalculateNextField(Position pos, int amount)
        {
            // jeżeli amount jest spoza 1-6 to zwracamy null'a
            if (amount < 1 || amount > 6) return null;

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

        public IEnumerable<Move> GetPossibleMoves(Position pos, int firstDice, int secondDice)
        {
            List<Move> possibleMoves = new List<Move>();

            Position? nextFieldFirstDice = CalculateNextField(pos, firstDice); 
            Position? nextFieldSecondDice = CalculateNextField(pos, secondDice);

            if (nextFieldFirstDice != null && CanMove(nextFieldFirstDice)) possibleMoves.Add(new Move(pos, nextFieldFirstDice, 1));
            if (nextFieldSecondDice != null && CanMove(nextFieldSecondDice)) possibleMoves.Add(new Move(pos, nextFieldSecondDice, 2));

            if (possibleMoves.Count == 2
                && possibleMoves[0].to.row == possibleMoves[1].to.row
                && possibleMoves[0].to.col == possibleMoves[1].to.col
            ) return possibleMoves.Take(1).ToList().Select(m => { m.SetDice(0); return m; });

            return possibleMoves;
        }

        public IEnumerable<Move> GetPossibleBackOnBoardMoves(int firstDice, int secondDice)
        {
            List<Move> possibleMoves = new List<Move>();
            int row = currentPlayer == Player.red ? 0 : 1;

            if (CanMove(new Position(row, firstDice - 1)))
            {
                possibleMoves.Add(new Move(null, new Position(row, firstDice - 1), 1));
            }

            if (firstDice != secondDice && CanMove(new Position(row, secondDice - 1)))
            {
                possibleMoves.Add(new Move(null, new Position(row, secondDice - 1), 2));
            }

            return possibleMoves;
        }

        public bool IsAnyMove(int firstDice, int secondDice)
        {
            if (beatenPawns[currentPlayer] > 0)
            {
                return GetPossibleBackOnBoardMoves(firstDice, secondDice).Count() > 0;
            }

            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 12; c++)
                {
                    Position from = new Position(r, c);
                    if (GetBoardField(from).player == currentPlayer
                        && GetPossibleMoves(from, firstDice, secondDice).Count() > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Move GetLongestMove(Position? from, Position to, Position to2)
        {
            // wyprowadzamy zbite piony
            if (from == null)
            {
                return new Move(from, to.col > to2.col ? to : to2);
            }

            Position finalTo;
            if (currentPlayer == Player.red)
            {
                if (from.row == 0)
                {
                    if (to.row == 1 && to2.row == 0) finalTo = to;
                    else if (to.row == 0 && to2.row == 1) finalTo = to2;
                    else if (to.row == 0 && to2.row == 0) finalTo = to.col > to2.col ? to : to2;
                    else finalTo = to.col < to2.col ? to : to2;
                }
                else finalTo = to.col < to2.col ? to : to2;
            }
            else
            {
                if (from.row == 1)
                {
                    if (to.row == 0 && to2.row == 1) finalTo = to;
                    else if (to.row == 1 && to2.row == 0) finalTo = to2;
                    else if (to.row == 1 && to2.row == 1) finalTo = to.col > to2.col ? to : to2;
                    else finalTo = to.col < to2.col ? to : to2;
                }
                else finalTo = to.col < to2.col ? to : to2;
            }

            return new Move(from, finalTo);
        }

        public void MakeMove(Move move)
        {
            // from
            if (move.from != null)
            {
                int restAmount = GetBoardField(move.from).amount - 1;
                SetBoardField(move.from, restAmount > 0 ? currentPlayer : Player.none, restAmount);
            }
            else
            {
                beatenPawns[currentPlayer]--;
            }

            // to
            if (GetBoardField(move.to).player != currentPlayer && GetBoardField(move.to).player != Player.none)
            {
                // bicie
                SetBoardField(move.to, currentPlayer, 1);
                beatenPawns[GetOppositePlayer()]++;
            }
            else
            {
                SetBoardField(move.to, currentPlayer, GetBoardField(move.to).amount + 1);
            }
        }
    }
}
