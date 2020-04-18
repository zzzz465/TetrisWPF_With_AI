using System;
using System.Collections.Generic;
using System.Linq;
using Tetris;
using ColdClear;
using System.Drawing;
using Tetris;
using log4net;

namespace ColdClear
{
    // wrapper class for cold clear API
    public class ColdClear : IDisposable, AI
    {
        #region static methods and variables
        readonly static List<Point> blankPointOnHold = new List<Point>();
        public static ColdClear CreateInstance(CCOptions options, CCWeights weights)
        {
            return new ColdClear(options, weights);
        }

        public static ColdClear CreateInstance()
        {
            CCOptions options;
            ColdClearAPI.cc_default_options(out options);
            CCWeights weights;
            ColdClearAPI.cc_default_weights(out weights);
            return new ColdClear(options, weights);
        }
        #endregion
        ILog Log = LogManager.GetLogger("ColdClear");
        IntPtr CCBot;
        bool isInstructionRequested = false;
        private bool disposedValue;

        protected ColdClear(CCOptions options, CCWeights weights)
        {
            CCBot = ColdClearAPI.cc_launch_async(ref options, ref weights);
            if(ColdClearAPI.cc_is_dead_async(CCBot))
                throw new Exception("CCBot died immediately!");
        }

        public void AddMino(Tetromino mino)
        {
            Log.Debug($"Add Mino {mino} to CC");
            ColdClearAPI.cc_add_next_piece_async(CCBot, mino.ToCCPiece());
        }

        public bool TryGetInstructionSet(Int32 incoming, out InstructionSet InstructionSet)
        {
            Log.Debug("Try to get instruction");
            if(!isInstructionRequested)
            {
                Log.Debug("Instruction is not requested, requesting new one...");
                ColdClearAPI.cc_request_next_move(CCBot, incoming);
                isInstructionRequested = true;
            }

            if(ColdClearAPI.cc_poll_next_move(CCBot, out var CCMove))
            {
                Log.Debug($"Successfully polled next move, movement count : {CCMove.movement_count}");
                isInstructionRequested = false;

                var convertedMovements = new List<Instruction>();

                if (CCMove.hold)
                    convertedMovements.Insert(0, Instruction.Hold);

                for(int i = 0; i < CCMove.movement_count; i++)
                {
                    var convertedInst = CCMove.movements[i].ToMovement();
                    convertedMovements.Add(convertedInst);
                }

                convertedMovements.Add(Instruction.SoftDrop_ToBottom);
                convertedMovements.Add(Instruction.LockMino);
                convertedMovements.Add(Instruction.InstructionDone);

                Point[] expected_points = new Point[4];
                for(int i = 0; i < 4; i++)
                    expected_points[i] = new Point(CCMove.expected_x[i], CCMove.expected_y[i]);

                InstructionSet = new InstructionSet(convertedMovements, expected_points);
                return true;
            }
            else
            {
                Log.Debug("Cannot poll next move");
                InstructionSet = null;
                return false;
            }
        }

        public void Reset() // Reset to initial state
        {
            ColdClearAPI.cc_reset_async(CCBot, new bool[10*20], false, 0);
        }

        public void Reset(bool[] grid, bool b2b, int combo)
        {
            ColdClearAPI.cc_reset_async(CCBot, grid, b2b, combo);
        }


        #region Dispose of unmanaged objects

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ColdClearAPI.cc_destroy_async(CCBot);
                    CCBot = IntPtr.Zero;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        ~ColdClear()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}