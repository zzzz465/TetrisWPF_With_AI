using System;
using System.Collections.Generic;
using System.Linq;
using Tetris;
using ColdClear;

namespace ColdClear
{
    // wrapper class for cold clear API
    public class ColdClear : IDisposable
    {
        #region static methods and variables
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
        IntPtr CCBot;
        Queue<CCMove> moves;
        IEnumerator<Instruction> currentInstructions;
        public int RequestButNotPolledMovesCount { get; private set; } = 0;
        private bool disposedValue;

        protected ColdClear(CCOptions options, CCWeights weights)
        {
            CCBot = ColdClearAPI.cc_launch_async(ref options, ref weights);
            if(ColdClearAPI.cc_is_dead_async(CCBot))
                throw new Exception("CCBot died immediately!");

            moves = new Queue<CCMove>();
        }

        public void AddMino(Tetromino mino)
        {
            ColdClearAPI.cc_add_next_piece_async(CCBot, mino.ToCCPiece());
        }

        public void RequestNextMoveSets(Int32 incoming)
        {
            RequestButNotPolledMovesCount += 1;
            ColdClearAPI.cc_request_next_move(CCBot, incoming);
        }

        public bool PollNextMoveToMoveQueue()
        {
            var success = ColdClearAPI.cc_poll_next_move(CCBot, out var move);

            if(success)
            {
                moves.Enqueue(move);
                RequestButNotPolledMovesCount -= 1;
            }

            return success;
        }

        public bool TryGetInstruction(Int32 incoming, out Instruction instruction)
        {
            instruction = Instruction.None;

            if(currentInstructions == null)
            {
                if(moves.Count == 0)
                {
                    if(RequestButNotPolledMovesCount <= 3)
                        RequestNextMoveSets(incoming);
                        
                    if(!PollNextMoveToMoveQueue())
                        return false;
                }
                
                this.currentInstructions = InstructionEnumerator(moves.Dequeue());
            }

            if(currentInstructions.MoveNext())
            {
                instruction = currentInstructions.Current;
                return true;
            }
            else
            {
                currentInstructions = null;
                return false;
            }
        }

        IEnumerator<Instruction> InstructionEnumerator(CCMove ccMove)
        {
            if(ccMove.hold)
            {
                yield return Instruction.Hold;
                yield break;
            }

            foreach(var move in ccMove.movements)
            {
                yield return move.ToMovement();
            }

            yield return Instruction.InstructionDone;
        }

        // public bool 

        public void Reset() // Reset to initial state
        {
            moves.Clear();
            ColdClearAPI.cc_reset_async(CCBot, new bool[10*20], false, 0);
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