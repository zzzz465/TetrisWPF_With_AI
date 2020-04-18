using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    public class TetrisGameSetting
    {
        public TimeSpan ARRDelay;
        public TimeSpan DASDelay;
        public TimeSpan minoSpawnDelay;
        public TimeSpan softDropDelay;
        public TimeSpan autoDropDelay;
        private TetrisGameSetting()
        {
            
        }
        public TetrisGameSetting(TimeSpan ARRDelay, TimeSpan DASDelay, TimeSpan minoSpawnDelay, TimeSpan softDropDelay, TimeSpan autoDropDelay)
        {
            this.ARRDelay = ARRDelay;
            this.DASDelay = DASDelay;
            this.minoSpawnDelay = minoSpawnDelay;
            this.softDropDelay = softDropDelay;
            this.autoDropDelay = autoDropDelay;
        }

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