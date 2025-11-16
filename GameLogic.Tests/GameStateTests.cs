using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
