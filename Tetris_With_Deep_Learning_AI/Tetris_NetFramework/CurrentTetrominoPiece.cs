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
                    { RotationState.L, new Point[] { new Point(-1, 0), new Point(-1, 1), new Point(-1, 0) } }
                }},
            {
                Tetromino.T, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(1, 0), new Point(-1, 0), new Point(1, 0) } },
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
        public Point offset { get; set; }
        public RotationState rotState { get; private set; } = RotationState.Zero;
        public CurrentTetrominoPiece(Tetromino minoType, Point offset)
        {
            this.minoType = minoType;
            this.offset = offset;
        }

        public CurrentTetrominoPiece(Tetromino minoType, Point offset, RotationState rotState) : this(minoType, offset)
        {
            this.rotState = rotState;
        }

        public IEnumerable<Point> GetPos()
        {
            return minoType.GetPos(offset, rotState);
        }
    }
}