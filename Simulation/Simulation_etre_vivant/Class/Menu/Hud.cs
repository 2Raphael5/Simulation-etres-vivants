using Class.Entite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation_etre_vivant;
using Simulation_etre_vivant.Class.Entite;
using Color = Microsoft.Xna.Framework.Color;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;


namespace Class
{
    public class Hud
    {
        private readonly SpriteFont Font;
        public Hud(SpriteFont font)
        {
            Font = font;
        }
        public void DrawHUD(SpriteBatch spriteBatch, Block selected, Color color, Game1 game)
        {            
            spriteBatch.DrawString(Font, $"{selected.Name}", new Vector2(0, 0), color);
           
        }

    }
}
