using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    public class AITetrisGame : TetrisGame
    {
        public AITetrisGame(TetrisGameSetting gameSetting, TetrominoBag bag = null) : base(gameSetting, bag)
        {
            throw new NotImplementedException();
        }

        protected override void Update(TimeSpan curTime)
        {
            if(this.gameState != GameState.Playing)
                return;

            iMovementProvider provider = null;

            var curMove = provider.GetMovement();

            switch(curMove)
            {
                case Instruction.CCW:
                case Instruction.CW:
                {
                    break;
                }

                case Instruction.HardDrop:
                {
                    break;
                }

                case Instruction.SoftDrop:
                {
                    break;
                }

                case Instruction.Left:
                case Instruction.Right:
                {
                    break;
                }
                
                case Instruction.Hold:
                {
                    break;
                }

                default:
                    throw new NotImplementedException($"Parameter {curMove} is not implemented function");
            }
        }
    }

    public interface iMovementProvider
    {
        Instruction GetMovement();
        void PollNewMove();
    }
}