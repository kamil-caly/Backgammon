namespace GameLogic
{
    public class BoardField
    {
        public Player player { get; }
        public int amount { get; }

        public BoardField(Player player, int amount)
        {
            this.player = player;
            if (player == Player.none) this.amount = 0;
            else this.amount = amount;
        }
    }
}
