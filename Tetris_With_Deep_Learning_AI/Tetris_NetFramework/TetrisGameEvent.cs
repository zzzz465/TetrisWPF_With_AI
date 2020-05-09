using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Tetris
{
    public interface iTetrisGameEvent
    { // 공개용 인터페이스
        event EventHandler<EventArgs> BoardChanged;
        event EventHandler<EventArgs> LineCleared;
        event EventHandler<EventArgs> CurMinoMoved;
        event EventHandler<EventArgs> CurMinoRotated;
        event EventHandler<EventArgs> CurMinoHardDroped;
        event EventHandler<EventArgs> Hold;
    }
    public class TetrisGameEvent : iTetrisGameEvent
    { // Event 담당 클래스
        public event EventHandler<EventArgs> BoardChanged;
        public event EventHandler<EventArgs> LineCleared;
        public event EventHandler<EventArgs> CurMinoMoved;
        public event EventHandler<EventArgs> CurMinoRotated;
        public event EventHandler<EventArgs> CurMinoHardDroped;
        public event EventHandler<EventArgs> Hold;
        public void OnBoardChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = BoardChanged;
            if(handler != null)
                handler.Invoke(this, e);
        }
        public void OnLineCleared(EventArgs e)
        {
            EventHandler<EventArgs> handler = LineCleared;
            if(handler != null)
                handler.Invoke(this, e);
        }

        public void OnCurMinoMoved(EventArgs e)
        {
            EventHandler<EventArgs> handler = CurMinoMoved;
            if(handler != null)
                handler.Invoke(this, e);
        }

        public void OnCurMinoRotated(EventArgs e)
        {
            EventHandler<EventArgs> handler = CurMinoRotated;
            if(handler != null)
                handler.Invoke(this, e);
        }

        public void OnCurMinoHardDroped(EventArgs e)
        {
            EventHandler<EventArgs> handler = CurMinoHardDroped;
            if(handler != null)
                handler.Invoke(this, e);
        }

        public void OnHold(EventArgs e)
        {
            EventHandler<EventArgs> handler = Hold;
            if(handler != null)
                handler.Invoke(this, e);
        }
    }
}