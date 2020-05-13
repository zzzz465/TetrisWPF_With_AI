using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.XAudio2;
using SharpDX;
using SharpDX.Multimedia;
using System.IO;

namespace Tetris.AudioModule
{
    public class CachedSound
    {
        public AudioBuffer audioBuffer { get; private set; }
        public WaveFormat waveFormat { get; private set; }
        public string rawFilePath { get; private set; }
        public uint[] DecodedPacketsInfo { get; private set; }
        private float _volume = 1.0f;
        public float volume
        { // value -> 0.0f ~ 1.0f
            get
            {
                return _volume;
            }
            set
            {
                if (value > 1)
                    _volume = 1f;
                else if (value < 0)
                    _volume = 0f;
                else
                    _volume = value;
            }
        }
        public CachedSound(string FilePath)
        {
            this.rawFilePath = FilePath;
            CreateBuffer();
        }

        private void CreateBuffer()
        {
            var fs = new FileStream(rawFilePath, FileMode.Open);
            var stream = new SoundStream(fs);
            this.waveFormat = stream.Format;
            this.DecodedPacketsInfo = stream.DecodedPacketsInfo;
            var buffer = new AudioBuffer { Stream = stream.ToDataStream(), AudioBytes = (int)stream.Length, Flags = BufferFlags.EndOfStream };
            stream.Close();
            fs.Close();

            this.audioBuffer = buffer;
        }
    }
}