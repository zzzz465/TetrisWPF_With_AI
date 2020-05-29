using ColdClear;
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
            public AI ai = null;
            public InputSetting playerInputSetting = InputSetting.Default;
            public PlayerType playerType = PlayerType.Player;
        }

        public static GlobalSetting Instance { get; } = new GlobalSetting(); // SINGLETON
        public PlayerSetting Player1Setting { get; private set; }
        public PlayerSetting Player2Setting { get; private set; }

        private GlobalSetting()
        {
            Player1Setting = new PlayerSetting();
            Player1Setting.playerType = PlayerType.Player;

            Player2Setting = new PlayerSetting();
            Player2Setting.playerType = PlayerType.AI;
            Player2Setting.ai = ColdClearAI.CreateInstance();
        }
    }
}
