using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using log4net;

namespace Tetris
{
    public class AITetrisGame : TetrisGame // CC봇으로 먼저 만들어보자.
    {
        protected override ILog Log { get; set; }
        AI ai;
        InstructionSet instructionSet;
        TimeSpan LastUpdateTime = TimeSpan.Zero;
        TimeSpan UpdateDelay = TimeSpan.FromMilliseconds(16);
        public IEnumerable<Point> expectedMinoEndPoints { get { return instructionSet?.expectedPoints; } }
        public AITetrisGame(AI ai, AIGameSetting AIGameSetting, TetrominoBag bag = null) : base(AIGameSetting, bag)
        {
            this.UpdateDelay = AIGameSetting.UpdateDelay;
            this.ai = ai;
            Log = LogManager.GetLogger("AITetrisGame");
            // throw new NotImplementedException();
        }

        public override void InitializeGame()
        {
            this.tetrominoBag.Reset();

            ai.CleanReset(this.tetrominoBag);
            ai.OnGameInitialize();

            this.gameState = GameState.OnInitialize;
        }
        public override void PauseGame()
        {
            throw new NotImplementedException();
        }

        public override void ResumeGame(TimeSpan curTime)
        {
            throw new NotImplementedException();
        }

        protected override void Update(TimeSpan curTime)
        {
            if(this.gameState != GameState.Playing)
                return;

            if(curTime - LastUpdateTime < UpdateDelay)
                return;
            else
                LastUpdateTime = curTime;

            //  Log.Debug("Start Updating AITetrisGame");

            /*
            1. 미노 생성 직전에 리퀘스트를 날려야함
            2. 미노 생성을 해야함
            3. InstructionSet을 받아와야함
            */

            if(currentPiece == null)
            {
                if(!TryCreateCurrentPiece(curTime)) // 무조건 성공하거나, 게임이 종료되어야함
                    return;
            }

            if (instructionSet == null || instructionSet.Current == Instruction.InstructionDone || instructionSet.Current == Instruction.SkipToNextMino || instructionSet.Current == Instruction.None)
            {
                if (ai.TryGetInstructionSet(out instructionSet))
                    instructionSet.MoveNext(); // set index to zero
                else
                    return;
            }

            var curMove = instructionSet.Current;
            // Log.Debug($"current move : {curMove} | Index : {instructions.index}");

            switch(curMove)
            {
                case Instruction.CCW:
                case Instruction.CW:
                {
                    if(curMove == Instruction.CCW)
                    {
                        // Log.Debug("Try CCW");
                        if(currentPiece.TrySpin(InputType.CCW))
                        {
                            Log.DebugAI("CCW success");
                            LastUpdateTime = curTime;
                            instructionSet.MoveNext();
                        }
                        else
                        {
                            // shouldn't fail but what if somehow it failed to spin?
                        }
                    }
                    else
                    {
                        // Log.Debug("Try CW");
                        if(currentPiece.TrySpin(InputType.CW))
                        {
                            Log.DebugAI("CW Success");
                            LastUpdateTime = curTime;
                            instructionSet.MoveNext();
                        }
                        else
                        {
                            // shouldn't fail but what if somehow it failed to spin?
                        }
                    }
                    break;
                }

                case Instruction.HardDrop:
                {
                    Log.DebugAI("Try HardDrop");
                    while(currentPiece.TryShift(new Point(0, -1)));
                    LastUpdateTime = curTime;
                    instructionSet.MoveNext();
                    break;
                }

                case Instruction.SoftDrop_ToBottom:
                {
                    if(curTime - LastSoftDropTime > SoftDropDelay)
                    {
                        if(currentPiece.TryShift(new Point(0, -1)))
                        {
                            LastSoftDropTime = curTime;
                            LastUpdateTime = curTime;
                            Log.DebugAI("SoftDrop to bottom");
                        }
                        else
                        {
                            instructionSet.MoveNext();
                        }
                    }
                    else
                    {
                        Log.DebugAI("Waiting for delay");
                    }
                    break;
                }

                case Instruction.SoftDrop:
                {
                    if(curTime - LastSoftDropTime > SoftDropDelay)
                    {
                        // Log.Debug("Try Softdrop");
                        if(currentPiece.TryShift(new Point(0, -1)))
                        {
                            LastSoftDropTime = curTime;
                            LastUpdateTime = curTime;
                            instructionSet.MoveNext();
                            Log.DebugAI("Softdrop Success");
                        }
                        else
                        {
                            Log.DebugAI("Cannot softdrop");
                            instructionSet.MoveNext();
                        }
                    }
                    break;
                }

                case Instruction.Left:
                case Instruction.Right:
                {
                    if(curTime - lastMinoMoveTime > ARRDelay)
                    {
                        // Log.Debug("Try Move");
                        var direction = curMove == Instruction.Left ? new Point(-1, 0) : new Point(1, 0);
                        if(currentPiece.TryShift(direction))
                        {
                            Log.DebugAI("Move success");
                            lastMinoMoveTime = curTime;
                            LastUpdateTime = curTime;
                            instructionSet.MoveNext();
                        }
                        else
                        {
                            // exception
                        }
                    }
                    break;
                }
                
                case Instruction.Hold:
                {
                    // Log.Debug("Try Hold");
                    if(TrySwapHold())
                    {
                        Log.DebugAI("Hold success");
                        LastUpdateTime = curTime;
                        instructionSet.MoveNext();
                    }
                    else
                    {
                        throw new Exception();
                    }

                    break;
                }

                case Instruction.LockMino:
                {
                    lockCurrentMinoToPlace(curTime);
                    instructionSet.MoveNext();
                    break;
                }

                case Instruction.SkipToNextMino:
                case Instruction.InstructionDone:
                {
                    //CheckExpectedMinoPosIsCorrect();
                    this.instructionSet = null;
                    break;
                }

                case Instruction.None:
                    // 예외처리

                default:
                    throw new NotImplementedException($"Parameter {curMove} is not implemented function");
            }
        }

