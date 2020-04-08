using System;
using System.Collections.Generic;
using System.Linq;
using Tetris;
using Xunit;
using System.Drawing;

namespace XUnitTest_Tetris
{
    public class Test_CurrentTetromino
    {
        
        [Theory]
        [MemberData(nameof(MinoOffset))]
        public void GetPos_Should_Return_valid_offset_per_tetromino(Tetromino mino, Point offset, RotationState rotState, Point[] points)
        {
            var minoPiece = new CurrentTetrominoPiece(mino, offset, rotState);
            var expectedPos = minoPiece.GetPos();
            foreach(var pos in expectedPos)
            {
                Assert.Contains(pos, points);
            }
        }
        
        public static IEnumerable<object[]> MinoOffset()
        {
            yield return new object[]
            {
                Tetromino.L,
                new Point(11, 3),
                RotationState.Two,
                new Point[] { new Point(11, 3), new Point(10, 3), new Point(10, 2), new Point(12, 3) }
            };

            yield return new object[]
            {
                Tetromino.J,
                new Point(10, 8),
                RotationState.R,
                new Point[] { new Point(10, 8), new Point(10, 9), new Point(11, 9), new Point(10, 7) }
            };

            yield return new object[]
            {
                Tetromino.I,
                new Point(4, 4),
                RotationState.Zero,
                new Point[] { new Point(3, 4), new Point(4, 4), new Point(5, 4), new Point(6, 4) },
            };

            yield return new object[]
            {
                Tetromino.Z,
                new Point(4, 4),
                RotationState.L,
                new Point[] { new Point(4, 4), new Point(4, 5), new Point(3, 4), new Point(3, 3) }
            };
        }
        
    }
}