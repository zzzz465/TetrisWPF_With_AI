using System;
using System.Collections.Generic;
using System.Linq;
using Tetris;
using ColdClear;
using System.Drawing;
using log4net;

namespace ColdClear
{
    // wrapper class for cold clear API
    public class ColdClearAI : IDisposable, AI
    {
        #region static methods and variables
        readonly static List<Point> blankPointOnHold = new List<Point>();
        public static ColdClearAI CreateInstance(CCOptions options, CCWeights weights, TetrominoBag currentUsingBag)
        {
            if(currentUsingBag == null)
                throw new ArgumentNullException();

            var cc = new ColdClearAI(options, weights);
            cc.currentUsingBag = currentUsingBag;

            if(ColdClearAPI.cc_is_dead_async(cc.CCBot))
                throw new Exception("CCBot died immediately");

            return cc;
        }

        public static ColdClearAI CreateInstance()
        {
            CCOptions options;
            ColdClearAPI.cc_default_options(out options);
            CCWeights weights;
            ColdClearAPI.cc_default_weights(out weights);
            return new ColdClearAI(options, weights);
        }
        #endregion
        ILog Log = LogManager.GetLogger("ColdClear");
        IntPtr CCBot = IntPtr.Zero;
        bool isInstructionRequested = false;
        private bool disposedValue;

        #region options
        CCOptions cc_options;
        CCWeights cc_weights;
        int UsedMinoCountForSpeculation = 5;
        #endregion

        TetrominoBag currentUsingBag;
        bool isMinoExistInHold = false;

        protected ColdClearAI(CCOptions options, CCWeights weights)
        {
            this.cc_options = options;
            this.cc_weights = weights;
            this.CCBot = ColdClearAPI.cc_launch_async(ref this.cc_options, ref this.cc_weights);
        }

        public void OnGameInitialize()
        {
            Log.DebugAI($"Initializing ColdClearAI, using mino {UsedMinoCountForSpeculation}");
            for(int i = 0; i < UsedMinoCountForSpeculation; i++)
            {
                var mino = currentUsingBag.Peek(i);
                AddMino(currentUsingBag.Peek(i));
            }
        }

        public void RequestNextInstructionSet(int incoming)
        {
            ColdClearAPI.cc_request_next_move(CCBot, incoming);
            Log.DebugAI($"Instruction requested with incoming : {incoming}");

            if(ColdClearAPI.cc_is_dead_async(CCBot))
                Log.Fatal("Bot died");
            /*
            5개를 집어넣었으면, 0~4까지 들어가있고, request를 하면 0번의 무브먼트를 요청 -> 하나를 추가로 넣어줌
            이것 이후에는 분명히 미노가 사용되는걸 보장받아야함
            */
        }

        public bool TryGetInstructionSet(out InstructionSet InstructionSet)
        {
            if(ColdClearAPI.cc_poll_next_move(CCBot, out var CCMove))
            {
                Log.DebugAI($"Successfully polled next move, movement count : {CCMove.movement_count}");
                isInstructionRequested = false;

                var convertedMovements = new List<Instruction>();

                if (CCMove.hold)
                {
                    convertedMovements.Insert(0, Instruction.Hold);
                    /*
                    if(isMinoExistInHold == false)
                    {
                        NotifyBagConsumed();
                        isMinoExistInHold = true;
                    }
                    */
                }

                for(int i = 0; i < CCMove.movement_count; i++)
                {
                    var convertedInst = CCMove.movements[i].ToMovement();
                    convertedMovements.Add(convertedInst);
                }

                convertedMovements.Add(Instruction.HardDrop);
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
                Log.DebugAI("Cannot poll next move");
                InstructionSet = null;
                return false;
            }
        }
        public void NotifyBagConsumed()
        {
            /*
            T Z J L I S O 인 상태에서 UsedMinoCountForSpeculation(길이) 가 5라면, T Z J L I까지 들어가있고, S를 넣어야함.
            T를 소모한 다음 이 함수가 호출되므로, 호출된 당시에 Bag은 Z J L I S O 이므로, S가 들어갈려면 인덱스 기준 Z J L I S, x - 1번째를 추가해야함
            */
            AddMino(currentUsingBag.Peek(UsedMinoCountForSpeculation - 1));
        }

        public void AddMino(Tetromino mino)
        {
            Log.DebugAI($"Add Mino {mino} to CC");
            ColdClearAPI.cc_add_next_piece_async(CCBot, mino.ToCCPiece());
        }

        public void NotifyGridChanged(IEnumerable<TetrisLine> grid, int combo, bool b2b)
        {
            Log.Debug("Grid changed due to garbage line or misdrop... reset grid");

            var lines = grid.ToList();
            bool[] convertedGrid = new bool[400];
            for(int y = 0; y < 20; y++)
            {
                var curLine = y < lines.Count ? lines[y].line : new Tetromino[10];
                for(int x = 0; x < 10; x++)
                {
                    convertedGrid[x + y * 10] = curLine[x] != Tetromino.None;
                }
            }

            ColdClearAPI.cc_reset_async(CCBot, convertedGrid, b2b, combo);
        }
        
        public void Reset(bool[] grid, bool b2b, int combo) // Reset Game Board, including grid
        {
            ColdClearAPI.cc_reset_async(CCBot, grid, b2b, combo);
        }

        public void CleanReset(TetrominoBag bag)
        { // reset, meaning kill bot and re-initializing bot so the hold data can be removed.
            if(CCBot != IntPtr.Zero && !ColdClearAPI.cc_is_dead_async(CCBot))
                ColdClearAPI.cc_destroy_async(CCBot);

            this.CCBot = ColdClearAPI.cc_launch_async(ref this.cc_options, ref this.cc_weights);
            this.currentUsingBag = bag;
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

        ~ColdClearAI()
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