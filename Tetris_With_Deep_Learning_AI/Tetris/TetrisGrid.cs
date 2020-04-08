using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

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
    public class TetrisGrid
    {
        public int width { get; private set; } = 10;
        public int maxHeight { get; private set; } // ?
        public IEnumerable<TetrisLine> getLines { get { return board; } }
        protected List<TetrisLine> board;
        public TetrisGrid(int maxHeight = 30)
        {
            this.maxHeight = maxHeight;
            Reset();
        }

        public void Reset()
        {
            board = new List<TetrisLine>();
        }

        public bool Get(Point point)
        {
            return Get(point.X, point.Y);
        }

        public bool Get(int x, int y)
        {
            while(board.Count - 1 < y)
                board.Add(new TetrisLine());

            return board[y].line[x];
        }

        // accepts 4,2 array which takes x, y position of 0, 1, 2, 3 block.
        public void SetMino(IEnumerable<Point> mino, Tetromino minoType = Tetromino.None)
        {
            if(mino.Count() != 4 || minoType == Tetromino.None)
                throw new InvalidOperationException();

            var maxY = mino.Max(i => i.Y);
            while(board.Count - 1 < maxY)
                board.Add(new TetrisLine());

            foreach(var point in mino)
            {
                var x = point.X;
                var y = point.Y;
                
                var currentLine = board[y];
                currentLine.line[x] = true;
                currentLine.minoType[x] = minoType;
                // 여기서 true에 true를 덮어쓰는지 체크를 하는게 좋지않을까
            }
        }
        public void UpdateBoard()
        {
            for(int y = 0; y < maxHeight;)
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