using Microsoft.Xna.Framework;

namespace Class.Entite
{
    public class Animal : LivingBeing
    {
        public float Speed;
        public float VisionRange;
        public float MaxDistanceStep;
        public Point Goal;
        public Animal(string name, float life, PixelGeneric pixel, float speed, float visionRange, float time) : base(name, life, pixel, time)
        {
            Speed = speed;
            VisionRange = visionRange;
        }
        public override void Update(float deltaTime)
        {
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
            Animal animal = new Animal(Name, Life, new PixelGeneric(position, Pixel.Color), Speed, VisionRange, TimeBeforeAction);
            animal.DefineGoal(Goal);
            return animal;
        }
    }
}