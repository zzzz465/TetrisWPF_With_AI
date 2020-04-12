using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Diagnostics;

namespace Tetris
{
    public class InputManager : iInputProvider
    {
        public struct Setting
        {
            public int ARR;
            public int DAS;
            public int DropDelay;
            public Key CCW;
            public Key CW;
            public Key HardDrop;
            public Key SoftDrop;
            public Key Left;
            public Key Right;
            public Key Hold;
            public static readonly Setting Default = new Setting { ARR = 100, DAS = 100, DropDelay = 300, CCW = Key.D, CW = Key.F, HardDrop = Key.I, SoftDrop = Key.K, Left = Key.J, Right = Key.L, Hold = Key.R };
        }
        #region settings
        int ARR_Delay;
        int DAS_Delay;
        int Drop_Delay;
        Setting _setting;
        public Setting setting 
        {
            get
            {
                return _setting;
            }
            set
            {
                _setting = value;
                ApplySetting();
            }
        }
        #endregion

        #region Key variables
        Key Left { get { return _setting.Left; } }
        Key Right { get { return _setting.Right; } }
        Key HardDrop { get { return _setting.HardDrop; } }
        Key SoftDrop { get { return _setting.SoftDrop; } }
        Key Hold { get { return _setting.Hold; } }
        Key CCW { get { return _setting.CCW; } }
        Key CW { get { return _setting.CW; } }
        #endregion

        Stopwatch sw = new Stopwatch();
        TimeSpan LastARRTime;
        TimeSpan LastDASTime;
        bool isMoving = false;
        TimeSpan LastSoftDropTime;
        HashSet<Key> LastKeyState = new HashSet<Key>();
        
        public InputManager(Setting setting)
        {
            this.setting = setting;
            ApplySetting();
            sw.Start();
        }

        void ApplySetting()
        {
            this.ARR_Delay = setting.ARR;
            this.DAS_Delay = setting.DAS;
            this.Drop_Delay = setting.DropDelay;
        }
        
        public InputState GetInputState()
        {     
            TimeSpan curTime = sw.Elapsed;
            var curKeyState = new HashSet<Key>();
            var state = InputState.None;


            if(Keyboard.IsKeyDown(Left) && Keyboard.IsKeyDown(Right))
            {
                curKeyState.Add(Left);
                curKeyState.Add(Right);
                // 무시
            }
            else if(Keyboard.IsKeyDown(Left) || Keyboard.IsKeyDown(Right))
            {
                var isLeft = Keyboard.IsKeyDown(Left);
                var MoveDeltaTime = (curTime - LastDASTime).TotalMilliseconds;
                if(!isMoving)
                {
                    if(MoveDeltaTime > DAS_Delay)
                    {
                        isMoving = true;
                        LastDASTime = curTime;
                        LastARRTime = curTime;

                        state = isLeft ? state |= InputState.LeftPressed : state |= InputState.RightPressed;
                    }
                }
                else
                {
                    if(MoveDeltaTime > ARR_Delay)
                    {
                        LastDASTime = curTime;
                        LastARRTime = curTime;

                        state = isLeft ? state |= InputState.LeftPressed : state |= InputState.RightPressed;
                    }
                }
            }
            else
                isMoving = false;
            
            if(Keyboard.IsKeyDown(SoftDrop))
            {
                var deltaTime = (curTime - LastSoftDropTime).TotalMilliseconds;
                if(deltaTime > Drop_Delay)
                {
                    LastSoftDropTime = curTime;
                    state |= InputState.SoftDrop;
                }

                curKeyState.Add(SoftDrop);
            }

            if(Keyboard.IsKeyDown(HardDrop))
            {
                if(!LastKeyState.Contains(HardDrop))
                    state |= InputState.HardDrop;
                
                curKeyState.Add(HardDrop);
            }

            if(Keyboard.IsKeyDown(CCW) && Keyboard.IsKeyDown(CW))
            {
                // ignore
                curKeyState.Add(CCW);
                curKeyState.Add(CW);
            }
            else if(Keyboard.IsKeyDown(CCW))
            {
                if(!LastKeyState.Contains(CCW))
                    state |= InputState.CCW;

                curKeyState.Add(CCW);
            }
            else if(Keyboard.IsKeyDown(CW))
            {
                if(!LastKeyState.Contains(CW))
                    state |= InputState.CW;
                
                curKeyState.Add(CW);
            }

            if(Keyboard.IsKeyDown(Hold))
            {
                if(!LastKeyState.Contains(Hold))
                    state |= InputState.Hold;

                curKeyState.Add(Hold);
            }

            return state;
        }
    }

}