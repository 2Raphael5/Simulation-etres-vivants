using Class.Entite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Entite
{
    public class Animal : LivinBeing
    {
        public float Speed;
        public float VisionRange;
        public Animal(string name, float life, PixelGeneric pixel, float speed, float visionRange) : base(name, life, pixel)
        {
            Speed = speed;
            VisionRange = visionRange;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
        public void Move()
        {

        }
    }
}