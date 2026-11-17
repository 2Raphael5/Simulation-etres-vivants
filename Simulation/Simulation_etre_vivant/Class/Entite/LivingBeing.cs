using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Class.Entite
{
    public class LivingBeing
    {
        public string Name;
        public float Life;
        public PixelGeneric Pixel;
        public float Timer;
        public float TimeBeforeAction;
        public LivingBeing(string name, float life, PixelGeneric pixel, float time)
        {
            Name = name;
            Life = life;
            Pixel = pixel;
            TimeBeforeAction = time;
        }

        public virtual void Update(float deltaTime)
        {
            if (Life <= 0)
            {
                Die();
            }
        }

        public void Spawn(SpriteBatch _spriteBach)
        {
            Pixel.DrawRectangle(_spriteBach);
        }

        public void Die()
        {
            Pixel = null;
        }

        public virtual LivingBeing Clone(Vector2 position)
        {
            return new LivingBeing(Name, Life, new PixelGeneric(position, Pixel.Color), TimeBeforeAction);
        }
    }
}
