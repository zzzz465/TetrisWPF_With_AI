using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    public struct TetrisLine
    {
        public bool[] line;
        public Tetromino[] minoType;
        public TetrisLine(int width)
        {
            line = new bool[width];
            minoType = new Tetromino[width];
            for(int i = 0; i < width; i++)
                minoType[i] = Tetromino.None;
        }

        public bool isBlankLine()
        {
            if(Enumerable.All(line, x => !x))
                return true;
            else
                return false;
        }
    }
    public class TetrisMatrix
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public IEnumerable<TetrisLine> getLines { get { return board; } }
        protected List<TetrisLine> board;
        public TetrisMatrix(int height = 20, int width = 10)
        {
            this.width = width;
            this.height = height;
            Reset();
        }

        public void Reset()
        {
            board = new List<TetrisLine>();
            for(int i = 0; i < height; i++)
                board.Add(new TetrisLine(width));
        }

        public ref bool Get(int x, int y)
        {
            if(x < width)
                return ref board[y].line[x];
            else
                throw new Exception();
        }

        // accepts 4,2 array which takes x, y position of 0, 1, 2, 3 block.
        public void Set(int[,] mino, Tetromino minoType = Tetromino.None)
        {
            for(int i = 0; i < 4; i++)
            {
                var x = mino[i,0];
                var y = mino[i,1];
                
                var currentLine = board[y];
                currentLine.line[x] = true;
                currentLine.minoType[x] = minoType;
                // 여기서 true에 true를 덮어쓰는지 체크를 하는게 좋지않을까
            }
        }
        public void UpdateBoard()
        {
            for(int y = 0; y < height;)
            {
                var current = board[y];
                if (Enumerable.All(current.line, x => x))
                {
                    this.board.Remove(current);
                    this.board.Add(new TetrisLine(width));
                    continue;
                }
                else
                    y++;
            }
        }
    }
}