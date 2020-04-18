using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Tetris
{
    public class TetrisLine
    {
        public Tetromino[] line;
        public TetrisLine(int width = 10)
        {
            line = new Tetromino[width];
        }

        public bool isLineFilled()
        {
            foreach(var x in line)
                if(x == Tetromino.None)
                    return false;
            
            return true;
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
            while(board.Count - 1 <= y)
                board.Add(new TetrisLine());

            return board[y].line[x] != Tetromino.None;
        }

        // accepts 4,2 array which takes x, y position of 0, 1, 2, 3 block.
        public void Set(IEnumerable<Point> mino, Tetromino minoType = Tetromino.None)
        {
            var maxY = mino.Max(i => i.Y);
            while(board.Count - 1 < maxY)
                board.Add(new TetrisLine());

            foreach(var point in mino)
            {
                var x = point.X;
                var y = point.Y;
                
                var currentLine = board[y];
                currentLine.line[x] = minoType;
                // 여기서 true에 true를 덮어쓰는지 체크를 하는게 좋지않을까
            }
        }
        public void UpdateBoard()
        {
            for(int y = 0; y < board.Count;)
            {
                var current = board[y];
                if (current.isLineFilled())
                {
                    this.board.Remove(current);
                    this.board.Add(new TetrisLine(width));
                    continue;
                }
                else
                    y++;
            }
        }

        public bool TryPlace(IEnumerable<Point> points, Tetromino minoType)
        {
            if(points.Any(p => Get(p)))
                return false;

            var arePointsFloating = points.All(p => isFloating(p));

            if(arePointsFloating)
                return false;

            Set(points, minoType);

            return true;
        }

        public void AddGarbageLine(int garbageLineCount, int x_of_empty_line)
        {
            if(garbageLineCount < 1)
                throw new ArgumentOutOfRangeException("trashLine parameter should be more than 1 !");

            while(garbageLineCount-- > 0)
            {
                var garbageLine = new TetrisLine() { line = Enumerable.Repeat<Tetromino>(Tetromino.Garbage, 10).ToArray() };
                garbageLine.line[x_of_empty_line] = (int)Tetromino.None;
                board.Insert(0, garbageLine);
            }
        }

        public bool CanMinoExistHere(Point point)
        {
            if(point.X < 0 || point.X >= width)
                return false;
            
            if(point.Y < 0 || point.Y >= maxHeight)
                return false;
                
            return !Get(point);
        }

        public bool CanMinoExistHere(params Point[] points) => points.All(p => CanMinoExistHere(p));

        public bool CanMinoExistHere(IEnumerable<Point> points) => points.All(p => CanMinoExistHere(p));

        bool isFloating(Point p)
        {
            if(p.Y == 0)
                return false;

            if(Get(p.X, p.Y - 1) == false)
                return true;
            else
                return false;
        }
    }
}