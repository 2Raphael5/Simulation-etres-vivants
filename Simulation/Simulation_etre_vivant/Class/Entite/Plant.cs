using Microsoft.Xna.Framework;
using System;

namespace Class.Entite
{
    public class Plant : LivingBeing
    {
        Plant[] neighbourPlant = new Plant[4];
        public Plant(string name, float life, PixelGeneric pixel, float time) : base(name, life, pixel, time)
        {
            Timer = time;
        }

        public override void Update(float deltaTime)
        {
        }

        public Plant Grow()
        {
            Random rnd = new Random();
            int wayToGrow = rnd.Next(0, 4);
            while (neighbourPlant[wayToGrow] == null)
            {
                wayToGrow = rnd.Next(0, 4);
            }
            PixelGeneric pixel = new PixelGeneric(new Vector2(0,0), Pixel.Color);
            neighbourPlant[wayToGrow] = new Plant(Name, Life, pixel, 2);
            return neighbourPlant[wayToGrow];

        }
        public override LivingBeing Clone(Vector2 position)
        {
            return new Plant(Name, Life, new PixelGeneric(position, Pixel.Color), (int)TimeBeforeAction);
        }
    }   
}
