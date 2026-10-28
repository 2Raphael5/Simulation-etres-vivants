using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Class.Entite
{
    public class Animal : LivingBeing
    {
        public float Speed;
        public float VisionRange;
        public float MaxDistanceStep;
        public int NbChild;
        public int NbMaxChild;

        public Animal(string name, float life, PixelGeneric pixel, float speed, float visionRange, float time, int nbChild, int nbMaxChild) : base(name, life, pixel, time)
        {
            Speed = speed;
            VisionRange = visionRange;
            NbChild = nbChild;
            NbMaxChild = nbMaxChild;
        }
        public override void Update(float deltaTime)
        {
            Life--;
        }
        public virtual void Move()
        {
        }
        public override LivingBeing Clone(Vector2 position)
        {
            Animal animal = new Animal(Name, Life, new PixelGeneric(position, Pixel.Color), Speed, VisionRange, TimeBeforeAction, 0, NbMaxChild);
            animal.Timer = this.Timer;
            return animal;
        }
    }
}