using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Tetris
{
    public static class SRS
    {
        static readonly Dictionary<RotationState, Point[]> JLSTZ_Offset = new Dictionary<RotationState, Point[]>()
        {
            { RotationState.Zero, Enumerable.Repeat(new Point(0, 0), 5).ToArray() },
            { RotationState.R, new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, -1), new Point(0, 2), new Point(1, 2) } },
            { RotationState.Two, Enumerable.Repeat(new Point(0, 0), 5).ToArray() },
            { RotationState.L, new Point[] { new Point(0, 0), new Point(-1, 0), new Point(-1, -1), new Point(0, 2), new Point(-1, 2) } }
        };

        static readonly Dictionary<RotationState, Point[]> I_Offset = new Dictionary<RotationState, Point[]>()
        {
            { RotationState.Zero, new Point[] { new Point(0, 0), new Point(-1, 0), new Point(+2, 0), new Point(-1, 0), new Point(2, 0) } },
            { RotationState.R, new Point[] { new Point(-1, 0), new Point(0, 0), new Point(0, 0), new Point(0, 1), new Point(0, -2) } },
            { RotationState.Two, new Point[] { new Point(-1, 1), new Point(1, 1), new Point(-2, 1), new Point(1, 0), new Point(-2, 0) } },
            { RotationState.L, new Point[] { new Point(0, 1), new Point(0, 1), new Point(0, 1), new Point(0, -1), new Point(0, 2) } }
        };

        public static IEnumerable<Point> Translation(Tetromino mino, RotationState before, RotationState current)
        {
            if(isIllegalRotation(before, current))
                throw new InvalidOperationException();    

            switch(mino)
            {
                case Tetromino.O:
                {
                    yield return new Point(0, 0);
                    yield break;
                }

                case Tetromino.I:
                {
                    var beforeOffsets = I_Offset[before];
                    var curOffsets = I_Offset[current];
                    for(int i = 0; i < curOffsets.Length; i++)
                    {
                        int x1, x2, y1, y2;
                        x1 = beforeOffsets[i].X; y1 = beforeOffsets[i].Y; x2 = curOffsets[i].X; y2 = curOffsets[i].Y;
                        yield return new Point(x1 - x2, y1 - y2);
                    }
                    yield break;
                }

                case Tetromino.L:
                case Tetromino.J:
                case Tetromino.S:
                case Tetromino.Z:
                case Tetromino.T:
                {
                    var beforeOffsets = JLSTZ_Offset[before];
                    var curOffsets = JLSTZ_Offset[current];
                    for(int i = 0; i < curOffsets.Length; i++)
                    {
                        int x1, x2, y1, y2;
                        x1 = beforeOffsets[i].X; y1 = beforeOffsets[i].Y; x2 = curOffsets[i].X; y2 = curOffsets[i].Y;
                        yield return new Point(x1 - x2, y1 - y2);
                    }
                    yield break;
                }
            }
        }

        static bool isIllegalRotation(RotationState before, RotationState current)
        {
            if(before == RotationState.None || current == RotationState.None)
                return true;

            if(Math.Abs((int)current - (int)before) == 1)
                return false;
            
            if(before == RotationState.Zero && current == RotationState.L)
                return false;

            if(before == RotationState.L && current == RotationState.Zero)
                return false;

            return true;
        }
    }
}