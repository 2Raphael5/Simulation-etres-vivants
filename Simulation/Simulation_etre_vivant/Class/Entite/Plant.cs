using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Entite
{
    public class Plant : LivinBeing
    {
        public float TimeToGrow;
        Plant[] neighbourPlant = new Plant[4];
        public Plant(string name, float life, PixelGeneric pixel, float timeToGrow) : base(name, life, pixel)
        {
            TimeToGrow = timeToGrow;
            Timer = timeToGrow;
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
        public override LivinBeing Clone(Vector2 position)
        {
            return new Plant(Name, Life, new PixelGeneric(position, Color.DarkGreen), TimeToGrow);
        }
    }   
}
