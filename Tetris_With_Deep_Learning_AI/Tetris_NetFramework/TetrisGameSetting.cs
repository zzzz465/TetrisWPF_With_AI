using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    public struct TetrisGameSetting
    {
        public TimeSpan ARRDelay;
        public TimeSpan DASDelay;
        public TimeSpan minoSpawnDelay;
        public TimeSpan softDropDelay;
        public TimeSpan autoDropDelay;
        public static readonly TetrisGameSetting Default = new TetrisGameSetting() 
        { 
            ARRDelay = TimeSpan.FromMilliseconds(100),
            DASDelay = TimeSpan.FromMilliseconds(250),
            minoSpawnDelay = TimeSpan.FromMilliseconds(280),
            softDropDelay = TimeSpan.FromMilliseconds(70),
            autoDropDelay = TimeSpan.FromMilliseconds(700)
        };
    }
}