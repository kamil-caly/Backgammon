namespace GameLogic.Tests
{
    public class GameStateTests
    {
        [Theory]
        [InlineData(0, 0, Player.red, 2)]
        [InlineData(0, 5, Player.white, 5)]
        [InlineData(0, 7, Player.white, 3)]
        [InlineData(0, 11, Player.red, 5)]
        [InlineData(1, 0, Player.white, 2)]
        [InlineData(1, 5, Player.red, 5)]
        [InlineData(1, 7, Player.red, 3)]
        [InlineData(1, 11, Player.white, 5)]

        [InlineData(0, 1, Player.none, 0)]
        [InlineData(0, 2, Player.none, 0)]
        [InlineData(0, 3, Player.none, 0)]
        [InlineData(0, 4, Player.none, 0)]
        [InlineData(0, 6, Player.none, 0)]
        [InlineData(0, 8, Player.none, 0)]
        [InlineData(0, 9, Player.none, 0)]
        [InlineData(0, 10, Player.none, 0)]

        [InlineData(1, 1, Player.none, 0)]
        [InlineData(1, 2, Player.none, 0)]
        [InlineData(1, 3, Player.none, 0)]
        [InlineData(1, 4, Player.none, 0)]
        [InlineData(1, 6, Player.none, 0)]
        [InlineData(1, 8, Player.none, 0)]
        [InlineData(1, 9, Player.none, 0)]
        [InlineData(1, 10, Player.none, 0)]
        public void InitBoard_ReturnsCorrectInitialBoard(int row, int col, Player player, int amount)
        {
            // arrange

            var gameState = new GameState();
            var board = gameState.CopyBoard();

            // assert

            Assert.Equal(player, gameState.GetBoardField(new Position(row, col)).player);
            Assert.Equal(amount, gameState.GetBoardField(new Position(row, col)).amount);
        }

        [Fact]
        public void CopyBoard_ReturnsDeepCopyOfBoard()
        {
            // arrange

            var gameState = new GameState();
            var boardCopy = gameState.CopyBoard();

            // act

            boardCopy[0, 0] = new BoardField(Player.white, 10);

            // assert

            Assert.Equal(Player.red, gameState.GetBoardField(new Position(0, 0)).player);
            Assert.Equal(2, gameState.GetBoardField(new Position(0, 0)).amount);
        }

        [Fact]
        public void SetBoardField_ForPosition0_7_UpdatesBoardField()
        {
            // arrange

            var gameState = new GameState();
            var position = new Position(0, 7);
            var player = Player.white;
            int amount = 2;

            // act

            var result = gameState.SetBoardField(position, player, amount);

            // assert

            Assert.Equal(Player.white, gameState.GetBoardField(position).player);
            Assert.Equal(2, gameState.GetBoardField(position).amount);
            Assert.True(result);
        }

        [Fact]
        public void SetBoardField_ForInvalidPosition_ReturnsFalse()
        {
            // arrange

            var gameState = new GameState();
            var position = new Position(2, 0); // invalid row
            var player = Player.white;
            int amount = 2;

            // act

            var result = gameState.SetBoardField(position, player, amount);

            // assert

            Assert.False(result);
        }

        [Fact]
        public void GetBoardField_ForPosition1_7_ReturnsCorrectBoardField()
        {
            // arrange

            var gameState = new GameState();
            var position = new Position(1, 7);

            // act

            var result = gameState.GetBoardField(position);

            // assert

            Assert.Equal(Player.red, result.player);
            Assert.Equal(3, result.amount);
        }

        [Fact]
        public void GetBoardField_ForInvalidPosition_ReturnsPlayerNoneAnd0Amount()
        {
            // arrange

            var gameState = new GameState();
            var position = new Position(2, 0);

            // act

            var result = gameState.GetBoardField(position);

            // assert

            Assert.Equal(Player.none, result.player);
            Assert.Equal(0, result.amount);
        }

        [Fact]
        public void SwitchPlayer_ChangesCurrentPlayer()
        {
            // arrange

            var gameState = new GameState();
            var initialPlayer = gameState.currentPlayer;

            // act

            gameState.SwitchPlayer();
            var newPlayer = gameState.currentPlayer;

            // assert

            Assert.NotEqual(initialPlayer, newPlayer);
            Assert.True(newPlayer == Player.red || newPlayer == Player.white);
        }

        [Fact]
        public void GetOppositePlayer_ReturnsCorrectOpponent()
        {
            // arrange

            var gameState = new GameState();

            // act
            var initialPlayer = gameState.currentPlayer;
            var oppositePlayer = gameState.GetOppositePlayer();

            // assert

            Assert.NotEqual(initialPlayer, oppositePlayer);
            Assert.True(oppositePlayer == Player.red || oppositePlayer == Player.white);
        }

        [Fact]
        public void IsCurrPlayerHere_ForCurrentPlayerPosition_ReturnsTrue()
        {
            // arrange

            var gameState = new GameState();
            var position = new Position(0, 0);
            if (gameState.currentPlayer != Player.red)
            {
                gameState.SwitchPlayer();
            }

            // act

            var result = gameState.IsCurrPlayerHere(position);

            // assert

            Assert.True(result);
        }

        [Fact]
        public void IsGameOver_For14WhiteAnd13RedPawnsInCourt_ReturnsFalse()
        {
            // arrange

            var gameState = new GameState();
            gameState.courtPawns[Player.white] = 14;
            gameState.courtPawns[Player.red] = 13;

            // act

            var result = gameState.IsGameOver();

            // assert

            Assert.False(result);
        }

        [Theory]
        [InlineData(1, 2)]
        public void GetPossibleMoves_ForInitialStateAndDice1And2ForRedPlayer_ReturnsCorrectNormalMoves(int dice1, int dice2)
        {
            // arrange

            var gameState = new GameState();
            if (gameState.currentPlayer != Player.red)
            {
                gameState.SwitchPlayer();
            }

            // act

            var possibleMoves = gameState.GetPossibleMoves(new Position(0, 0), dice1, dice2).ToList();

            // assert

            Assert.Contains(possibleMoves, move => move.from!.row == 0 && move.from.col == 0 && move.to.row == 0 && move.to.col == 1);
            Assert.Contains(possibleMoves, move => move.from!.row == 0 && move.from.col == 0 && move.to.row == 0 && move.to.col == 2);
        }

        [Theory]
        [InlineData(6, 3)]
        public void GetPossibleMoves_ForChangedGameBoardAndDice1And2ForWhitePlayer_ReturnsCorrectNormalMoves(int dice1, int dice2)
        {
            // arrange

            var gameState = new GameState();
            gameState.SetBoardField(new Position(0, 6), Player.white, 1);
            gameState.SetBoardField(new Position(0, 11), Player.red, 1);
            if (gameState.currentPlayer != Player.white)
            {
                gameState.SwitchPlayer();
            }

            // act

            var possibleMoves = gameState.GetPossibleMoves(new Position(0, 6), dice1, dice2).ToList();

            // assert

            Assert.NotNull(possibleMoves);
            Assert.Single(possibleMoves);
            Assert.Contains(possibleMoves, move => move.from!.row == 0 && move.from.col == 6 && move.to.row == 0 && move.to.col == 3);
        }

        [Theory]
        [InlineData(5, 4)]
        [InlineData(3, 3)]
        public void GetPossibleMoves_ForBlockedPosition_ReturnsNoMoves(int dice1, int dice2)
        {
            // arrange

            var gameState = new GameState();
            gameState.SetBoardField(new Position(1, 9), Player.white, 2);
            gameState.SetBoardField(new Position(1, 8), Player.white, 3);
            gameState.SetBoardField(new Position(1, 7), Player.white, 4);
            if (gameState.currentPlayer != Player.red)
            {
                gameState.SwitchPlayer();
            }

            // act

            var possibleMoves = gameState.GetPossibleMoves(new Position(0, 11), dice1, dice2).ToList();

            // assert

            Assert.NotNull(possibleMoves);
            Assert.Empty(possibleMoves);
        }

        [Theory]
        [InlineData(2, 6)]
        public void GetPossibleMoves_ForChangedGameBoardAndDice1And2ForWhitePlayer_ReturnsCorrectInHomeMoves(int dice1, int dice2)
        {
            // arrange

            var gameState = new GameState();
            gameState.SetBoardField(new Position(0, 5), Player.none, 0);
            gameState.SetBoardField(new Position(0, 7), Player.none, 0);
            gameState.SetBoardField(new Position(1, 0), Player.none, 0);
            gameState.SetBoardField(new Position(1, 11), Player.none, 0);
            gameState.SetBoardField(new Position(0, 3), Player.white, 1);
            gameState.SetBoardField(new Position(0, 5), Player.white, 1);
            if (gameState.currentPlayer != Player.white)
            {
                gameState.SwitchPlayer();
            }

            // act

            var possibleMoves1 = gameState.GetPossibleMoves(new Position(0, 3), dice1, dice2).ToList();
            var possibleMoves2 = gameState.GetPossibleMoves(new Position(0, 5), dice1, dice2).ToList();

            // assert

            Assert.NotNull(possibleMoves1);
            Assert.Single(possibleMoves1);
            Assert.Contains(possibleMoves1, move => move.from!.row == 0 && move.from.col == 3 && move.to.row == 0 && move.to.col == 1);

            Assert.NotNull(possibleMoves2);
            Assert.Equal(2, possibleMoves2.Count);
            Assert.Contains(possibleMoves2, move => move.from!.row == 0 && move.from.col == 5 && move.to.row == 0 && move.to.col == 3);
            Assert.Contains(possibleMoves2, move => move.from!.row == 0 && move.from.col == 5 && move.to.row == 0 && move.to.col == -1);
        }

        [Theory]
        [InlineData(2, 5)]
        public void GetPossibleBackOnBoardMoves_ForChangedGameBoardAndDice1And2ForWhitePlayer_ReturnsOneCorrctMove(int dice1, int dice2)
        {
            // arrange

            var gameState = new GameState();
            if (gameState.currentPlayer != Player.white)
            {
                gameState.SwitchPlayer();
            }
            gameState.SetBoardField(new Position(1, 4), Player.red, 3);

            // act

            var result = gameState.GetPossibleBackOnBoardMoves(dice1, dice2);

            // assert

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, move => move.from == null && move.to.row == 1 && move.to.col == 1);
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(1, 1)]
        [InlineData(6, 6)]
        public void GetPossibleBackOnBoardMoves_ForChangedGameBoardAndDice1And2ForRedPlayer_ReturnsNoMoves(int dice1, int dice2)
        {
            // arrange

            var gameState = new GameState();
            if (gameState.currentPlayer != Player.red)
            {
                gameState.SwitchPlayer();
            }
            gameState.SetBoardField(new Position(0, 2), Player.white, 3);
            gameState.SetBoardField(new Position(0, 0), Player.white, 2);
            gameState.SetBoardField(new Position(0, 5), Player.white, 6);

            // act

            var result = gameState.GetPossibleBackOnBoardMoves(dice1, dice2);

            // assert

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(1, 6)]
        [InlineData(2, 4)]
        [InlineData(5, 5)]
        public void IsAnyMove_ForChangedGameBoardAndDice1AndDice2ForRedPlayer_ReturnsTrue(int dice1, int dice2)
        {
            // arrange

            var gameState = new GameState();
            if (gameState.currentPlayer != Player.red)
            {
                gameState.SwitchPlayer();
            }

            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 12; c++)
                {
                    gameState.SetBoardField(new Position(r, c), Player.white, 2);
                }
            }

            gameState.SetBoardField(new Position(0, 6), Player.red, 2);
            gameState.SetBoardField(new Position(0, 7), Player.red, 2);
            gameState.SetBoardField(new Position(1, 11), Player.red, 2);
            gameState.SetBoardField(new Position(0, 8), Player.red, 2);
            gameState.SetBoardField(new Position(0, 10), Player.red, 2);
            gameState.SetBoardField(new Position(0, 11), Player.red, 2);
            

            // act

            var result = gameState.IsAnyMove(dice1, dice2);

            // assert

            Assert.True(result);
        }

        [Theory]
        [InlineData(1, 6)]
        [InlineData(2, 4)]
        [InlineData(5, 5)]
        public void IsAnyMove_ForChangedGameBoardAndDice1AndDice2ForWhitePlayer_ReturnsFalse(int dice1, int dice2)
        {
            // arrange

            var gameState = new GameState();
            if (gameState.currentPlayer != Player.white)
            {
                gameState.SwitchPlayer();
            }

            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 12; c++)
                {
                    gameState.SetBoardField(new Position(r, c), Player.red, 2);
                }
            }

            gameState.SetBoardField(new Position(0, 0), Player.white, 2);
            gameState.SetBoardField(new Position(0, 10), Player.white, 2);
            gameState.SetBoardField(new Position(1, 5), Player.white, 2);

            // act

            var result = gameState.IsAnyMove(dice1, dice2);

            // assert

            Assert.False(result);
        }

        private List<(Position[], int)> _getLongesMoveParamsRed = new List<(Position[], int)>()
        {
            (new Position[] { new Position(0, 7), new Position(0, 10), new Position(1, 10) }, 1),
            (new Position[] { new Position(0, 7), new Position(0, 9), new Position(0, 8) }, 0),
            (new Position[] { new Position(0, 10), new Position(1, 8), new Position(1, 9) }, 0),
        };

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetLongestMove_ForInitGameBoardAndDice1AndDice2ForRedPlayer_ReturnsCorrectedMove(int idx)
        {
            // arrange

            var gameState = new GameState();
            if (gameState.currentPlayer != Player.red)
            {
                gameState.SwitchPlayer();
            }
            var from = _getLongesMoveParamsRed[idx].Item1[0];
            var to = _getLongesMoveParamsRed[idx].Item1[1];
            var to2 = _getLongesMoveParamsRed[idx].Item1[2];
            var which = _getLongesMoveParamsRed[idx].Item2;

            // act

            var result = gameState.GetLongestMove(from, to, to2);

            // assert

            Assert.NotNull(result);
            Assert.Equal(result.from!.row, from.row);
            Assert.Equal(result.from!.col, from.col);
            Assert.Equal(result.to.row, which == 0 ? to.row : to2.row);
            Assert.Equal(result.to.col, which == 0 ? to.col : to2.col);
        }

        private List<(Position[], int)> _getLongesMoveParamsWhite = new List<(Position[], int)>()
        {
            (new Position[] { new Position(0, 7), new Position(0, 5), new Position(0, 4) }, 1),
            (new Position[] { new Position(1, 7), new Position(1, 9), new Position(0, 10) }, 1),
            (new Position[] { new Position(1, 7), new Position(1, 11), new Position(1, 9) }, 0),
        };

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void GetLongestMove_ForInitGameBoardAndDice1AndDice2ForWhitePlayer_ReturnsCorrectedMove(int idx)
        {
            // arrange

            var gameState = new GameState();
            if (gameState.currentPlayer != Player.white)
            {
                gameState.SwitchPlayer();
            }
            var from = _getLongesMoveParamsWhite[idx].Item1[0];
            var to = _getLongesMoveParamsWhite[idx].Item1[1];
            var to2 = _getLongesMoveParamsWhite[idx].Item1[2];
            var which = _getLongesMoveParamsWhite[idx].Item2;

            // act

            var result = gameState.GetLongestMove(from, to, to2);

            // assert

            Assert.NotNull(result);
            Assert.Equal(result.from!.row, from.row);
            Assert.Equal(result.from!.col, from.col);
            Assert.Equal(result.to.row, which == 0 ? to.row : to2.row);
            Assert.Equal(result.to.col, which == 0 ? to.col : to2.col);
        }

        [Fact]
        public void GetLongestMove_ForChangedGameBoardAnd1BeatenRedPawnAndDice1AndDice2ForRedPlayer_ReturnsCorrectedMove()
        {
            // arrange

            var gameState = new GameState();
            gameState.beatenPawns[Player.red] = 1;
            if (gameState.currentPlayer != Player.red)
            {
                gameState.SwitchPlayer();
            }
            Position? from = null;
            Position to = new Position(0, 1);
            Position to2 = new Position(0, 3);

            // act

            var result = gameState.GetLongestMove(from, to, to2);

            // assert

            Assert.NotNull(result);
            Assert.Null(result.from);
            Assert.Equal(result.to.row, to2.row);
            Assert.Equal(result.to.col, to2.col);
        }

        [Fact]
        public void MakeMove_ForChangedBoardForRedPlayerAndCapturingWhitePlayer_ReturnsCorrectlyUpdatedBoard()
        {
            // arrange

            var gameState = new GameState();
            if (gameState.currentPlayer != Player.red)
            {
                gameState.SwitchPlayer();
            }
            gameState.SetBoardField(new Position(0, 9), Player.white, 1);
            Move move = new Move(new Position(0, 7), new Position(0, 9));

            // act

            gameState.MakeMove(move);

            // assert

            Assert.Equal(Player.red, gameState.GetBoardField(new Position(0, 7)).player);
            Assert.Equal(2, gameState.GetBoardField(new Position(0, 7)).amount);
            Assert.Equal(Player.red, gameState.GetBoardField(new Position(0, 9)).player);
            Assert.Equal(1, gameState.GetBoardField(new Position(0, 9)).amount);
            Assert.Equal(1, gameState.beatenPawns[Player.white]);
            Assert.Equal(0, gameState.beatenPawns[Player.red]);
        }

        [Fact]
        public void MakeMove_ForChangedBoardForWhitePlayerMovingToEmptyField_ReturnsCorrectlyUpdatedBoard()
        {
            // arrange

            var gameState = new GameState();
            if (gameState.currentPlayer != Player.white)
            {
                gameState.SwitchPlayer();
            }
            gameState.SetBoardField(new Position(0, 7), Player.white, 6);
            Move move = new Move(new Position(0, 7), new Position(0, 9));

            // act

            gameState.MakeMove(move);

            // assert

            Assert.Equal(Player.white, gameState.GetBoardField(new Position(0, 7)).player);
            Assert.Equal(5, gameState.GetBoardField(new Position(0, 7)).amount);
            Assert.Equal(Player.white, gameState.GetBoardField(new Position(0, 9)).player);
            Assert.Equal(1, gameState.GetBoardField(new Position(0, 9)).amount);
        }

        [Fact]
        public void MakeMove_ForChangedBoardForWhitePlayerMovingToCourt_ReturnsCorrectlyUpdatedBoard()
        {
            // arrange

            var gameState = new GameState();
            if (gameState.currentPlayer != Player.white)
            {
                gameState.SwitchPlayer();
            }

            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 12; c++)
                {
                    gameState.SetBoardField(new Position(r, c), Player.none, 0);
                }
            }

            gameState.SetBoardField(new Position(0, 4), Player.white, 1);
            gameState.SetBoardField(new Position(0, 3), Player.red, 5);

            Move move = new Move(new Position(0, 4), new Position(0, -1));

            // act

            gameState.MakeMove(move);

            // assert

            Assert.Equal(Player.red, gameState.GetBoardField(new Position(0, 3)).player);
            Assert.Equal(5, gameState.GetBoardField(new Position(0, 3)).amount);
            Assert.Equal(Player.none, gameState.GetBoardField(new Position(0, 4)).player);
            Assert.Equal(0, gameState.GetBoardField(new Position(0, 4)).amount);
            Assert.Equal(1, gameState.courtPawns[Player.white]);
        }
    }
}