        protected override void OnGarbageLineReceived()
        {
            ai.NotifyGridChanged(tetrisGrid.getLines, this.Combo, this.B2B);
            // ai.RequestNextInstructionSet(incomingGarbageLine);
        } 

        bool TrySwapHold()
        {
            if(Hold == null)
            {
                Hold = currentPiece;
                currentPiece = new CurrentTetrominoPiece(tetrisGrid, tetrominoBag.GetNext(), spawnOffset);
                ai.NotifyBagConsumed();
                canHold = false;
                return true;
            }
            
            if(canHold)
            {
                var temp = Hold;
                Hold = currentPiece;
                currentPiece = temp;
                currentPiece.ResetToInitialState();
                canHold = false;
                return true;
            }
            else
                return false;
        }


        protected override bool TryCreateCurrentPiece(TimeSpan curTime)
        {
            if(curTime - lastMinoPlaced > minoSpawnDelay)
            {
                ai.RequestNextInstructionSet(incomingGarbageLine); // 이 함수가 실행되면, 반드시 다음 미노를 만들어야하거나, 아니면 게임이 종료되어야함
                var mino = this.tetrominoBag.GetNext();
                ai.NotifyBagConsumed();

                currentPiece = new CurrentTetrominoPiece(tetrisGrid, mino, spawnOffset);
                var expectedPos = currentPiece.GetPosOfBlocks();
                if(tetrisGrid.CanMinoExistHere(expectedPos))
                    return true;
                else
                {
                    Log.Info("Cannot spawn mino, set game state to Dead");
                    this.gameState = GameState.Dead;
                    return false;
                }
            }
            else
                return false;
        }

        void CheckExpectedMinoPosIsCorrect()
        {
            // var expected = this.instructionSet.expectedPoints;
            // 
        }

        public override IEnumerable<Tetromino> PeekBag()
        {
            return tetrominoBag.PeekMany(5);
        }
    }

    public interface AI
    {
        /*
        리퀘스트는 무조건 현재 미노가 null이고, 홀드가 있는지 없는지 모르면서, bag의 0번째가 다음으로 사용할 미노가 될 때
        */
        void OnGameInitialize();
        void RequestNextInstructionSet(Int32 incoming);
        bool TryGetInstructionSet(out InstructionSet instructions);
        void NotifyBagConsumed();
        void NotifyGridChanged(IEnumerable<TetrisLine> grid, int combo, bool b2b);
        void CleanReset(TetrominoBag BagToUse);
    }
}