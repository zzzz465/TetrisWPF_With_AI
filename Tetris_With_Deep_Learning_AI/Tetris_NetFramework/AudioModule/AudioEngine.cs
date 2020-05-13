using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using SharpDX.XAudio2;
using SharpDX;
using SharpDX.Multimedia;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris.AudioModule
{
    public class AudioEngine
    {
        #region static
        public static AudioEngine SoundEffect;
        private static Task DisposeThread;
        private static volatile bool DisposeThreadRun = true;
        private static BlockingCollection<Action> disposeAction = new BlockingCollection<Action>();
        static AudioEngine()
        {
            SoundEffect = new AudioEngine();
            InitializeDisposeThread();
        }
        static void InitializeDisposeThread()
        {
            DisposeThread = Task.Run(() => 
            {
                while(AudioEngine.DisposeThreadRun)
                {
                    
                    var action = disposeAction.Take();
                    action();
                    Console.WriteLine("Disposed!");
                }

                disposeAction.CompleteAdding();
                disposeAction.Dispose();
                disposeAction = null;
            });
        }
        #endregion
        XAudio2 xAudio2;
        MasteringVoice masteringVoice;
        private AudioEngine()
        {
            this.xAudio2 = new XAudio2();
            masteringVoice = new MasteringVoice(xAudio2);
        }

        public void PlaySound(CachedSound cachedSound)
        {
            var sourceVoice = new SourceVoice(xAudio2, cachedSound.waveFormat, true);
            sourceVoice.SetVolume(cachedSound.volume);
            sourceVoice.SubmitSourceBuffer(cachedSound.audioBuffer, cachedSound.DecodedPacketsInfo);
            sourceVoice.BufferEnd += (context) => 
            {
                AudioEngine.disposeAction.Add(() => 
                {
                    sourceVoice.DestroyVoice();
                    sourceVoice.Dispose();
                });
            };
            sourceVoice.Start();
        }
    }
}