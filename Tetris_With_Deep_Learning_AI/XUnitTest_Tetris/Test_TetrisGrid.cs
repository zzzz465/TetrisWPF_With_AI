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
            grid.Set(points, Tetromino.Garbage);

            Point[] A = new Point[] { new Point(4, 2), new Point(5, 2), new Point(6, 2), new Point(5, 3) };
            grid.Set(A, Tetromino.T);

            Point[] B = new Point[] { new Point(7, 2), new Point(8, 2), new Point(9, 2), new Point(9, 3) };
            grid.Set(B, Tetromino.L);

            Point[] C = new Point[] { new Point(3, 1), new Point(3, 2), new Point(3, 3), new Point(4, 3) };
            grid.Set(C, Tetromino.J);

            Point[] D = new Point[] { new Point(0, 4), new Point(1, 4), new Point(2, 4), new Point(3, 4) };
            grid.Set(D, Tetromino.I);

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
            grid.Set(points, Tetromino.Garbage);

            Point[] wrong = new Point[] { new Point(4, 0), new Point(4, 1), new Point(5,0), new Point(5, 1) };
            Assert.False(grid.TryPlace(wrong, Tetromino.None), "failure at test case wrong");

            Point[] A = new Point[] { new Point(4, 2), new Point(5, 2), new Point(6, 2), new Point(5, 3) };
            Assert.True(grid.TryPlace(A, Tetromino.T), "failure at test case A"); // can place on blank place
            Assert.False(grid.TryPlace(A, Tetromino.T), "failure at test case A"); // cannot place if the block already exists

            Point[] B = new Point[] { new Point(7, 2), new Point(8, 2), new Point(9, 2), new Point(9, 3) };
            Assert.True(grid.TryPlace(B, Tetromino.L), "failure at test case B - true");
            Assert.False(grid.TryPlace(B, Tetromino.L), "failure at test case B - false");

            Point[] C = new Point[] { new Point(3, 1), new Point(3, 2), new Point(3, 3), new Point(4, 3) };
            Assert.True(grid.TryPlace(C, Tetromino.J), "Failure at tast case C - true");
            Assert.False(grid.TryPlace(C, Tetromino.J), "Failure at tast case C - false");

            Point[] D = new Point[] { new Point(0, 4), new Point(1, 4), new Point(2, 4), new Point(3, 4) };
            Assert.True(grid.TryPlace(D, Tetromino.I), "Failure at D");
            Assert.False(grid.TryPlace(D, Tetromino.I), "Failure at D");

            Point[] FloatingBlock = new Point[] { new Point(4, 6), new Point(4, 7), new Point(5, 6), new Point(5, 7) };
            Assert.False(grid.TryPlace(FloatingBlock, Tetromino.Garbage), "FloatingBlockCheck failed");
        }

        [Fact]
        public void Grid_Update_Should_Work_As_Expected()
        {
            var testGrid = CreateTestGrid();

            Point[] FillTwoLine = new Point[] 
            { // 0번째 줄, 1번째 줄을 채움
                new Point(0, 0), new Point(0, 1), new Point(1, 0), 
                new Point(1, 1), new Point(2, 0), new Point(2, 1), 
                new Point(3, 0), new Point(4, 1), new Point(5, 1), 
                new Point(8, 1), new Point(9, 1)
            };

            testGrid.Set(FillTwoLine, Tetromino.Garbage);
            testGrid.UpdateBoard();

            var lines = testGrid.getLines.ToList();

            var T = Tetromino.T;
            var L = Tetromino.L;
            var J = Tetromino.J;
            var I = Tetromino.I;
            var None = Tetromino.None;
            
            Assert.Equal(lines[0].line, new Tetromino[] { None, None, None, J, T, T, T, L, L, L });
            Assert.Equal(lines[1].line, new Tetromino[] { None, None, None, J, J, T, None, None, None, L });
            Assert.Equal(lines[2].line, new Tetromino[] { I, I, I, I, None, None, None, None, None, None });
            
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

        [Fact]
        public void Add_Trash_Line_Should_Push_Default_Grid_To_Up()
        {
            /*
              
              0 1 2 3 4 5 6 7 8 9
            9
            8
            7 D D D D
            6       C C A       B
            5       C A A A B B B
            4 F F F C F F T T F F
            3 F F F F T T T T T T
            2 T T T T F T T T T T
            1 T T T T F T T T T T
            0 T T T T F T T T T T
            */
            var testGrid = CreateTestGrid();
            testGrid.AddTrashLine(3, 4);

            Point[] shouldExist = new Point[] 
            {
                new Point(0, 0), new Point(2, 2), new Point(2, 1), new Point(2, 0),
                new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(3, 1),
                new Point(5, 1), new Point(6, 1), new Point(7, 1), new Point(8, 1),
                new Point(9, 1)
            };

            Point[] shouldNotExist = new Point[]
            {
                new Point(4, 0),
                new Point(4, 1),
                new Point(4, 2),
            };

            foreach(var candidate in shouldExist)
                Assert.True(testGrid.Get(candidate));

            foreach(var candidate in shouldNotExist)
                Assert.False(testGrid.Get(candidate));
        }
    }
}