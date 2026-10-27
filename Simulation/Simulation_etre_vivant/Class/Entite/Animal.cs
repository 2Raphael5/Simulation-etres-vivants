using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Class.Entite
{
    public class Animal : LivingBeing
    {
        public float Speed;
        public float VisionRange;
        public float MaxDistanceStep;
        public Point Goal;
        public int NbMaxChild;

        public Animal(string name, float life, PixelGeneric pixel, float speed, float visionRange, float time, int nbMaxChild) : base(name, life, pixel, time)
        {
            Speed = speed;
            VisionRange = visionRange;
            NbMaxChild = nbMaxChild;
        }
        public override void Update(float deltaTime)
        {
            Life--;
        }
        public virtual void Move()
        {
        }
        public void DefineGoal(Point point)
        {
            Goal = point;
        }
        public override LivingBeing Clone(Vector2 position)
        {
            Animal animal = new Animal(Name, Life, new PixelGeneric(position, Pixel.Color), Speed, VisionRange, TimeBeforeAction, NbMaxChild);
            animal.DefineGoal(Goal);
            animal.Timer = this.Timer;
            return animal;
        }
    }
}