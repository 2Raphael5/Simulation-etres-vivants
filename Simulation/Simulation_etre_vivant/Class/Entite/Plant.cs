using Microsoft.Xna.Framework;
using System;

namespace Class.Entite
{
    public class Plant : LivingBeing
    {
        public Plant(string name, float life, PixelGeneric pixel, float time) : base(name, life, pixel, time)
        {
            Timer = time;
        }

        public override void Update(float deltaTime)
        {
        }

        public override LivingBeing Clone(Vector2 position)
        {
            return new Plant(Name, Life, new PixelGeneric(position, Pixel.Color), (int)TimeBeforeAction);
        }
    }
}
