namespace GameLogic
{
    public class Move
    {
        public Position from { get; }
        public Position to { get; }

        public Move(Position from, Position to)
        {
            this.from = from;
            this.to = to;
        }
    }
}
