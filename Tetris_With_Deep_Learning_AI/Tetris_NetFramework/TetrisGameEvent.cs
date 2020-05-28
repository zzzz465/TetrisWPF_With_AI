using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using log4net;
using OpenCvSharp;
using System.Windows.Media;

namespace Tetris
{
    public interface iTetrisGameEvent
    { // 공개용 인터페이스
        event EventHandler BoardChanged;
        event EventHandler LineCleared;
        event EventHandler CurMinoMoved;
        event EventHandler CurMinoRotated;
        event EventHandler CurMinoHardDropped;
        event EventHandler Hold;
        event EventHandler MinoLocked;
        event EventHandler SoftDropped;
    }
    public class TetrisGameEvent : iTetrisGameEvent
    { // Event 담당 클래스
        public event EventHandler BoardChanged;
        public event EventHandler LineCleared;
        public event EventHandler CurMinoMoved;
        public event EventHandler CurMinoRotated;
        public event EventHandler CurMinoHardDropped;
        public event EventHandler Hold;
        public event EventHandler MinoLocked;
        public event EventHandler SoftDropped;
        public void OnBoardChanged(EventArgs e)
        {
            EventHandler handler = BoardChanged;
            if(handler != null)
                handler.Invoke(this, e);
        }
        public void OnLineCleared(EventArgs e)
        {
            EventHandler handler = LineCleared;
            if(handler != null)
                handler.Invoke(this, e);
        }

        public void OnCurMinoMoved(EventArgs e)
        {
            EventHandler handler = CurMinoMoved;
            if(handler != null)
                handler.Invoke(this, e);
        }

        public void OnCurMinoRotated(EventArgs e)
        {
            EventHandler handler = CurMinoRotated;
            if(handler != null)
                handler.Invoke(this, e);
        }

        public void OnCurMinoHardDropped(HardDropEventArgs e)
        {
            EventHandler handler = CurMinoHardDropped;
            if(handler != null)
                handler.Invoke(this, e);
        }

        public void OnHold(EventArgs e)
        {
            EventHandler handler = Hold;
            if(handler != null)
                handler.Invoke(this, e);
        }

        public void OnMinoLocked(EventArgs e)
        {
            EventHandler handler = MinoLocked;
            if(handler != null)
                handler.Invoke(this, e);
        }

        public void OnSoftDropped(EventArgs e)
        {
            EventHandler handler = SoftDropped;
            if (handler != null)
                handler.Invoke(this, e);
        }
    }

    public class HardDropEventArgs : EventArgs
    {
        public IEnumerable<System.Drawing.Point> hardDropPos;
        public HardDropEventArgs(IEnumerable<System.Drawing.Point> hardDropPos) : base()
        {
            this.hardDropPos = hardDropPos;
        }
    }
    /*
    public class TetrisGameEventArgs
    {
        public bool softdropPressed;
        public TetrisGameEventArgs()
        {

        }
    }
    */
}