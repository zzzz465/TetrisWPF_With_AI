using System;
using Tetris.AudioModule;
using Tetris;
using System.Collections.Generic;

namespace Tetris_WPF_Proj
{
    public class SoundEffects : EventArgs
    {
        public readonly static SoundEffects Default = new SoundEffects()
        {
            minoMoved = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_move.wav"),
            minoHold = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_hold.wav") { volume = 0.5f },
            minoHardDropped = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_harddrop.wav") { volume = 0.3f },
            minoLocked = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_fixa.wav") { volume = 0.4f },
            minoRotated = new CachedSound("./Resources/SoundEffect/Tetris99/se_game_rotate.wav") { volume = 0.7f }
        };

        public CachedSound minoMoved;
        public CachedSound minoHold;
        public CachedSound minoHardDropped;
        public CachedSound minoLocked;
        public CachedSound minoRotated;
        public SoundEffects()
        { // default value

        }

        public void Subscribe(iTetrisGameEvent gameEvent)
        {
            gameEvent.CurMinoHardDropped += (sender, e) => 
            {
                AudioEngine.SoundEffect.PlaySound(minoHardDropped);
            };

            gameEvent.CurMinoMoved += (sender, e) => 
            {
                AudioEngine.SoundEffect.PlaySound(minoMoved);
            };

            gameEvent.Hold += (sender, e) => 
            {
                AudioEngine.SoundEffect.PlaySound(minoHold);
            };

            gameEvent.MinoLocked += (sender, e) => 
            {
                AudioEngine.SoundEffect.PlaySound(minoLocked);
            };

            gameEvent.CurMinoRotated += (sender, e) => 
            {
                AudioEngine.SoundEffect.PlaySound(minoRotated);
            };
        }
    }
}