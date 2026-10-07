using Class.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace Class
{
    public class InputManager
    {
        private readonly KeyboardInputManager _keyboard = new();
        private readonly Dictionary<InputAction, Keys> _keyMap = new()
        {
            { InputAction.MoveLeft, Keys.Left },
            { InputAction.MoveRight, Keys.Right },
            { InputAction.MoveUp, Keys.Up },
            { InputAction.MoveDown, Keys.Down },
            { InputAction.Shoot, Keys.Space },
            { InputAction.Pause, Keys.P },
            { InputAction.Confirm, Keys.Enter },
            { InputAction.Restart, Keys.R },
            { InputAction.Debug, Keys.F1 },
            { InputAction.Exit, Keys.Escape},
        };
        private readonly Dictionary<InputAction, Buttons> _buttonMap = new()
        {
            { InputAction.MoveLeft, Buttons.DPadLeft },
            { InputAction.MoveRight, Buttons.DPadRight },
            { InputAction.MoveDown, Buttons.DPadDown },
            { InputAction.MoveUp, Buttons.DPadUp },
            { InputAction.Shoot, Buttons.A },
            { InputAction.Pause, Buttons.Start },
            { InputAction.Confirm, Buttons.A },
            { InputAction.Restart, Buttons.Y },
        };

        private GamePadState _previousGamePad;
        private GamePadState _currentGamePad;
        private const float StickDeadzone = 0.0000025f;
        public void Update(GameTime gameTime)
        {
            _keyboard.Update(gameTime, Keyboard.GetState());
            _previousGamePad = _currentGamePad;
            _currentGamePad = GamePad.GetState(PlayerIndex.One);
            
        }
        public bool IsDown(InputAction action)
        {
            if (_keyMap.TryGetValue(action, out Keys key) && _keyboard.IsDown(key))
            {
                return true;
            }
            if (_buttonMap.TryGetValue(action, out Buttons button) && _currentGamePad.IsButtonDown(button))
            {
                return true;
            }
            return IsDirectionActive(action, _currentGamePad.ThumbSticks.Left);
        }
        public bool IsJustPressed(InputAction action)
        {
            if (_keyMap.TryGetValue(action, out Keys key) && _keyboard.IsJustPressed(key))
            {
                return true;
            }
            if (_buttonMap.TryGetValue(action, out Buttons button))
            {
                bool wasDown = _previousGamePad.IsButtonDown(button);
                bool isDown = _currentGamePad.IsButtonDown(button);
                if (isDown && !wasDown)
                {
                    return true;
                }
            }
            bool wasDirectionActive = IsDirectionActive(action, _previousGamePad.ThumbSticks.Left);
            bool isDirectionActive = IsDirectionActive(action, _currentGamePad.ThumbSticks.Left);
            return isDirectionActive && !wasDirectionActive;
        }
        private bool IsDirectionActive(InputAction action, Vector2 stick)
        {
            return action switch
            {
                InputAction.MoveLeft => stick.X < -StickDeadzone,
                InputAction.MoveRight => stick.X > StickDeadzone,
                InputAction.MoveUp => stick.Y > StickDeadzone,
                InputAction.MoveDown => stick.Y < -StickDeadzone,
                _ => false,
            };
        }
    }
}

