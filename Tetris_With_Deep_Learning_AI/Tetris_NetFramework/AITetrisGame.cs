using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using log4net;

namespace Tetris
{
    public class AITetrisGame : TetrisGame // CC봇으로 먼저 만들어보자.
    {
        ILog Log;
        AI ai;
        InstructionSet instructions;
        TimeSpan LastUpdateTime = TimeSpan.Zero;
        TimeSpan UpdateDelay = TimeSpan.FromMilliseconds(16);
        public IEnumerable<Point> expectedMinoEndPoints { get { return instructions?.expectedPoints; } }
        Queue<Tetromino> AIBag = new Queue<Tetromino>();
        public AITetrisGame(AI ai, AIGameSetting AIGameSetting, TetrominoBag bag = null) : base(AIGameSetting, bag)
        {
            this.UpdateDelay = AIGameSetting.UpdateDelay;
            this.ai = ai;
            Log = LogManager.GetLogger("AITetrisGame");
            // throw new NotImplementedException();
        }

        public override void StartGame()
        {
            Log.Debug("Starting game...");
            ai.Reset();
            for(int i = 0; i < 5; i++)
            {
                var nextMino = tetrominoBag.GetNext();
                AIBag.Enqueue(nextMino);
                ai.AddMino(nextMino);
                Log.Debug($"Add initial tetromino {nextMino} to AI");
            }

            InstructionSet InitInstructions = null;
            {
                Log.Debug("Trying to get Initial instructions from AI");
                for(int i = 0; i < 5; i++)
                {
                    if (ai.TryGetInstructionSet(0, out InitInstructions))
                    {
                        Log.Debug($"received Initial instructions, Length : {InitInstructions.Length}");

                        while(InitInstructions.MoveNext())
                            Log.Debug($"{InitInstructions.Current} | {InitInstructions.index}");
                        
                        InitInstructions.Reset();

                        break;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1);
                        Log.Debug("Failed to get initial instructions... retry count : " + i);
                    }
                }
            }

            // ai.AddMino(tetrominoBag.Peek(peekIndex++));

            this.instructions = InitInstructions;
            if(!instructions.MoveNext())
                throw new Exception();

            this.gameState = GameState.Playing;
            Log.Debug($"Set Gamestate to {this.gameState}");
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

            if (currentPiece == null)
            { // 메소드로 분리하기
                if (curTime - lastMinoPlaced > minoSpawnDelay)
                {
                    Log.Debug("trying to get new CurrentTetrominoPiece...");
                    currentPiece = new CurrentTetrominoPiece(tetrisGrid, AIBag.Dequeue(), spawnOffset);
                    var expectedPos = currentPiece.GetPosOfBlocks();
                    if(tetrisGrid.CanMinoExistHere(expectedPos) == false)
                    {
                        Log.Debug("Cannot place currentTetrominoPiece to spawn offset");
                        this.gameState = GameState.Dead;
                    }
                }

                if(currentPiece == null)
                    return;
            }

            var curMove = instructions.Current;
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
                            Log.Debug("CCW success");
                            LastUpdateTime = curTime;
                            instructions.MoveNext();
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
                            Log.Debug("CW Success");
                            LastUpdateTime = curTime;
                            instructions.MoveNext();
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
                    Log.Debug("Try HardDrop");
                    while(currentPiece.TryShift(new Point(0, -1)));
                    LastUpdateTime = curTime;
                    instructions.MoveNext();
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
                            Log.Debug("SoftDrop to bottom");
                        }
                        else
                        {
                            instructions.MoveNext();
                        }
                    }
                    else
                    {
                        Log.Debug("Waiting for delay");
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
                            instructions.MoveNext();
                            Log.Debug("Softdrop Success");
                        }
                        else
                        {
                            Log.Debug("Cannot softdrop");
                            instructions.MoveNext();
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
                            Log.Debug("Move success");
                            lastMinoMoveTime = curTime;
                            LastUpdateTime = curTime;
                            instructions.MoveNext();
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
                        Log.Debug("Hold success");
                        LastUpdateTime = curTime;
                        instructions.MoveNext();
                    }
                    else
                    {
                        throw new Exception();
                    }

                    break;
                }

                case Instruction.LockMino:
                {
                    lockCurrentMinoToPlace();
                    instructions.MoveNext();
                    break;
                }

                case Instruction.SkipToNextMino:
                case Instruction.InstructionDone:
                {
                    GetNextInstructions();
                    break;
                }

                case Instruction.None:
                    // 예외처리

                default:
                    throw new NotImplementedException($"Parameter {curMove} is not implemented function");
            }
        }
        
        bool TrySwapHold()
        {
            if(Hold == null)
            {
                Hold = currentPiece;
                currentPiece = new CurrentTetrominoPiece(tetrisGrid, AIBag.Dequeue(), spawnOffset);
                var nextMino = tetrominoBag.GetNext();
                AIBag.Enqueue(nextMino);
                ai.AddMino(nextMino);
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

        void lockCurrentMinoToPlace()
        {
            if(currentPiece == null)
                throw new InvalidOperationException("setMinoToPlace Method shouldn't be called when the currentPiece is null...");

            var PosOfCurrentPiece = currentPiece.GetPosOfBlocks();
            var isValid = tetrisGrid.CanMinoExistHere(PosOfCurrentPiece);
            if(!isValid)
                throw new Exception("Unexpected behaviour of currentPiece, currentPiece's current pos should always valid");

            tetrisGrid.Set(PosOfCurrentPiece, currentPiece.minoType);
            canHold = true;
            currentPiece = null;
        }

        void GetNextInstructions(int incoming = 0)
        {
            Log.Debug("Trying to get next instructions");
            if(ai.TryGetInstructionSet(incoming, out var newInstructions))
            {
                Log.Debug("Get next instructions success");
                Log.Debug($"Instructions list : (length : {newInstructions.Length}");

                // debug instructions
                while(newInstructions.MoveNext())
                    Log.Debug(newInstructions.Current);

                newInstructions.Reset();
                
                this.instructions = newInstructions;
                instructions.MoveNext();
                var nextMino = tetrominoBag.GetNext();
                AIBag.Enqueue(nextMino);
                ai.AddMino(nextMino);
            }
        }

        public override IEnumerable<Tetromino> PeekBag()
        {
            return AIBag;
        }
    }

    public interface AI
    {
        void AddMino(Tetromino mino);
        bool TryGetInstructionSet(Int32 incoming, out InstructionSet instructions);
        void Reset();
        void Reset(bool[] grid, bool b2b, int combo);
    }
}