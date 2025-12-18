
namespace Tetris_Game
{
    internal class Position
    {
        public int Row {  get; set; }
        public int Column { get; set; }

        public Position(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }
    }
}
