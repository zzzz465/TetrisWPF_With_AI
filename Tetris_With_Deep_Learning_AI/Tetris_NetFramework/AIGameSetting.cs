using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    public class AIGameSetting : TetrisGameSetting
    {
        public TimeSpan UpdateDelay;

        public AIGameSetting(TimeSpan UpdateDelay,
                             TimeSpan ARRDelay,
                             TimeSpan minoSpawnDelay,
                             TimeSpan softDropDelay) : base(
                                 ARRDelay,
                                 TimeSpan.Zero,
                                 minoSpawnDelay,
                                 softDropDelay,
                                 TimeSpan.Zero)
        {
            this.UpdateDelay = UpdateDelay;
        }
    }
}