using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris;

namespace Tetris_WPF_Proj
{
    public class GlobalSetting
    {
        public class PlayerSetting
        {
            public TetrisGameSetting PlayerGameSetting = TetrisGameSetting.Default;
            public AI ai;
            public InputSetting playerInputSetting;
        }

        public static GlobalSetting Instance { get; } = new GlobalSetting(); // SINGLETON
        public PlayerSetting Player1Setting { get; private set; } = new PlayerSetting();
        public PlayerSetting Player2Setting { get; private set; } = new PlayerSetting();

        private GlobalSetting()
        {

        }
    }
}
