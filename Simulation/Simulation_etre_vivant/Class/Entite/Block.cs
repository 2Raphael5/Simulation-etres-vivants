using Class;
using Class.Entite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation_etre_vivant.Class.Entite
{
    public class Block
    {
        public string Name;
        public PixelGeneric Pixel;
        public float Life;

        public Block(string name, PixelGeneric pixel, float life)
        {
            Name = name;
            Pixel = pixel;
            Life = life;
        }

        public virtual void Spawn(SpriteBatch _spriteBach)
        {
            Pixel.DrawRectangle(_spriteBach);
        }

        public virtual Block Clone(Vector2 position)
        {
            return new Block(Name, new PixelGeneric(position, Pixel.Color), Life);
        }
    }
}
