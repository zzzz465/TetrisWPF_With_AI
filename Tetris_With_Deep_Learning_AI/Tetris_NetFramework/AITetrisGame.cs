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
            throw new NotImplementedException();
        }
    }
}