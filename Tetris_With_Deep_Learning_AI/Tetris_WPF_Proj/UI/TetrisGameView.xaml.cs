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
using System.Diagnostics;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// TetrisGame.xaml에 대한 상호 작용 논리
    /// </summary>
    public enum State
    {
        Playing,
        Paused
    }

    public partial class TetrisGameView : UserControl
    { // 뷰모델과 모델이 결합했다...
        ILog Log = LogManager.GetLogger("TetrisGameUserControl");

        Stopwatch sw = new Stopwatch();
        TetrisGame _p1;
        TetrisGame _p2;
        public TetrisGame player1 { get { return _p1; } set { _p1 = value; GameView_1.tetrisGame = value; } }
        public TetrisGame player2 { get { return _p2; } set { _p2 = value; GameView_2.tetrisGame = value; } }
        public List<iInputProvider> inputProviders { get; set; }


        public State gameState
        {
            get { return (State)GetValue(gameStateProperty); }
            set { SetValue(gameStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for gameState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty gameStateProperty =
            DependencyProperty.Register("gameState", typeof(State), typeof(TetrisGameView), new PropertyMetadata(State.Playing));

        public TetrisGameView()
        {
            InitializeComponent();

            var effectEventArgs = new EffectEventArgs();
            effectEventArgs.minoMoved = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_move.wav");
            effectEventArgs.minoHold = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_hold.wav") { volume = 0.5f };
            effectEventArgs.minoHardDropped = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_harddrop.wav") { volume = 0.3f };
            effectEventArgs.minoLocked = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_fixa.wav") { volume = 0.4f };
            effectEventArgs.minoRotated = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_rotate.wav") { volume = 0.7f };

            SetSoundEffect(this, effectEventArgs);
            CompositionTarget.Rendering += this.OnUpdate;
        }

        public TetrisGameView(TetrisGame player1, TetrisGame player2, List<iInputProvider> inputProviders) : this()
        {
            Debug.Assert(player1 != null || player2 != null, "Player 1 and Player 2 is all null");

            // initialize userControl
            this.player1 = player1;
            this.player2 = player2;

            this.inputProviders = inputProviders;
        }

        public void SetSoundEffect(object sender, EventArgs e)
        {
            var effectEventArgs = e as EffectEventArgs;
            if (effectEventArgs == null)
                return;

            var SoundEffect = effectEventArgs;
            if(player1 != null)
                SoundEffect.Subscribe(player1.TetrisGameEvent);
            if(player2 != null)
                SoundEffect.Subscribe(player2.TetrisGameEvent);
        }

        public void StartNewGame()
        { // 여기서 이거 하면 안된다 -> 옮겨야함 FIXME
            Action<TetrisGame> doGame = (game) =>
            {
                game.ResetGame();
                game.InitializeGame();
                game.StartGame();
            };

            if (player1 != null)
                doGame(player1);
            if (player2 != null)
                doGame(player2);

            sw.Start();
        }

        public void TogglePause()
        {
            if(gameState == State.Playing)
            {
                sw.Stop();
                this.gameState = State.Paused;
            }
            else
            {
                sw.Start();
                this.gameState = State.Playing;
            }
        }

        void OnUpdate(object sender, EventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.Escape))
                TogglePause();

            if (this.gameState != State.Playing)
                return;

            if(inputProviders != null)
                foreach (var inputProvider in inputProviders)
                    inputProvider.Update();

            var elapsed = sw.Elapsed;

            if (player1 != null)
            {
                player1.UpdateGame(elapsed);
                GameView_1.UpdateGameView();
            }

            if(player2 != null)
            {
                player2.UpdateGame(elapsed);
                GameView_2.UpdateGameView();
            }
        }
    }
}
