using System;

namespace Tetris_Game
{
    internal class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };

        private readonly Random random = new Random();
        public Block NextBlock {  get; private set; }
        
        public BlockQueue()
        {
            NextBlock = RandomBlock();
        }

        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock;
            
            // Ensure next block is different from current
            Block nextBlock;
            do
            {
                nextBlock = RandomBlock();
            } while (block.Id == nextBlock.Id);
            
            NextBlock = nextBlock;
            return block;
        }
    }
}
