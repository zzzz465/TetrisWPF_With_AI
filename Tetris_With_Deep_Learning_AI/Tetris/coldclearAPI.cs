using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Tetris;
using ColdClear;
/*
write by madeline, 2020/03/15
CCL-2.0
*/
namespace Tetris
{
    public static partial class Converter
    {
        public static bool[] ToBoolArr(this TetrisGrid tetrisGrid)
        {
            var lines = tetrisGrid.getLines.ToList();
            int width = tetrisGrid.width;
            int height = tetrisGrid.maxHeight;
            var arr = new bool[width * height];
            for(int y = 0; y < height; y++)
            {
                var curLine = lines[y];
                for(int x = 0; x < width; x++)
                {
                    var coord = width * y + x;
                    arr[coord] = curLine.line[x];
                }
            }

            return arr;
        }
        public static Tetromino ToTetromino(this CCPiece piece)
        {
            switch(piece)
            {
                case CCPiece.CC_I:
                    return Tetromino.I;
                case CCPiece.CC_J:
                    return Tetromino.J;
                case CCPiece.CC_L:
                    return Tetromino.L;
                case CCPiece.CC_O:
                    return Tetromino.O;
                case CCPiece.CC_S:
                    return Tetromino.S;
                case CCPiece.CC_T:
                    return Tetromino.T;
                case CCPiece.CC_Z:
                    return Tetromino.Z;
                default:
                    throw new Exception();
            }
        }

        public static CCPiece ToCCPiece(this Tetromino piece)
        {
            switch(piece)
            {
                case Tetromino.I:
                    return CCPiece.CC_I;
                case Tetromino.J:
                    return CCPiece.CC_J;
                case Tetromino.L:
                    return CCPiece.CC_L;
                case Tetromino.O:
                    return CCPiece.CC_O;
                case Tetromino.S:
                    return CCPiece.CC_S;
                case Tetromino.T:
                    return CCPiece.CC_T;
                case Tetromino.Z:
                    return CCPiece.CC_Z;
                default:
                    throw new Exception();
            }
        }

        public static Movement ToMovement(this CCMovement move)
        {
            switch(move)
            {
                case CCMovement.CC_CW:
                    return Movement.CW;
                case CCMovement.CC_CCW:
                    return Movement.CCW;
                case CCMovement.CC_Drop:
                    return Movement.HardDrop;
                case CCMovement.CC_Left:
                    return Movement.Left;
                case CCMovement.CC_Right:
                    return Movement.Right;
                default:
                    throw new Exception();
            }
        }
    }
    
}
namespace ColdClear
{
    public enum CCPiece : uint
    {
        CC_I, CC_T, CC_O, CC_S, CC_Z, CC_L, CC_J
    }
    public enum CCMovement : uint
    {
        CC_Left, CC_Right,CC_CW,CC_CCW,CC_Drop
    }
    public enum CCMovementMode : uint
    {
        CC_0G,
        CC_20G,
        CC_HARD_DROP_ONLY
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CCMove
    {
        /* Whether hold is required */
        [MarshalAs(UnmanagedType.U1)]
        public bool hold;
        /* Expected cell coordinates of placement, (0, 0) being the bottom left */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] expected_x;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] expected_y;
        /* Number of moves in the path */
        public byte movement_count;
        /* Movements */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public CCMovement[] movements;
        /* Bot Info */
        public UInt32 nodes;
        public UInt32 depth;
        public UInt32 original_rank;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CCOptions
    {            
        public CCMovementMode mode;
        [MarshalAs(UnmanagedType.U1)]
        public bool use_hold;
        [MarshalAs(UnmanagedType.U1)]
        public bool speculate;
        public UInt32 min_nodes;
        public UInt32 max_nodes;
        public UInt32 threads;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CCWeights
    {
        public Int32 back_to_back;
        public Int32 bumpiness;
        public Int32 bumpiness_sq;
        public Int32 height;
        public Int32 top_half;
        public Int32 top_quarter;
        public Int32 jeopardy;
        public Int32 cavity_cells;
        public Int32 cavity_cells_sq;
        public Int32 overhang_cells;
        public Int32 overhang_cells_sq;
        public Int32 covered_cells;
        public Int32 covered_cells_sq;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Int32[] tslot;
        public Int32 well_depth;
        public Int32 max_well_depth;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public Int32[] well_column;
        public Int32 b2b_clear;
        public Int32 clear1;
        public Int32 clear2;
        public Int32 clear3;
        public Int32 clear4;
        public Int32 tspin1;
        public Int32 tspin2;
        public Int32 tspin3;
        public Int32 mini_tspin1;
        public Int32 mini_tspin2;
        public Int32 perfect_clear;
        public Int32 combo_garbage;
        public Int32 move_time;
        public Int32 wasted_t;
        [MarshalAs(UnmanagedType.U1)]
        public bool use_bag;
    }
    public static class ColdClearAPI
    {
        
        [DllImport("cold_clear")]
        public static extern IntPtr cc_launch_async(ref CCOptions options, ref CCWeights weights);
        
        [DllImport("cold_clear")]
        public static extern IntPtr cc_launch_async(IntPtr optionsPtr, IntPtr weightsPtr);
        [DllImport("cold_clear")]
        public static extern void cc_destroy_async(IntPtr bot);
        [DllImport("cold_clear")]
        public static extern void cc_reset_async(IntPtr bot, bool[] field, bool b2b, Int32 combo);
        [DllImport("cold_clear")]
        public static extern void cc_add_next_piece_async(IntPtr bot, CCPiece piece);
        [DllImport("cold_clear")]
        public static extern void cc_request_next_move(IntPtr bot, Int32 incoming);
        [DllImport("cold_clear")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool cc_poll_next_move(IntPtr bot, out CCMove move);
        [DllImport("cold_clear")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool cc_is_dead_async(IntPtr bot);
        [DllImport("cold_clear")]
        public static extern void cc_default_options(out CCOptions options);
        [DllImport("cold_clear")]
        public static extern void cc_default_weights(out CCWeights weights);
        [DllImport("cold_clear")]
        public static extern void cc_fast_weights(out CCWeights weights);
    }
}