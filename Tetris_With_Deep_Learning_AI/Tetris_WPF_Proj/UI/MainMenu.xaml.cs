using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tetris;

namespace Tetris_WPF_Proj.UI
{
    /// <summary>
    /// MainMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void OnNavigationButtonClick(Object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            var mainWindowInstance = MainWindow.Instance;
            var str = btn.Content.ToString().ToLower();
            
            if(str.Contains("setting"))
            {
                var window = new SettingWindow();
                mainWindowInstance.OpenWindow(window);
            }
            else if(str.Contains("play"))
            {
                var tetrisGameView = new TetrisGameView();
                var globalSetting = GlobalSetting.Instance;
                (var p1, var p1_InputProvider) = CreateGame(globalSetting.Player1Setting);
                (var p2, var p2_InputProvider) = CreateGame(globalSetting.Player2Setting);
                var inputs = new List<iInputProvider>();
                if (p2_InputProvider != null)
                    inputs.Add(p2_InputProvider);
                if (p1_InputProvider != null)
                    inputs.Add(p1_InputProvider);
                tetrisGameView.SetTetrisGame(new List<TetrisGame>() { p1, p2 }, inputs);
                mainWindowInstance.OpenWindow(tetrisGameView);
                tetrisGameView.StartNewGame();
            }
            else if(str.Contains("exit") || str.Contains("quit"))
            {
                Application.Current.Shutdown(0);
            }
        }

        private (TetrisGame game, iInputProvider inputProvider) CreateGame(GlobalSetting.PlayerSetting setting)
        {
            if(setting.ai != null)
            {
                var game = new AITetrisGame(setting.ai, setting.PlayerGameSetting);
                return (game, null);
            }
            else
            {
                var inputProvider = new UserInputManager(setting.playerInputSetting);
                var game = new PlayerTetrisGame(inputProvider, setting.PlayerGameSetting);
                return (game, inputProvider);
            }
        }
    }
}
