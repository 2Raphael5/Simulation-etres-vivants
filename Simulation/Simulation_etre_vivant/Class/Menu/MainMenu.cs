using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Class.DebugTools;
using Class.Input;
namespace Class
{
    public class MainMenu
    {
        private readonly string[] _options = { "Play", "Quit" };
        private int _selectedIndex = 0;
        public int SelectedIndex => _selectedIndex;
        public void Update(InputManager _inputAction)
        {
            if (_inputAction.IsJustPressed(InputAction.MoveUp))
            {
                _selectedIndex++;
                if (_selectedIndex >= _options.Length)
{
                    _selectedIndex = 0;
                }
            }
            if (_inputAction.IsJustPressed(InputAction.MoveDown))
            {
                _selectedIndex--;
                if (_selectedIndex < 0)
                {
                    _selectedIndex = _options.Length - 1;
                }
            }
        }
        public bool IsConfirmed(InputManager _input)
        {
            return _input.IsJustPressed(InputAction.Confirm);
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 startPosition)
        {
            for (int i = 0; i < _options.Length; i++)
            {
                Color color = (i == _selectedIndex) ? Color.Yellow : Color.Black;
                Vector2 position = startPosition + new Vector2(0, i * 40);
                spriteBatch.DrawString(font, _options[i], position, color);
            }
        }

    }
}
