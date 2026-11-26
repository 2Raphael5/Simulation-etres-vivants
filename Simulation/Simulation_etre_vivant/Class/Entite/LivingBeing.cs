using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation_etre_vivant.Class.Entite;

namespace Class.Entite
{
    public class LivingBeing : Block
    {
        public float Timer;
        public float TimeBeforeAction;
        public LivingBeing(string name, float life, PixelGeneric pixel, float time):base(name, pixel, life)
        {
            TimeBeforeAction = time;
        }

        public void Die()
        {
            Pixel = null;
        }

        public override LivingBeing Clone(Vector2 position)
        {
            return new LivingBeing(Name, Life, new PixelGeneric(position, Pixel.Color), TimeBeforeAction);
        }
    }
}
