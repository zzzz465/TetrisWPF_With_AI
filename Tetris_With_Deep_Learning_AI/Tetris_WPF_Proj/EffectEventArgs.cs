using System;
using Tetris.AudioModule;
using Tetris;

namespace Tetris_WPF_Proj
{
    public class EffectEventArgs : EventArgs
    {
        public CachedSound minoMoved;
        public CachedSound minoHold;
        public CachedSound minoHardDropped;
        public CachedSound minoLocked;
        public CachedSound minoRotated;
        public EffectEventArgs()
        {

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