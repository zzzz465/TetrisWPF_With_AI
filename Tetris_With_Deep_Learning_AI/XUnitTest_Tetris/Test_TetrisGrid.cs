using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Tetris;
using Xunit;

namespace XUnitTest_Tetris
{
    public class Test_TetrisGrid
    {
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
        public void Grid_Should_Prevent_TryPlace_To_Existing_Block_And_Prevent_floating_Block()
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
            

            var grid = new TetrisGrid();
            grid.Set(points, true);

            Point[] wrong = new Point[] { new Point(4, 0), new Point(4, 1), new Point(5,0), new Point(5, 1) };
            Assert.False(grid.TryPlace(wrong, Tetromino.None));

            Point[] A = new Point[] { new Point(4, 2), new Point(5, 2), new Point(6, 2), new Point(5, 3) };
            Assert.True(grid.TryPlace(A, Tetromino.T), "failure at test case A"); // can place on blank place
            Assert.False(grid.TryPlace(A, Tetromino.T), "failure at test case A"); // cannot place if the block already exists

            Point[] B = new Point[] { new Point(7, 2), new Point(8, 2), new Point(9, 2), new Point(9, 3) };
            Assert.True(grid.TryPlace(B, Tetromino.None), "failure at test case B");
            Assert.False(grid.TryPlace(B, Tetromino.None), "failure at test case B");

            Point[] C = new Point[] { new Point(3, 1), new Point(3, 2), new Point(3, 3), new Point(4, 3) };
            Assert.True(grid.TryPlace(C, Tetromino.None), "Failure at tast case C - true");
            Assert.False(grid.TryPlace(C, Tetromino.None), "Failure at tast case C - false");

            Point[] D = new Point[] { new Point(0, 4), new Point(1, 4), new Point(2, 4), new Point(3, 4) };
            Assert.True(grid.TryPlace(D, Tetromino.None), "Failure at D");
            Assert.False(grid.TryPlace(D, Tetromino.None), "Failure at D");

            Point[] FloatingBlock = new Point[] { new Point(4, 6), new Point(4, 7), new Point(5, 6), new Point(5, 7) };
            Assert.False(grid.TryPlace(FloatingBlock, Tetromino.None), "FloatingBlockCheck failed");
        }

        [Fact]
        public void Grid_Update_Should_Work_As_Expected()
        {
            var testGrid = CreateTestGrid();

            Point[] FillTwoLine = new Point[] 
            { 
                new Point(0, 0), new Point(0, 1), new Point(1, 0), 
                new Point(1, 1), new Point(2, 0), new Point(2, 1), 
                new Point(3, 0), new Point(4, 1), new Point(5, 1), 
                new Point(8, 1), new Point(9, 1)
            };

            testGrid.Set(FillTwoLine, true, Tetromino.None);
            testGrid.UpdateBoard();

            var lines = testGrid.getLines.ToList();
            Assert.Equal(lines[0].line, new bool[] { false, false, false, true, true, true, true, true, true, true });
            Assert.Equal(lines[1].line, new bool[] { false, false, false, true, true, true, false, false, false, true });
            Assert.Equal(lines[2].line, new bool[] { true, true, true, true, false, false, false, false, false, false });
        }

        [Fact]
        public void Grid_Should_Disallow_Block_Out_Of_Grid()
        {
            var testGrid = CreateTestGrid(20);
            Point[] outOfBorderGrid = new Point[]
            {
                new Point(0, -1), new Point(-1, 0), new Point(-1, -1), new Point(10, 0),
                new Point(11, 1), new Point(5, -4), new Point(8, -1), new Point(9, 24)
            };

            foreach(var point in outOfBorderGrid)
            {
                Assert.False(testGrid.CanMinoExistHere(point));
            }
        }

        [Fact]
        public void Grid_Should_Disallow_BlockPlacement_To_Existing_Point()
        {
            var testGrid = CreateTestGrid();
            Point[] alreadyExistingPoints = new Point[] { new Point(2, 4) };
            Assert.False(testGrid.TryPlace(alreadyExistingPoints, Tetromino.None));
        }
    }
}