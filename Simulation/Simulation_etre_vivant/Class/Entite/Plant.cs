using Class;
using Class.Entite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation_etre_vivant.Class.Entite
{
    public class Plant : LivinBeing
    {
        protected float TimeToGrow;
        Plant[] neighbourPlant = new Plant[4];
        public Plant(string name, float life, PixelGeneric pixel, float timebyGrowth) : base(name, life, pixel)
        {
            TimeToGrow = timebyGrowth;
        }

        public override void Update(float deltaTime)
        {
            _timer += deltaTime;

            if (_timer >= TimeToGrow)
            {
                _timer = 0f;
                Grow();
            }
        }

        public void Grow()
        {
            Random rnd = new Random();
            int wayToGrow = rnd.Next(0, 4);
            if (neighbourPlant[wayToGrow] == null)
            {
                PixelGeneric pixel = new PixelGeneric(Pixel.Position, Pixel.Color);
                neighbourPlant[wayToGrow] = new Plant(Name, Life, pixel, 2);
            }
        }
    }
}
