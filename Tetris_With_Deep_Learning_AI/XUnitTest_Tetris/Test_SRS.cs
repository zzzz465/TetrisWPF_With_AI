using System;
using System.Collections.Generic;
using Xunit;
using Tetris;
using System.Drawing;

namespace XUnitTest_Tetris
{
    public class Test_SRS
    {
        [Theory]
        [MemberData(nameof(SRS_Translation_Cases))]
        public void SRS_Translation_Case_Check(Tetromino mino, RotationState before, RotationState current, Point[] expectedPoints)
        {
            var result = SRS.Translation(mino, before, current);
            foreach(var point in expectedPoints)
            {
                Assert.Contains(point, result);
            }
        }

        public static IEnumerable<object[]> SRS_Translation_Cases()
        {
            var I = Tetromino.I;
            yield return new object[]
            {
                I, RotationState.L, RotationState.Zero, 
                new Point[] { new Point(0, 1), new Point(1, 1), new Point(-2, 1) }
            };

            yield return new object[]
            {
                I, RotationState.R, RotationState.Two,
                new Point[] { new Point(0, -1), new Point(-1, -1), new Point(2, -1), new Point(-1, 1), new Point(2, -2) }
            };

            var T = Tetromino.T;

            yield return new object[]
            {
                T, RotationState.L, RotationState.Zero,
                new Point[] { new Point(0, 0), new Point(-1, 0), new Point(-1, -1), new Point(0, 2), new Point(-1, 2) }
            };

            yield return new object[]
            {
                T, RotationState.Two, RotationState.R,
                new Point[] { new Point(0, 0), new Point(-1, 0), new Point(-1, 1), new Point(0, -2), new Point(-1, -2) }
            };
        }
    }
}
