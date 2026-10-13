using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class.Entite
{
    public class LivinBeing
    {
        public string Name;
        public float Life;
        public PixelGeneric Pixel;
        public float Timer;

        public LivinBeing(string name, float life, PixelGeneric pixel)
        {
            Name = name;
            Life = life;
            Pixel = pixel;
        }

        public virtual void Update(float deltaTime)
        {
            if (Life<=0)
            {
                Die();
            }
        }
        public void Spawn(SpriteBatch _spriteBach)
        {
            Pixel.DrawRectangle(_spriteBach);
        }

        public void TakeDamage()
        {
            if (Life > 0)
            {
                Life -= 10;
            }
        }

        protected void Die() 
        { 
            Pixel = null;
        }
        public virtual LivinBeing Clone(Vector2 position)
        {
            return new LivinBeing(Name, Life, new PixelGeneric(position, Color.White));
        }
    }
}
