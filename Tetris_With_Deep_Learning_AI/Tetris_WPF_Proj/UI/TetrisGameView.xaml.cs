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
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Media;
using Tetris;
using Tetris.AudioModule;
using log4net;
using System.IO;
using SharpDX;
using SharpDX.XAudio2;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// TetrisGame.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TetrisGameView : UserControl
    {
        ILog Log = LogManager.GetLogger("TetrisGameUserControl");

        Stopwatch sw = new Stopwatch();
        List<TetrisGame> tetrisGames;
        List<iInputProvider> inputProviders;
        byte[] dontremove;

        public TetrisGameView()
        {
            InitializeComponent();
            sw.Reset();
        }

        public void SetTetrisGame(List<TetrisGame> games, List<iInputProvider> inputProviders)
        {
            games = games ?? new List<TetrisGame>();
            inputProviders = inputProviders ?? new List<iInputProvider>();
            this.tetrisGames = games;
            this.inputProviders = inputProviders;

            if (games.Count > 2 || inputProviders.Count > 2 || games.Count == 0)
                throw new Exception();

            //DEBUG
            var effectEventArgs = new EffectEventArgs();
            effectEventArgs.minoMoved = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_move.wav");
            effectEventArgs.minoHold = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_hold.wav") { volume = 0.5f };
            effectEventArgs.minoHardDropped = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_harddrop.wav") { volume = 0.3f };
            effectEventArgs.minoLocked = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_fixa.wav") { volume = 0.4f };
            effectEventArgs.minoRotated = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_rotate.wav") { volume = 0.7f };

            GameView_1.tetrisGame = games[0];
            GameView_2.tetrisGame = games.Count > 1 ? games[1] : null;

            SetSoundEffect(this, effectEventArgs);
        }

        List<SoundPlayer> players = new List<SoundPlayer>();

        public void SetSoundEffect(object sender, EventArgs e)
        {
            var effectEventArgs = e as EffectEventArgs;
            if (effectEventArgs == null)
                return;

            var SoundEffect = effectEventArgs;

            foreach(var game in tetrisGames)
            {
                SoundEffect.Subscribe(game.TetrisGameEvent);
            }
        }

        public void StartNewGame()
        { // 여기서 이거 하면 안된다 -> 옮겨야함 FIXME
            foreach(var game in tetrisGames)
            {
                game.ResetGame();
                game.InitializeGame();
                game.StartGame();
            }
            sw.Start();
            CompositionTarget.Rendering += OnUpdate;
        }

        public void PauseGame()
        {
            sw.Stop();
            CompositionTarget.Rendering -= OnUpdate;
        }

        void OnUpdate(object sender, EventArgs e)
        {
            foreach (var inputProvider in inputProviders)
                inputProvider.Update();

            foreach(var game in tetrisGames)
            {
                game.UpdateGame(sw.Elapsed);
            }

            if (tetrisGames.Count > 0)
                GameView_1.UpdateGameView();

            if (tetrisGames.Count > 1)
                GameView_2.UpdateGameView();
        }
    }
}
