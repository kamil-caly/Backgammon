namespace GameLogic
{
    public class GameState
    {
        private BoardField[,] board = new BoardField[2, 12];
        private Random rand;
        public Dictionary<Player, int> beatenPawns { get; private set; }
        public Dictionary<Player, int> courtPawns { get; private set; }
        public Player currentPlayer { get; private set; }


        public GameState()
        {
            InitBoard();
            beatenPawns = new Dictionary<Player, int>() { { Player.red, 0 }, { Player.white, 0 } };
            courtPawns = new Dictionary<Player, int> { { Player.red, 0 }, { Player.white, 0 } };
            rand = new Random();
            currentPlayer = GetRandomPlayer();
        }

        private void InitBoard()
        {
            for (int c = 0; c < 12; c++)
            {
                //board[1, 2] = new BoardField(Player.red, 2);
                //board[1, 3] = new BoardField(Player.red, 2);
                //board[1, 5] = new BoardField(Player.red, 2);

                //board[0, 1] = new BoardField(Player.white, 5);
                //board[0, 4] = new BoardField(Player.white, 2);
                //board[0, 5] = new BoardField(Player.white, 2);

                //if (board[0, c] == null) board[0, c] = new BoardField(Player.none, 0);
                //if (board[1, c] == null) board[1, c] = new BoardField(Player.none, 0);

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

        private BoardField[,] CopyBoard()
        {
            BoardField[,] copyBoard = new BoardField[2, 12];

            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 12; c++)
                {
                    copyBoard[r, c] = new BoardField(board[r, c].player, board[r, c].amount);
                }
            }

            return copyBoard;
        }

        private Player GetRandomPlayer()
        {
            return rand.Next(0, 2) == 0 ? Player.red : Player.white;
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

        private bool IsAllPawnsInHome()
        {
            if (beatenPawns[currentPlayer] > 0) return false;

            if (currentPlayer == Player.red)
            {
                for (int r = 0; r < 2; r++)
                {
                    for (int c = 0; c < 12; c++)
                    {
                        if (r == 1 && c < 6) continue;
                        if (IsCurrPlayerHere(new Position(r, c))) return false;
                    }
                }

                return true;
            }
            else
            {
                for (int r = 0; r < 2; r++)
                {
                    for (int c = 0; c < 12; c++)
                    {
                        if (r == 0 && c < 6) continue;
                        if (IsCurrPlayerHere(new Position(r, c))) return false;
                    }
                }

                return true;
            }
        }

        public bool IsGameOver()
        {
            return courtPawns[Player.red] == 15 || courtPawns[Player.white] == 15;
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
            if (IsAllPawnsInHome()) return GetPossibleInHomeMoves(pos, firstDice, secondDice);
            else return GetPossibleNormalMoves(pos, firstDice, secondDice);
        }

        private void DistinctMovesIfTwoTheSame(ref List<Move> moves)
        {
            if (moves.Count == 2
                && moves[0].to.row == moves[1].to.row
                && moves[0].to.col == moves[1].to.col
            ) moves = moves.Take(1).Select(m => { m.SetDice(0); return m; }).ToList();
        }

        private IEnumerable<Move> GetPossibleInHomeMoves(Position pos, int firstDice, int secondDice)
        {
            List<Move> possibleMoves = new List<Move>();
            bool isMorePawnsAtRight = false;

            foreach (var amount in new int[] { firstDice, secondDice })
            {
                // jeżeli amount jest z poza 1-6 to 'continue' bo kostka już wykorzystana
                if (amount < 0 || amount > 6) continue;
                int dice = firstDice == secondDice ? - 1 : amount == firstDice ? 1 : 2;

                if (pos.col + 1 - amount >= 0) possibleMoves.Add(new Move(pos, new Position(pos.row, pos.col - amount), dice));
                else
                {
                    for (int c = pos.col + 1; c < 6; c++)
                    {
                        if (IsCurrPlayerHere(new Position(pos.row, c)))
                        {
                            isMorePawnsAtRight = true;
                            break;
                        }
                    }

                    if (!isMorePawnsAtRight) possibleMoves.Add(new Move(pos, new Position(pos.row, -1), dice));
                }
            }

            DistinctMovesIfTwoTheSame(ref possibleMoves);
            return possibleMoves;
        }

        private void LeaveTheLongestMoveIfPossibleOnlyOneMoveAmoungTwo(ref List<Move> moves, int firstDice, int secondDice)
        {
            if (moves.Count <= 1 || firstDice == secondDice) return;
            
            var oryginalBoard = CopyBoard();
            var oryginalBeatenPawns = new Dictionary<Player, int>(beatenPawns);
            var oryginalCourtPawns = new Dictionary<Player, int>(courtPawns);

            var copyBoardForFirstMove = CopyBoard();
            var copyBeatenPawnsForFirstMove = new Dictionary<Player, int>(beatenPawns);
            var copyCourtPawnsForFirstMove = new Dictionary<Player, int>(courtPawns);
            var copyBoardForSecondMove = CopyBoard();
            var copyBeatenPawnsForSecondMove = new Dictionary<Player, int>(beatenPawns);
            var copyCourtPawnsForSecondMove = new Dictionary<Player, int>(courtPawns);

            // ruch z pierwszej kostki
            bool isAnyMoveAfterFirstMove = false;
            beatenPawns = copyBeatenPawnsForFirstMove;
            courtPawns = copyCourtPawnsForFirstMove;
            board = copyBoardForFirstMove;
            MakeMove(moves[0]);
            if (IsAnyMove(-1, secondDice)) isAnyMoveAfterFirstMove = true;

            // ruch z drugiej kostki
            bool isAnyMoveAfterSecondMove = false;
            beatenPawns = copyBeatenPawnsForSecondMove;
            courtPawns = copyCourtPawnsForSecondMove;
            board = copyBoardForSecondMove;
            MakeMove(moves[1]);
            if (IsAnyMove(firstDice, -1)) isAnyMoveAfterSecondMove = true;

            // sprawdzamy czy po zrobieniu ruchów z obu kostek nie można zrobić już żadnego innego ruchu
            // dla aktualnego gracza
            if (!isAnyMoveAfterFirstMove && !isAnyMoveAfterSecondMove)
            {
                // zostawiamy tylko ruch o większej liczbie oczek
                moves = firstDice > secondDice ? new List<Move> { moves[0] } : new List<Move> { moves[1] };
            }

            board = oryginalBoard;
            beatenPawns = oryginalBeatenPawns;
            courtPawns = oryginalCourtPawns;
        }

        private IEnumerable<Move> GetPossibleNormalMoves(Position pos, int firstDice, int secondDice)
        {
            List<Move> possibleMoves = new List<Move>();

            Position? nextFieldFirstDice = CalculateNextField(pos, firstDice);
            Position? nextFieldSecondDice = CalculateNextField(pos, secondDice);

            if (nextFieldFirstDice != null && CanMove(nextFieldFirstDice)) possibleMoves.Add(new Move(pos, nextFieldFirstDice, 1));
            if (nextFieldSecondDice != null && CanMove(nextFieldSecondDice)) possibleMoves.Add(new Move(pos, nextFieldSecondDice, 2));

            DistinctMovesIfTwoTheSame(ref possibleMoves);

            // Jeśli gracz może wykonać tylko jedno przesunięcie, ale o liczbę oczek z dowolnej z kostek,
            // wykonuje przesunięcie o większą liczbę oczek.
            LeaveTheLongestMoveIfPossibleOnlyOneMoveAmoungTwo(ref possibleMoves, firstDice, secondDice);

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
            // wychodzenie na dwór
            if (move.to.col == -1)
            {
                courtPawns[currentPlayer]++;
            }
            else
            {
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
}
