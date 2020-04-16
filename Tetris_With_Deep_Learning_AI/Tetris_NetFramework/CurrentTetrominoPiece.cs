using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Tetris
{
    public class CurrentTetrominoPiece
    {
        #region Tetris Block Position by offset and rotation data
        static readonly Dictionary<Tetromino, Dictionary<RotationState, Point[]>> minoData = new Dictionary<Tetromino, Dictionary<RotationState, Point[]>>()
        {
            { 
                Tetromino.I, new Dictionary<RotationState, Point[]>() 
                {
                    { RotationState.Zero, new Point[] { new Point(-1, 0), new Point(1, 0), new Point(2, 0) } },
                    { RotationState.R, new Point[] { new Point(0, 1), new Point(0, -1), new Point(0, -2) } },
                    { RotationState.Two, new Point[] { new Point(-2, 0), new Point(-1, 0), new Point(1, 0) } },
                    { RotationState.L, new Point[] { new Point(0, 2), new Point(0, 1), new Point(0, -1) } }
                }},
            {
                Tetromino.J, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(-1, 0), new Point(-1, 1), new Point(1, 0) } },
                    { RotationState.R, new Point[] { new Point(0, 1), new Point(1, 1), new Point(0, -1) } },
                    { RotationState.Two, new Point[] { new Point(1, 0), new Point(-1, 0), new Point(1, -1) } },
                    { RotationState.L, new Point[] { new Point(-1, -1), new Point(0, 1), new Point(0, -1) } }
                }},
            {
                Tetromino.L, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(-1, 0), new Point(1, 0), new Point(1, 1) } },
                    { RotationState.R, new Point[] { new Point(0, 1), new Point(0, -1), new Point(1, -1) } },
                    { RotationState.Two, new Point[] { new Point(-1, -1), new Point(-1, 0), new Point(1, 0) } },
                    { RotationState.L, new Point[] { new Point(-1, 1), new Point(0, 1), new Point(0, -1) } }
                }},
            {
                Tetromino.O, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, 1) } },
                    { RotationState.R, new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, 1) } },
                    { RotationState.Two, new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, 1) } },
                    { RotationState.L, new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, 1) } }
                }},
            {
                Tetromino.S, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(-1, 0), new Point(0, 1), new Point(1, 1) } },
                    { RotationState.R, new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, -1) } },
                    { RotationState.Two, new Point[] { new Point(1, 0), new Point(0, -1), new Point(-1, -1) } },
                    { RotationState.L, new Point[] { new Point(-1, 0), new Point(-1, 1), new Point(0, -1) } }
                }},
            {
                Tetromino.T, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1) } },
                    { RotationState.R, new Point[] { new Point(1, 0), new Point(0, 1), new Point(0, -1) } },
                    { RotationState.Two, new Point[] { new Point(1, 0), new Point(0, -1), new Point(-1, 0) } },
                    { RotationState.L, new Point[] { new Point(-1, 0), new Point(0, 1), new Point(0, -1) } }
                }},
            {
                Tetromino.Z, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(-1, 1), new Point(0, 1), new Point(1, 0) } },
                    { RotationState.R, new Point[] { new Point(1, 0), new Point(0, -1), new Point(1, 1) } },
                    { RotationState.Two, new Point[] { new Point(-1, 0), new Point(0, -1), new Point(1, -1) } },
                    { RotationState.L, new Point[] { new Point(-1, 0), new Point(0, 1), new Point(-1, -1) } }
                }
            }
        };
        #endregion

        public Tetromino minoType { get; private set; }
        TetrisGrid tetrisGrid;
        Point offset;
        public RotationState rotState { get; private set; } = RotationState.Zero;
        public CurrentTetrominoPiece(TetrisGrid curGrid, Tetromino minoType, Point offset)
        {
            this.tetrisGrid = curGrid;
            this.minoType = minoType;
            this.offset = offset;
        }

        public CurrentTetrominoPiece(TetrisGrid curGrid, Tetromino minoType, Point offset, RotationState rotState) : this(curGrid, minoType, offset)
        {
            this.rotState = rotState;
        }

        public IEnumerable<Point> GetPosOfBlocks()
        {
            return minoType.GetPos(offset, rotState);
        }

        public bool TrySpin(InputType spin)
        {
            if(spin != InputType.CCW && spin != InputType.CW)
                throw new InvalidOperationException("spin value must be CCW or CW");

            var before = rotState;
            RotationState after;

            if (spin == InputType.CCW) after = before.CCW();
            else if (spin == InputType.CW) after = before.CW();
            else return false;

            var expectedLocalOffsetPos = SRS.Translation(minoType, before, after);

            foreach(var localOffsetPos in expectedLocalOffsetPos)
            {
                var expectedPos = minoType.GetPos(localOffsetPos.Add(this.offset), after);
                bool canBlockExistAtOffset = tetrisGrid.CanMinoExistHere(expectedPos);

                if(canBlockExistAtOffset)
                {
                    this.offset = this.offset.Add(localOffsetPos);
                    this.rotState = after;
                    return true;
                }
            }
            return false;
        }

        public bool TryShift(Point localShiftPoint)
        {
            var newOffsetPos = offset.Add(localShiftPoint);
            var expectedBlockPos = minoType.GetPos(newOffsetPos, rotState);

            if(tetrisGrid.CanMinoExistHere(expectedBlockPos))
            {
                offset = newOffsetPos;
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<Point> GetExpectedHardDropPosOfBlocks()
        {
            var expectedBlockPos = minoType.GetPos(offset, rotState);
            if(tetrisGrid.CanMinoExistHere(expectedBlockPos) == false) // Dead State인데, 받아오려고 하면 에러가 남
                throw new Exception($"Unexpected exception on {nameof(this.GetExpectedHardDropPosOfBlocks)}");

            bool canPlaceMino = true;
            
            for(int i = -1; canPlaceMino; i--)
            {
                Point newOffsetPos = this.offset.Add(new Point(0, i));
                var newExpectedBlockPos = minoType.GetPos(newOffsetPos, rotState);
                canPlaceMino = tetrisGrid.CanMinoExistHere(newExpectedBlockPos);

                if(canPlaceMino)
                    expectedBlockPos = newExpectedBlockPos;
            }

            return expectedBlockPos;
        }
    }
}