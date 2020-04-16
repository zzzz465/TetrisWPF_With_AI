using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Diagnostics;

namespace Tetris
{
    public struct InputSetting
    {
        public Key CCW;
        public Key CW;
        public Key HardDrop;
        public Key SoftDrop;
        public Key Left;
        public Key Right;
        public Key Hold;
        public static readonly InputSetting Default = new InputSetting {CCW = Key.D, CW = Key.F, HardDrop = Key.I, SoftDrop = Key.K, Left = Key.J, Right = Key.L, Hold = Key.R };
    }
    public class UserInputManager : iInputProvider
    {
        Dictionary<Key, bool> LastKeyState = new Dictionary<Key, bool>();
        Dictionary<Key, KeyState> currentKeyState = new Dictionary<Key, KeyState>();
        HashSet<Key> ObservedKeys = new HashSet<Key>();
        InputSetting inputSetting;
        public UserInputManager(InputSetting inputSetting)
        {
            ObserveKey(
                inputSetting.CCW,
                inputSetting.CW,
                inputSetting.SoftDrop,
                inputSetting.HardDrop,
                inputSetting.Hold,
                inputSetting.Left,
                inputSetting.Right);

            this.inputSetting = inputSetting;
        }

        public void ObserveKey(Key key)
        {
            ObservedKeys.Add(key);
        }

        public void ObserveKey(params Key[] keys)
        {
            foreach(var key in keys)
                ObserveKey(key);
        }
        
        public void DeObserveKey(Key key)
        {
            ObservedKeys.Remove(key);
        }

        public void Update()
        {
            foreach(var key in ObservedKeys)
            {
                var isDown = Keyboard.IsKeyDown(key);

                KeyState curKeyState;

                if(!LastKeyState.ContainsKey(key))
                    LastKeyState[key] = false;

                if(isDown)
                {
                    if(LastKeyState[key])
                        curKeyState = KeyState.Down;
                    else
                        curKeyState = KeyState.ToggledDown;
                }
                else
                {
                    if(!LastKeyState[key])
                        curKeyState = KeyState.Up;
                    else
                        curKeyState = KeyState.ToggledUp;
                }

                currentKeyState[key] = curKeyState;
                LastKeyState[key] = isDown;
            }
        }

        public KeyState GetState(InputType inputType)
        {
            if (currentKeyState.TryGetValue(inputSetting.ConvertInputToKey(inputType), out var value))
                return value;
            else
                return KeyState.NotAvailable;
        }
    }

}