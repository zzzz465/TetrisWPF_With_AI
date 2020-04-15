using System;
using System.Collections.Generic;
using System.Linq;
using Tetris;
using Xunit;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using Xunit.Abstractions;

namespace XUnitTest_Tetris
{
    public class Test_CurrentTetromino
    {
        private readonly ITestOutputHelper output;

        public Test_CurrentTetromino(ITestOutputHelper helper)
        {
            this.output = helper;
        }

        [Theory]
        [MemberData(nameof(MinoOffset))]
        public void GetPos_Should_Return_valid_offset_per_tetromino(Tetromino mino, Point offset, RotationState rotState, Point[] points)
        {
            
            var minoPiece = new CurrentTetrominoPiece(new TetrisGrid(), mino, offset, rotState);
            var expectedPos = minoPiece.GetPosOfBlocks();
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
        
        TetrisGrid CreateTestGrid(int maxAllowedHeight = 20)
        {
                        /*
              
              0 1 2 3 4 5 6 7 8 9
            9
            8 
            7
            6
            5
            4 D D D D
            3       C C A       B
            2       C A A A B B B
            1 F F F C F F T T F F
            0 F F F F T T T T T T
            */
            Point[] points = new Point[] 
            { 
                new Point(4, 0), new Point(5, 0), new Point(6, 0), new Point(7, 0), new Point(8, 0), new Point(9, 0),
                new Point(6, 1), new Point(7, 1)
            };
            

            var grid = new TetrisGrid(maxAllowedHeight);
            grid.Set(points, true);

            Point[] A = new Point[] { new Point(4, 2), new Point(5, 2), new Point(6, 2), new Point(5, 3) };
            grid.Set(A, true);

            Point[] B = new Point[] { new Point(7, 2), new Point(8, 2), new Point(9, 2), new Point(9, 3) };
            grid.Set(B, true);

            Point[] C = new Point[] { new Point(3, 1), new Point(3, 2), new Point(3, 3), new Point(4, 3) };
            grid.Set(C, true);

            Point[] D = new Point[] { new Point(0, 4), new Point(1, 4), new Point(2, 4), new Point(3, 4) };
            grid.Set(D, true);

            return grid;
        }

        [Fact]
        public void Check_J_Rotation()
        {
            var tetrisGrid = CreateTestGrid();

            var offset1 = new Point(6, 6);
            var piece1 = new CurrentTetrominoPiece(tetrisGrid, Tetromino.J, offset1); // 5,6, 6,6 7,6 7,7
            Assert.True(piece1.TrySpin(InputType.CW));

            var actualPos1 = piece1.GetPosOfBlocks();
            Point[] expectedPos1 = new Point[] { new Point(6, 6), new Point(6, 7), new Point(6, 5), new Point(7, 5) };
            Assert.Equal(actualPos1, expectedPos1);

            Assert.True(piece1.TrySpin(InputType.CW));
            var actualPos2 = piece1.GetPosOfBlocks();
            Point[] expectedPos2 = new Point[] { new Point(6, 6), new Point(5, 6), new Point(5, 5), new Point(7, 6) };
            Assert.Equal(expectedPos2, actualPos2);

            Assert.True(piece1.TrySpin(InputType.CCW));
            Assert.True(piece1.TrySpin(InputType.CCW));
            Assert.True(piece1.TrySpin(InputType.CCW));
            var actualPos3 = piece1.GetPosOfBlocks();
            Point[] expectedPos3 = new Point[] { new Point(6, 6), new Point(6, 7), new Point(5, 7), new Point(6, 5) };
            Assert.Equal(expectedPos3, actualPos3);
        }

        TetrisGrid CreateComplexGrid()
        {
            Point[] Points = new Point[] 
            {
                new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(3, 0), new Point(4, 0),
                new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(3, 1),
                new Point(0, 2), new Point(1, 2),
                new Point(1, 3), new Point(2, 3), new Point(3, 3),
                new Point(6, 0), new Point(7, 0), new Point(8, 0), new Point(9, 0),
                new Point(6, 1), new Point(7, 1), new Point(8, 1), new Point(9, 1),
                new Point(6, 2), new Point(7, 2), new Point(8, 2), new Point(9, 2),
            /*new Point(6, 3),*/ new Point(7, 3), new Point(8, 3), new Point(9, 3),
                new Point(6, 4), new Point(7, 4), new Point(8, 4), new Point(9, 4),
                new Point(5, 5), new Point(6, 5), new Point(7, 5),
                new Point(4, 6), new Point(5, 6)
            };

            TetrisGrid testGrid = new TetrisGrid();
            testGrid.Set(Points, true);

            return testGrid;
        }

        [Fact]
        public void Check_Complex_J_Rotation()
        {
            var testGrid = CreateComplexGrid();
            var currentPiece = new CurrentTetrominoPiece(testGrid, Tetromino.J, new Point(4, 4), RotationState.Zero);
            Assert.True(currentPiece.TrySpin(InputType.CCW), "it should spin in this specific case but somehow it failed to spin...");
            var expectedPos = new Point[] { new Point(4, 1), new Point(5, 1), new Point(5, 2), new Point(5, 3) };
            var actualPos = currentPiece.GetPosOfBlocks();
            CollectionAssert.CollectionSameWithoutOrder<Point>(expectedPos, actualPos);
        }
        
        [Fact]
        public void Print_Complex_Grid_To_Output()
        {
            var testGrid = CreateComplexGrid();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("==== Grid Visual ===");
            foreach(var line in testGrid.getLines.Reverse())
            {
                foreach(var x in line.line)
                {
                    builder.Append(x ? "1" : "0");
                }
                builder.AppendLine();
            }
            builder.Append("=== End of Grid Visual ===");
            output.WriteLine(builder.ToString());
        }

        /*
        [Theory]
        [MemberData(nameof(GetPos_Test_Case), MemberType = typeof(Test_CurrentTetromino))]
        public void GetPos_Method_Should_return_valid_values(Tetromino minoType, Point offset, IEnumerable<Point> expectedPos)
        {
            // minoType.GetPos();
        }

        public static IEnumerable<object[]> GetPos_Test_Case()
        {

        }
        */
    }
}