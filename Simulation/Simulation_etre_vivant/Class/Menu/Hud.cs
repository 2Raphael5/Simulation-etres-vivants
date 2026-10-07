using Class.DebugTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Class
{
    public class Hud
    {
        private readonly SpriteFont _font;
        public Hud(SpriteFont font)
        {
            _font = font;
        }
        public void DrawMenue(SpriteBatch spriteBatch, int score, int lives, Game game)
        {/*
            spriteBatch.DrawString(_font, $"Score : {score}", new Vector2(10, 10), Color.White); spriteBatch.DrawString(_font, $"Live: {lives}", new Vector2(10, 40), Color.White);

            spriteBatch.DrawString(_font, "Press R to restart", new Vector2(game.Window.ClientBounds.Width - 150, 0), Color.White);*/
        }

    }
}
