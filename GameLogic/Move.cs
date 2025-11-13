namespace GameLogic
{
    public class Move
    {
        public Position? from { get; }
        public Position to { get; }
        public int dice { get; private set; }

        public Move(Position? from, Position to, int dice = 0)
        {
            this.from = from;
            this.to = to;
            this.dice = dice;
        }

        public void SetDice(int dice)
        {
            this.dice = dice;
        }
    }
}
