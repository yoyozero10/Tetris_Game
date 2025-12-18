using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;
using NAudio.Wave;

namespace Tetris_Game
{
    internal class GameState
    {
        public readonly AudioManager backgroundAudio;
        public readonly AudioManager gameOverAudio;
        public readonly AudioManager placeBlockAudio;
        public readonly AudioManager removeRowAudio;
        public readonly AudioManager levelUpAudio;
        private Block currentBlock;

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public int Level { get; set; } = 1;
        public const int UpLevel = 10;
        public int clearedRow = 0;
        public Block HeldBlock { get; private set; }
        public bool CamHold { get; private set; }
        public int[] BasePoint = new int[5] { 0, 100, 300, 500, 800 };

        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
            CamHold = true;

            backgroundAudio = new AudioManager("resources/audio/BackGroundAudio.wav");
            gameOverAudio = new AudioManager("resources/audio/GameOverAudio.wav");
            placeBlockAudio = new AudioManager("resources/audio/PlaceBlockAudio.wav");
            removeRowAudio = new AudioManager("resources/audio/RemoveRowAudio.wav");
            levelUpAudio = new AudioManager("resources/audio/LevelUpAudio.wav");
        }

        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    if (!BlockFits())
                        currentBlock.Move(-1, 0);
                }
            }
        }

        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column))
                    return false;
            }

            return true;
        }

        public void HoldBlock()
        {
            if (!CamHold)
                return;

            if (HeldBlock == null)
            {
                HeldBlock = currentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }

            CamHold = false;
        }

        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();

            if (!BlockFits())
                CurrentBlock.RotateCCW();
        }

        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCCW();

            if (!BlockFits())
                CurrentBlock.RotateCW();
        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
                CurrentBlock.Move(0, 1);
        }

        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
                CurrentBlock.Move(0, -1);
        }

        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        private void PlaceBlock()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            placeBlockAudio.Play();

            int rowsCleared = GameGrid.ClearFullRows();
            clearedRow += rowsCleared;

            if (clearedRow >= UpLevel)
            {
                Level++;
                clearedRow -= UpLevel;
                levelUpAudio.Play();
            }

            if (rowsCleared > 0)
            {
                Score += (BasePoint[rowsCleared] * Level);
                removeRowAudio.Play();
            }

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
                CamHold = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private int TileDropDistance(Position p)
        {
            int drop = 0;

            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TilePositions())
                drop = System.Math.Min(drop, TileDropDistance(p));

            return drop;
        }

        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }

        public void SetVolume(float volume)
        {
            backgroundAudio.SetVolume(volume);
            gameOverAudio.SetVolume(volume);
            placeBlockAudio.SetVolume(volume);
            removeRowAudio.SetVolume(volume);
            levelUpAudio.SetVolume(volume);
        }
    }
}
