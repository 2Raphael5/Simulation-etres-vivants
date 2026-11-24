using Microsoft.Xna.Framework;

namespace Class.Entite
{
    public class Plant : LivingBeing
    {
        public Plant(string name, float life, PixelGeneric pixel, float time) : base(name, life, pixel, time)
        {
            Timer = time;
        }

        public override LivingBeing Clone(Vector2 position)
        {
            return new Plant(Name, Life, new PixelGeneric(position, Pixel.Color), (int)TimeBeforeAction);
        }
    }
}
